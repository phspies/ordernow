using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using order_microservice.Datamodels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace order_microservice.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private OrderDBContext OrderDBContext;
        private ILogger<OrderController> logger;

        public OrderController(OrderDBContext context, ILogger<OrderController> _logger)
        {
            OrderDBContext = context;
            logger = _logger;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDataModel>>> List()
        {
            using (var db = new OrderDataAccess(OrderDBContext, logger))
            {
                logger.LogInformation("Retrieving all Orders API");
                return await db.GetListAsync();
            }
        }
        // GET: api/orders/xxxx-xxxx-xxxx-xxxx
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDataModel>> Get(Guid id)
        {
            using (var db = new OrderDataAccess(OrderDBContext, logger))
            {
                logger.LogInformation($"Retrieving Order API: {id}");
                return await db.GetAsync(id);
            }
        }
        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDataModel>> Create(CreateOrderDataModel Order)
        {
            using (var db = new OrderDataAccess(OrderDBContext, logger))
            {
                logger.LogInformation($"Creating Order API: {JsonSerializer.Serialize(Order)}");
                return await db.CreateAsync(Order);
            }
        }
        // PUT: api/orders/xxxx-xxxx-xxxx-xxxx
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDataModel>> Put(Guid id, UpdateOrderDataModel Order)
        {
            logger.LogInformation($"Update Order API: {id}:{JsonSerializer.Serialize(Order)}");
            using (var db = new OrderDataAccess(OrderDBContext, logger))
            {
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                return await db.UpdateAsync(id, Order);
            }
        }
        // DELETE: api/order/xxxx-xxxx-xxxx-xxxx
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItem(Guid id)
        {

            using (var db = new OrderDataAccess(OrderDBContext, logger))
            {
                logger.LogInformation($"Delete Order API: {id}");
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                await db.DeleteAsync(id);
                return Ok();
            }
        }

        private bool OrderExists(Guid id) => this.OrderDBContext.Orders.Any(e => e.Id == id);

    }
}
