using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json;
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
        public async Task<ActionResult> GetAsync()
        {
            var r = await _repository.GetAllResults();
            return Ok(r);
        }

        ////в адрес надо писать пока localhost
        //public record Dto(int Minutes, string Address);

        //[HttpPost]
        //public async Task<OkResult> Start([FromBody] Dto dto)
        //{
        //    AxiomaDataExtractor axiomaDataExtractor = new(_repository);
        //    axiomaDataExtractor.Process(dto.Address, dto.Minutes);
        //    return Ok();
        //}

        /// <summary>
        /// Поинт, который принимает данные
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddData([FromBody] List<AxiomaData> Data)
        {
            await _repository.AddResult(new MongoAxiomaData() { Data = Data, DateTime = DateTime.UtcNow });
            return Ok();
        }

    }
}
