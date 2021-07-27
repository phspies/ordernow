using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using customer_microservice.Datamodels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace customer_microservice.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private DBContext customerDBContext;
        private ILogger<CustomerController> CustomerControllerlogger;
        private ILogger<CustomerDOA> CustomerDOAlogger;
        IProducer<Null, string> kafkaProducer;


        public CustomerController(DBContext context, ILogger<CustomerDOA> _doalogger, ILogger<CustomerController> _controllerlogger, IProducer<Null, string> _producer)
        {
            customerDBContext = context;
            CustomerDOAlogger = _doalogger;
            CustomerControllerlogger = _controllerlogger;
            kafkaProducer = _producer;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDataModel>>> List()
        {
            using (var db = new CustomerDOA(customerDBContext, CustomerDOAlogger, kafkaProducer))
            {
                CustomerControllerlogger.LogInformation("Retrieving all customers API");
                return await db.GetListAsync();
            }
        }
        // GET: api/customers/xxxx-xxxx-xxxx-xxxx
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDataModel>> Get(Guid id)
        {
            using (var db = new CustomerDOA(customerDBContext, CustomerDOAlogger, kafkaProducer))
            {
                CustomerControllerlogger.LogInformation($"Retrieving customer API: {id}");
                return await db.GetAsync(id);
            }
        }
        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDataModel>> Create(CreateCustomerDataModel customer)
        {
            using (var db = new CustomerDOA(customerDBContext, CustomerDOAlogger, kafkaProducer))
            {
                CustomerControllerlogger.LogInformation($"Creating customer API: {JsonSerializer.Serialize(customer)}");
                return await db.CreateAsync(customer);
            }
        }
        // PUT: api/customers/xxxx-xxxx-xxxx-xxxx
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDataModel>> Update(Guid id, UpdateCustomerDataModel customer)
        {
            CustomerControllerlogger.LogInformation($"Update customer API: {id}:{JsonSerializer.Serialize(customer)}");
            using (var db = new CustomerDOA(customerDBContext, CustomerDOAlogger, kafkaProducer))
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
        public async Task<ActionResult> Delete(Guid id)
        {

            using (var db = new CustomerDOA(customerDBContext, CustomerDOAlogger, kafkaProducer))
            {
                CustomerControllerlogger.LogInformation($"Delete customer API: {id}");
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
