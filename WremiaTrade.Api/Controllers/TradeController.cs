namespace WremiaTrade.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using WremiaTrade.Models.Dtos;
    using WremiaTrade.Services.Trade.Interfaces;

    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService tradeService;
        private readonly ILogger<TradeController> logger;

        public TradeController(ITradeService tradeService, ILogger<TradeController> logger)
        {
            this.tradeService = tradeService;
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TradeOrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTradeAsync([FromBody] TradeOrderRequest request, CancellationToken cancellationToken)
        {
            var result = await tradeService.CreateOrderAsync(request, cancellationToken);

            if (!result.Succeeded || result.Data == null)
            {
                logger.LogWarning("Trade creation failed: {Error}", result.Error?.Message);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetTradeByIdAsync), new { id = result.Data.Id }, result.Data);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TradeOrderDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTradesAsync(CancellationToken cancellationToken)
        {
            var result = await tradeService.GetOrdersAsync(cancellationToken);
            return Ok(result.Data);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TradeOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTradeByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await tradeService.GetOrderAsync(id, cancellationToken);
            if (!result.Succeeded || result.Data == null)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Data);
        }
    }
}
