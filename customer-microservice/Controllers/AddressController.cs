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
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private DBContext customerDBContext;
        private ILogger<AddressController> AddressControllerlogger;
        private ILogger<AddressDOA> AddressDOAlogger;
        IProducer<Null, string> kafkaProducer;


        public AddressController(DBContext context, ILogger<AddressDOA> _doalogger, ILogger<AddressController> _controllerlogger, IProducer<Null, string> _producer)
        {
            customerDBContext = context;
            AddressDOAlogger = _doalogger;
            AddressControllerlogger = _controllerlogger;
            kafkaProducer = _producer;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDataModel>>> List()
        {
            using (var db = new AddressDOA(customerDBContext, AddressDOAlogger, kafkaProducer))
            {
                AddressControllerlogger.LogInformation("Retrieving all customers API");
                return await db.GetListAsync();
            }
        }
        // GET: api/customers/xxxx-xxxx-xxxx-xxxx
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDataModel>> Get(Guid id)
        {
            using (var db = new AddressDOA(customerDBContext, AddressDOAlogger, kafkaProducer))
            {
                AddressControllerlogger.LogInformation($"Retrieving customer API: {id}");
                return await db.GetAsync(id);
            }
        }
        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<AddressDataModel>> Create(CreateAddressDataModel address)
        {
            using (var db = new AddressDOA(customerDBContext, AddressDOAlogger, kafkaProducer))
            {
                var newAddress = await db.CreateAsync(address);
                AddressControllerlogger.LogInformation($"Creating customer API: {JsonSerializer.Serialize(newAddress)}");
                return newAddress;
            }
        }
        // PUT: api/customers/xxxx-xxxx-xxxx-xxxx
        [HttpPut("{id}")]
        public async Task<ActionResult<AddressDataModel>> Update(Guid id, UpdateAddressDataModel customer)
        {
            AddressControllerlogger.LogInformation($"Update customer API: {id}:{JsonSerializer.Serialize(customer)}");
            using (var db = new AddressDOA(customerDBContext, AddressDOAlogger, kafkaProducer))
            {
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }
                if (!AddressExists(id))
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

            using (var db = new AddressDOA(customerDBContext, AddressDOAlogger, kafkaProducer))
            {
                AddressControllerlogger.LogInformation($"Delete customer API: {id}");
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }
                if (!AddressExists(id))
                {
                    return NotFound();
                }
                await db.DeleteAsync(id);
                return Ok();
            }
        }

        private bool AddressExists(Guid id) => this.customerDBContext.Customers.Any(e => e.Id == id);

    }
}
