using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoDois.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoDois.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
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

        public IEnumerable<TesteDois> Get()
        {

            List<TesteDois> lista = new List<TesteDois>()
            {
                new TesteDois(){Id = Guid.NewGuid(), Descricao = "Teste 2 - 1"},
                new TesteDois(){Id = Guid.NewGuid(), Descricao = "Teste 2 - 2"},
                new TesteDois(){Id = Guid.NewGuid(), Descricao = "Teste 2 - 3"},
                new TesteDois(){Id = Guid.NewGuid(), Descricao = "Teste 2 - 4"}
            };

            return lista;
        }
    }
}
