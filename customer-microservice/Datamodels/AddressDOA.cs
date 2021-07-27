using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using customer_microservice.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace customer_microservice.Datamodels
{
    public class AddressDOA : IDisposable
    {
        private DBContext addressDBContext;
        private ILogger logger;
        IProducer<Null, string> kafkaProducer;

        public AddressDOA(DBContext context, ILogger<AddressDOA> _logger,  IProducer<Null, string> _producer)
        {
            addressDBContext = context;
            logger = _logger;
            kafkaProducer = _producer;

        }
        public async Task<ActionResult<AddressDataModel>> CreateAsync(CreateAddressDataModel address)
        {
            try
            {
                AddressDataModel privateAddress = new AddressDataModel();
                PropertyCopier<CreateAddressDataModel, AddressDataModel>.Copy(address, privateAddress);
                privateAddress.Id = Guid.NewGuid();
                await addressDBContext.Address.AddAsync(privateAddress);
                await addressDBContext.SaveChangesAsync();
                await kafkaProducer.ProduceAsync("ordernow-address-events",
                    new Message<Null, string> {
                        Value = JsonConvert.SerializeObject(
                            new AddressKafkaMessage() {
                                Action = ActionEnum.create,
                                AddressID = privateAddress.Id,
                                Address = privateAddress
                            }
                            )
                    });
                return privateAddress;
            }
            catch (DbUpdateException mysqlex)
            {
                logger.LogError(mysqlex.InnerException.Message);
                throw new InvalidOperationException(mysqlex.InnerException.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<List<AddressDataModel>> GetListAsync()
        {
            try
            {
                return await addressDBContext.Address.ToListAsync();
            }
            catch (DbUpdateException mysqlex)
            {
                logger.LogError(mysqlex.InnerException.Message);
                throw new InvalidOperationException(mysqlex.InnerException.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<AddressDataModel> GetAsync(Guid id)
        {
            try
            {
                return await addressDBContext.Address.FindAsync(id);
            }
            catch (DbUpdateException mysqlex)
            {
                logger.LogError(mysqlex.InnerException.Message);
                throw new InvalidOperationException(mysqlex.InnerException.Message);
            }
            catch (Exception ex)
            { 
                logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<ActionResult<AddressDataModel>> UpdateAsync(Guid id, UpdateAddressDataModel address)
        {
            try
            {
                using (var transaction = addressDBContext.Database.BeginTransaction())
                {
                    addressDBContext.Entry(await addressDBContext.Address.FirstOrDefaultAsync(x => x.Id == id)).CurrentValues.SetValues(address);
                    await addressDBContext.SaveChangesAsync();
                    transaction.Commit();
                    await kafkaProducer.ProduceAsync("ordernow-address-events", new Message<Null, string> { Value = JsonConvert.SerializeObject(new AddressKafkaMessage() { Action = ActionEnum.update, AddressID = id, Address = await addressDBContext.Address.FirstOrDefaultAsync(x => x.Id == id) }) });
                    return this.addressDBContext.Address.Find(id);
                }

            }
            catch (DbUpdateException mysqlex)
            {
                logger.LogError(mysqlex.InnerException.Message);
                throw new InvalidOperationException(mysqlex.InnerException.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var addressItem = await addressDBContext.Address.FindAsync(id);
                await kafkaProducer.ProduceAsync("ordernow-address-events", new Message<Null, String> { Value = JsonConvert.SerializeObject(new AddressKafkaMessage() { Action = ActionEnum.delete, AddressID = id }) });
                addressDBContext.Address.Remove(addressItem);
                return await addressDBContext.SaveChangesAsync();
            }
            catch (DbUpdateException mysqlex)
            {
                logger.LogError(mysqlex.InnerException.Message);
                throw new InvalidOperationException(mysqlex.InnerException.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
        }


        public void Dispose()
        {
            ((IDisposable)addressDBContext).Dispose();
        }

        private AddressDataModel CopyPublicToPrivateAddress(Object address)
        {
            AddressDataModel privateAddressObject = new AddressDataModel();
            foreach (PropertyInfo property in address.GetType().GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(privateAddressObject, property.GetValue(address, null), null);
            }
            return privateAddressObject;
        }
        public bool AddressExists(Guid id) => this.addressDBContext.Address.Any(e => e.Id == id);

    }
}
