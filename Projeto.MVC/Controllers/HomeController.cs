using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Projeto.MVC.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

namespace Projeto.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

      
        public IActionResult Index()
        {
            var usuario = User;
            var token = HttpContext.GetTokenAsync("access_token").Result;
           
            /// Irá conseguir acessar pois o MVC está configurado para acessar a ApiUm atarvés do scopeUm
            var retornoUm = ListaApiUm();

            /// Não irá acessar
            /// , pois o token que Api recebe vem do MVC e ApiDois reconhece que apenas 
            /// o scopeApi está configurado no Client do MVC

            var retornoDoisPelaUm = ListaApiDoisPelaApiUm();

            /// Não irá acessar pelo mesmo motivo do anterior 
            var retornoDois = ListaApiDois();

            ///COMO RESOLVER ESSA QUESTÂO?
            /// Foi declarado no Config do IdentityServer o scopeGateway que pertence a ambos resources 
            /// resourceApiUm e resourceApiDois
            /// basta a configuração do MVC substituir o scopeApi pelo scopeGateway que ele acessará ambas Apis

            return View();
        }

        public IActionResult Privacy()
        {
           
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("oidc");
            HttpContext.SignOutAsync("Cookies");

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        

        private List<TesteApi1> ListaApiUm()
        {

            var retorno = new List<TesteApi1>();
            var client = new HttpClient();
            client.ClientApi("https://localhost:44386/", HttpContext.GetTokenAsync("access_token").Result);

            using (HttpResponseMessage response = client.GetAsync("api/weatherforecast").Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    retorno = JsonConvert.DeserializeObject<List<TesteApi1>>(responseBody);
                }
                    
            }

            return retorno;
        }

        private List<TesteApi1> ListaApiDoisPelaApiUm()
        {

            var retorno = new List<TesteApi1>();
            var client = new HttpClient();
            client.ClientApi("https://localhost:44386/", HttpContext.GetTokenAsync("access_token").Result);

            using (HttpResponseMessage response = client.GetAsync("api/weatherforecast/ListaApiDoisPelaApiUm").Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    retorno = JsonConvert.DeserializeObject<List<TesteApi1>>(responseBody);
                }

            }

            return retorno;
        }

        private List<TesteApi2> ListaApiDois()
        {
            var retorno = new List<TesteApi2>();
            var client = new HttpClient();
            client.ClientApi("https://localhost:44315/", HttpContext.GetTokenAsync("access_token").Result);

            using (HttpResponseMessage response = client.GetAsync("api/weatherforecast").Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    retorno = JsonConvert.DeserializeObject<List<TesteApi2>>(responseBody);
                }

            }

            return retorno;
        }
    }
}
