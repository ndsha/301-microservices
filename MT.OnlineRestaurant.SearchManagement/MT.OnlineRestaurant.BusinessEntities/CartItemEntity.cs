using System;
using System.Collections.Generic;
using System.Text;

namespace MT.OnlineRestaurant.BusinessEntities
{
    public class CartItemsEntity
    {
        public int RestaurantId { get; set; }
        public int ItemId { get; set; }
        public bool IsItemAvailable { get; set; }
        public bool IsItemPriceChanged { get; set; }
        public decimal ItemOldPrice { get; set; }
        public decimal ItemNewPrice { get; set; }
        public string Message { get; set; }
    }
}
