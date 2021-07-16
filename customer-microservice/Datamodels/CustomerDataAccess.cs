using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using customer_microservice.Controllers;
using customer_microservice.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace customer_microservice.Datamodels
{
    public class CustomerDataAccess : IDisposable
    {
        private CustomerDBContext customerDBContext;
        private ILogger<CustomerController> logger;

        public CustomerDataAccess(CustomerDBContext context, ILogger<CustomerController> _logger)
        {
            customerDBContext = context;
            logger = _logger;
        }
        public async Task<ActionResult<CustomerDataModel>> CreateAsync(CreateCustomerDataModel customer)
        {
            try
            {
                CustomerDataModel privateCustomer = new CustomerDataModel();
                PropertyCopier<CreateCustomerDataModel, CustomerDataModel>.Copy(customer, privateCustomer);
                privateCustomer.Id = Guid.NewGuid();
                await customerDBContext.Customers.AddAsync(privateCustomer);
                await customerDBContext.SaveChangesAsync();
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
                return await customerDBContext.Customers.ToListAsync();
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
                return await customerDBContext.Customers.FindAsync(id);
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
