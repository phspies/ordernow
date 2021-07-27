using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using order_microservice.Controllers;
using order_microservice.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace order_microservice.Datamodels
{
    public class OrderDataAccess : IDisposable
    {
        private OrderDBContext orderDBContext;
        private ILogger<OrderController> logger;

        public OrderDataAccess(OrderDBContext context, ILogger<OrderController> _logger)
        {
            orderDBContext = context;
            logger = _logger;
        }
        public async Task<ActionResult<OrderDataModel>> CreateAsync(CreateOrderDataModel order)
        {
            try
            {
                OrderDataModel privateOrder = new OrderDataModel();
                PropertyCopier<CreateOrderDataModel, OrderDataModel>.Copy(order, privateOrder);
                privateOrder.Id = Guid.NewGuid();
                await orderDBContext.Orders.AddAsync(privateOrder);
                await orderDBContext.SaveChangesAsync();
                return privateOrder;
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
        public async Task<List<OrderDataModel>> GetListAsync()
        {
            try
            {
                return await orderDBContext.Orders.ToListAsync();
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
        public async Task<OrderDataModel> GetAsync(Guid id)
        {
            try
            {
                return await orderDBContext.Orders.FindAsync(id);
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
        public async Task<ActionResult<OrderDataModel>> UpdateAsync(Guid id, UpdateOrderDataModel order)
        {
            try
            {
                using (var transaction = orderDBContext.Database.BeginTransaction())
                {
                    orderDBContext.Entry(await orderDBContext.Orders.FirstOrDefaultAsync(x => x.Id == id)).CurrentValues.SetValues(order);
                    await orderDBContext.SaveChangesAsync();
                    transaction.Commit();
                    return this.orderDBContext.Orders.Find(id);
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
                var orderItem = await orderDBContext.Orders.FindAsync(id);
                orderDBContext.Orders.Remove(orderItem);
                return await orderDBContext.SaveChangesAsync();
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
            ((IDisposable)orderDBContext).Dispose();
        }

        private OrderDataModel CopyPublicToPrivateOrder(Object order)
        {
            OrderDataModel privateOrderObject = new OrderDataModel();
            foreach (PropertyInfo property in order.GetType().GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(privateOrderObject, property.GetValue(order, null), null);
            }
            return privateOrderObject;
        }
        public bool OrderExists(Guid id) => this.orderDBContext.Orders.Any(e => e.Id == id);

    }
}
