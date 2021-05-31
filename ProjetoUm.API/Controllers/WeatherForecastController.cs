using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjetoUm.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjetoUm.API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        

        [HttpGet]

        public IEnumerable<TesteApiUm> Get()
        {

            List<TesteApiUm> lista = new List<TesteApiUm>()
            {
                new TesteApiUm(){Id = Guid.NewGuid(), Descricao = "Teste 1 - 1"},
                new TesteApiUm(){Id = Guid.NewGuid(), Descricao = "Teste 1 - 2"},
                new TesteApiUm(){Id = Guid.NewGuid(), Descricao = "Teste 1 - 3"},
                new TesteApiUm(){Id = Guid.NewGuid(), Descricao = "Teste 1 - 4"}
            };

            return lista;
        }

        [HttpGet]
        [Route("ListaApiDoisPelaApiUm")]
        public List<TesteApiDois> ListaApiDoisPelaApiUm()
        {
            var retorno = new List<TesteApiDois>();
            var client = new HttpClient();
            client.ClientApi("https://localhost:44315/", HttpContext.GetTokenAsync("access_token").Result);

            using (HttpResponseMessage response = client.GetAsync("api/weatherforecast").Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    retorno = JsonConvert.DeserializeObject<List<TesteApiDois>>(responseBody);
                }

            }

            return retorno;
        }
    }
}
