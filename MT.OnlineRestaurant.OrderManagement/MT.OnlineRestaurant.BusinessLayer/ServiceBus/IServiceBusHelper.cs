﻿using MT.OnlineRestaurant.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.OnlineRestaurant.BusinessLayer.ServiceBus
{
    public interface IServiceBusHelper
    {
        CartItemsEntity RecieveDataFromServiceBus();
    }
}
