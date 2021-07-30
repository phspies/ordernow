using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using customer_microservice.Datamodels;
using customer_microservice.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace address_microservice.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private DBContext addressDBContext;
        private ILogger<AddressController> AddressControllerlogger;
        private ILogger<AddressDOA> AddressDOAlogger;
        IMessageProducer kafkaProducer;
        HttpRequest httpRequest; 


        public AddressController(DBContext context, ILogger<AddressDOA> _doalogger, ILogger<AddressController> _controllerlogger, IMessageProducer _producer)
        {
            addressDBContext = context;
            AddressDOAlogger = _doalogger;
            AddressControllerlogger = _controllerlogger;
            kafkaProducer = _producer;
            httpRequest = HttpContext.Request;
        }

        // GET: api/addresss
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDataModel>>> List(CancellationToken stoppingToken)
        {
            using (var db = new AddressDOA(addressDBContext, AddressDOAlogger, kafkaProducer, stoppingToken))
            {
                AddressControllerlogger.LogInformation("Retrieving all addresss API");
                return await db.GetListAsync();
            }
        }
        // GET: api/addresss/xxxx-xxxx-xxxx-xxxx
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDataModel>> Get(Guid id, CancellationToken stoppingToken)
        {
            using (var db = new AddressDOA(addressDBContext, AddressDOAlogger, kafkaProducer, stoppingToken))
            {
                AddressControllerlogger.LogInformation($"Retrieving address API: {id}");
                return await db.GetAsync(id);
            }
        }
        // POST: api/addresss
        [HttpPost]
        public async Task<ActionResult<AddressDataModel>> Create(CreateAddressDataModel address, CancellationToken stoppingToken)
        {
            using (var db = new AddressDOA(addressDBContext, AddressDOAlogger, kafkaProducer, stoppingToken))
            {
                var newAddress = await db.CreateAsync(address);
                AddressControllerlogger.LogInformation($"Creating address API: {JsonSerializer.Serialize(newAddress)}");
                return newAddress;
            }
        }
        // PUT: api/addresss/xxxx-xxxx-xxxx-xxxx
        [HttpPut("{id}")]
        public async Task<ActionResult<AddressDataModel>> Update(Guid id, UpdateAddressDataModel address, CancellationToken stoppingToken)
        {
            AddressControllerlogger.LogInformation($"Update address API: {id}:{JsonSerializer.Serialize(address)}");
            using (var db = new AddressDOA(addressDBContext, AddressDOAlogger, kafkaProducer, stoppingToken))
            {
                return id == Guid.Empty ? BadRequest() : !await AddressExistsAsync(id) ? NotFound() : await db.UpdateAsync(id, address);
            }
        }
        // DELETE: api/address/xxxx-xxxx-xxxx-xxxx
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, CancellationToken stoppingToken)
        {

            using (var db = new AddressDOA(addressDBContext, AddressDOAlogger, kafkaProducer, stoppingToken))
            {
                AddressControllerlogger.LogInformation($"Delete address API: {id}");
                if (id == Guid.Empty)
                {
                    return BadRequest();
                }
                if (!await AddressExistsAsync(id))
                {
                    return NotFound();
                }
                await db.DeleteAsync(id);
                return Ok();
            }
        }

        private async Task<bool> AddressExistsAsync(Guid id) => await addressDBContext.Address.AnyAsync(e => e.Id == id);

    }
}
