﻿using Order.Domain.AggregateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        public Orders addOder(Orders order);
    }
}
