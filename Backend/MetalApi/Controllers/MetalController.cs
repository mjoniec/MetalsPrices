﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using MetalApi.Providers;
using MetalApi.GuandlModel;
using CommonReadModel;

namespace MetalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetalController : ControllerBase
    {
        private readonly IMetalProvider _metalsPricesProvider;

        public MetalController(IServiceProvider serviceProvider)
        {
            _metalsPricesProvider = (IMetalProvider)serviceProvider.GetService(typeof(GuandlMetalProvider));
        }

        [HttpGet("{metal}/{start}/{end}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Metal metal, DateTime start, DateTime end)
        {
            var json = await _metalsPricesProvider.Get(metal);

            var metalPrices = json
                .Deserialize()
                .Map(start, end);

            if (metalPrices == null)
            {
                return NoContent();
            }

            return Ok(metalPrices);
        }
    }
}