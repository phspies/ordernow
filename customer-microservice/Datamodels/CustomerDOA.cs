using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using customer_microservice.Kafka;
using customer_microservice.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace customer_microservice.Datamodels
{
    public class CustomerDOA : IDisposable
    {
        private DBContext customerDBContext;
        private ILogger logger;
        IMessageProducer kafkaProducer;
        CancellationToken stoppingToken;

        public CustomerDOA(DBContext context, ILogger<CustomerDOA> _logger, IMessageProducer _producer, CancellationToken _stoppingToken)
        {
            customerDBContext = context;
            logger = _logger;
            kafkaProducer = _producer;
            stoppingToken = _stoppingToken;

        }
        public async Task<ActionResult<CustomerDataModel>> CreateAsync(CreateCustomerDataModel customer)
        {
            try
            {
                CustomerDataModel privateCustomer = new CustomerDataModel();
                AddressDataModel privateAddress = new AddressDataModel();
                PropertyCopier<CreateCustomerDataModel, CustomerDataModel>.Copy(customer, privateCustomer);
                PropertyCopier<CreateAddressDataModel, AddressDataModel>.Copy(customer.Address, privateAddress);

                privateCustomer.Id = Guid.NewGuid();
                privateAddress.Id = Guid.NewGuid();
                await customerDBContext.Address.AddAsync(privateAddress);
                privateCustomer.Address = privateAddress;
                await customerDBContext.Customers.AddAsync(privateCustomer);
                
                await customerDBContext.SaveChangesAsync();

                var customerMessage = new CustomerMessage(new CustomerKafkaMessage() { Action = ActionEnum.create, CustomerID = privateCustomer.Id, Customer = privateCustomer });
                var addressMessage = new AddressMessage(new AddressKafkaMessage() { Action = ActionEnum.create, AddressID = privateAddress.Id, Address = privateAddress });
                var count = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    await kafkaProducer.ProduceAsync(null, customerMessage, stoppingToken);
                    logger.LogInformation($"Customer Kafka running at: {DateTimeOffset.Now} - {count}");
                    await Task.Delay(1000, stoppingToken);
                    count++;
                }
                count = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    await kafkaProducer.ProduceAsync(null, addressMessage, stoppingToken);
                    logger.LogInformation($"Customer Kafka running at: {DateTimeOffset.Now} - {count}");
                    await Task.Delay(1000, stoppingToken);
                    count++;
                }
                return privateCustomer;
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
        public async Task<List<CustomerDataModel>> GetListAsync()
        {
            try
            {
                return await customerDBContext.Customers.Include(x => x.Address).ToListAsync();
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
        public async Task<CustomerDataModel> GetAsync(Guid id)
        {
            try
            {
                return await customerDBContext.Customers.Include(x => x.Address).FirstOrDefaultAsync(x => x.Id == id);
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
        public async Task<ActionResult<CustomerDataModel>> UpdateAsync(Guid id, UpdateCustomerDataModel customer)
        {
            try
            {
                using (var transaction = customerDBContext.Database.BeginTransaction())
                {
                    customerDBContext.Entry(await customerDBContext.Customers.FirstOrDefaultAsync(x => x.Id == id)).CurrentValues.SetValues(customer);
                    await customerDBContext.SaveChangesAsync();
                    transaction.Commit();
                    var updatedCustomer = this.customerDBContext.Customers.Find(id);
                    var customerMessage = new CustomerMessage(new CustomerKafkaMessage() { Action = ActionEnum.update, CustomerID = id, Customer = updatedCustomer });
                    var count = 0;
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await kafkaProducer.ProduceAsync(null, customerMessage, stoppingToken);
                        logger.LogInformation($"Customer Kafka running at: {DateTimeOffset.Now} - {count}");
                        await Task.Delay(1000, stoppingToken);
                        count++;
                    }
                    return updatedCustomer;
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
                var customerItem = await customerDBContext.Customers.FindAsync(id);
                customerDBContext.Customers.Remove(customerItem);
                var customerMessage = new CustomerMessage(new CustomerKafkaMessage() { Action = ActionEnum.delete, CustomerID = id });
                var count = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    await kafkaProducer.ProduceAsync(null, customerMessage, stoppingToken);
                    logger.LogInformation($"Customer Kafka running at: {DateTimeOffset.Now} - {count}");
                    await Task.Delay(1000, stoppingToken);
                    count++;
                }
                return await customerDBContext.SaveChangesAsync();
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
            ((IDisposable)customerDBContext).Dispose();
        }

        private CustomerDataModel CopyPublicToPrivateCustomer(Object customer)
        {
            CustomerDataModel privateCustomerObject = new CustomerDataModel();
            foreach (PropertyInfo property in customer.GetType().GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(privateCustomerObject, property.GetValue(customer, null), null);
            }
            return privateCustomerObject;
        }
        public bool CustomerExists(Guid id) => this.customerDBContext.Customers.Any(e => e.Id == id);

    }
}
