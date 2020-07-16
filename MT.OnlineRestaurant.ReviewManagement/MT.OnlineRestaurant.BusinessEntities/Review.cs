using System;
using System.Collections.Generic;
using System.Text;

namespace MT.OnlineRestaurant.BusinessEntities
{
    public class Review
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public int Rating { get; set; }
        public string UserComments { get; set; }
        public int CustomerId { get; set; }
    }
}
