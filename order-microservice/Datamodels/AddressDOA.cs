using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using order_microservice.Kafka;
using order_microservice.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace order_microservice.Datamodels
{
    public class AddressDOA : IDisposable
    {
        private DBContext addressDBContext;
        private ILogger logger;
        IMessageProducer kafkaProducer;
        CancellationToken stoppingToken;

        public AddressDOA(DBContext context, ILogger<AddressDOA> _logger, IMessageProducer _producer, CancellationToken _stoppingToken)
        {
            addressDBContext = context;
            logger = _logger;
            kafkaProducer = _producer;
            stoppingToken = _stoppingToken;
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

                var customerMessage = new AddressMessage(new AddressKafkaMessage() { Action = ActionEnum.create, Address = privateAddress });
                var count = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    await kafkaProducer.ProduceAsync(null, new AddressKafkaMessage() { Action = ActionEnum.update, AddressID = privateAddress.Id, Address = privateAddress }, stoppingToken);
                    logger.LogInformation($"Address Kafka running at: {DateTimeOffset.Now} - {count}");
                    await Task.Delay(1000, stoppingToken);
                    count++;
                }
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
                    await kafkaProducer.ProduceAsync(null, new AddressKafkaMessage() { Action = ActionEnum.update, AddressID = id, Address = await addressDBContext.Address.FirstOrDefaultAsync(x => x.Id == id) }, stoppingToken);
                    return await this.addressDBContext.Address.FindAsync(id);
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
                await kafkaProducer.ProduceAsync(null, new AddressKafkaMessage() { Action = ActionEnum.update, AddressID = id }, stoppingToken);
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

        private bool AddressExists(Guid id) => this.addressDBContext.Address.Any(e => e.Id == id);

        private async Task SubmitKafkaMessageAsync(AddressMessage addressMessage)
        {
            int count = 0;
            string kafkaResult = "";
            while (!stoppingToken.IsCancellationRequested)
            {
                count++;
                try
                {
                    logger.LogInformation($"Address Kafka message submit at: {DateTimeOffset.Now} - {count}");
                    kafkaResult = await kafkaProducer.ProduceAsync(null, addressMessage, stoppingToken);
                    logger.LogInformation($"Address Kafka message submited at: {DateTimeOffset.Now} - {kafkaResult}");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Address Kafka message failed: {DateTimeOffset.Now} - {ex.Message} - {kafkaResult}");
                    await Task.Delay(1000, stoppingToken);
                }
                if (count > 100)
                {
                    throw new KafkaMessageException($"Address Kafka message failed after 100 tries - last error {kafkaResult}");
                }
            }
        }
    }
}
