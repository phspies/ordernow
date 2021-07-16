using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using customer_microservice.Datamodels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace customer_microservice.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private CustomerDBContext customerDBContext;
        private ILogger<CustomerController> logger;

        public CustomerController(CustomerDBContext context, ILogger<CustomerController> _logger)
        {
            customerDBContext = context;
            logger = _logger;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDataModel>>> List()
        {
            using (var db = new CustomerDataAccess(customerDBContext, logger))
            {
                logger.LogInformation("Retrieving all customers API");
                return await db.GetListAsync();
            }
        }
        // GET: api/customers/xxxx-xxxx-xxxx-xxxx
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDataModel>> Get(Guid id)
        {
            using (var db = new CustomerDataAccess(customerDBContext, logger))
            {
                logger.LogInformation($"Retrieving customer API: {id}");
                return await db.GetAsync(id);
            }
        }
        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDataModel>> Create(CreateCustomerDataModel customer)
        {
            using (var db = new CustomerDataAccess(customerDBContext, logger))
            {
                logger.LogInformation($"Creating customer API: {JsonSerializer.Serialize(customer)}");
                return await db.CreateAsync(customer);
            }
        }
        // PUT: api/customers/xxxx-xxxx-xxxx-xxxx
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDataModel>> Put(Guid id, UpdateCustomerDataModel customer)
        {
            logger.LogInformation($"Update customer API: {id}:{JsonSerializer.Serialize(customer)}");
            using (var db = new CustomerDataAccess(customerDBContext, logger))
            {
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                return await db.UpdateAsync(id, customer);
            }
        }
        // DELETE: api/customer/xxxx-xxxx-xxxx-xxxx
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(Guid id)
        {

            using (var db = new CustomerDataAccess(customerDBContext, logger))
            {
                logger.LogInformation($"Delete customer API: {id}");
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                await db.DeleteAsync(id);
                return Ok();
            }
        }

        private bool CustomerExists(Guid id) => this.customerDBContext.Customers.Any(e => e.Id == id);

    }
}
