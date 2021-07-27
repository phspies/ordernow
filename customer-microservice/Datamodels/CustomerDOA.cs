using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using Confluent.Kafka;
using customer_microservice.Controllers;
using customer_microservice.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json;
using static customer_microservice.Startup;

namespace customer_microservice.Datamodels
{
    public class CustomerDOA : IDisposable
    {
        private DBContext customerDBContext;
        private ILogger logger;
        IProducer<Null, string> kafkaProducer;

        public CustomerDOA(DBContext context, ILogger<CustomerDOA> _logger,  IProducer<Null, string> _producer)
        {
            customerDBContext = context;
            logger = _logger;
            kafkaProducer = _producer;

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
                await kafkaProducer.ProduceAsync("ordernow-customer-events", new Message<Null,  string> { Value = JsonConvert.SerializeObject(new CustomerKafkaMessage() { Action = ActionEnum.create,CustomerID= privateCustomer.Id, Customer = privateCustomer }) });
                await kafkaProducer.ProduceAsync("ordernow-address-events", new Message<Null, string> { Value = JsonConvert.SerializeObject(new AddressKafkaMessage() { Action = ActionEnum.create, AddressID = privateAddress.Id, Address = privateAddress }) });

                kafkaProducer.Flush();
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
                    await kafkaProducer.ProduceAsync("ordernow-customer-events", new Message<Null, string> { Value = JsonConvert.SerializeObject(new CustomerKafkaMessage() { Action = ActionEnum.update, CustomerID = id, Customer = await customerDBContext.Customers.FirstOrDefaultAsync(x => x.Id == id) }) });
                    kafkaProducer.Flush();
                    return this.customerDBContext.Customers.Find(id);
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
                await kafkaProducer.ProduceAsync("ordernow-customer-events", new Message<Null, String> { Value = JsonConvert.SerializeObject(new CustomerKafkaMessage() { Action = ActionEnum.delete, CustomerID = id }) });
                kafkaProducer.Flush();
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
