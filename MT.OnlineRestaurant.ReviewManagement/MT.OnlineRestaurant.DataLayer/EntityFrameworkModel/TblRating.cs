using System;

namespace MT.OnlineRestaurant.DataLayer.EntityFrameworkModel
{
    public partial class TblRating
    {
        public string Rating { get; set; }
        public string Comments { get; set; }
        public int RestaurantId { get; set; }
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime RecordTimeStamp { get; set; }
        public DateTime RecordTimeStampCreated { get; set; }
    }
}
