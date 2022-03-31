using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repos;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MyController : ControllerBase
    {
        private readonly ILogger<MyController> _logger;
        private readonly IRepository _repository;

        public MyController(ILogger<MyController> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<List<MongoAxiomaData>> GetAsync()
        {
            return await _repository.GetAllResults();
        }

        //в адрес надо писать пока localhost
        public record Dto(int Minutes, string Address);

        [HttpPost]
        public async Task<OkResult> Start([FromBody] Dto dto)
        {
            AxiomaDataExtractor axiomaDataExtractor = new(_repository);
            axiomaDataExtractor.Process(dto.Address, dto.Minutes);
            return Ok();
        }

    }
}
