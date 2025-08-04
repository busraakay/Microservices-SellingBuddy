using OrderService.Domain.AggregateModels.OrderAggregateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
    }
}
