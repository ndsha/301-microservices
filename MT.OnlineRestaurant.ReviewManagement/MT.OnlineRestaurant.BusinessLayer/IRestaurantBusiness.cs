using MT.OnlineRestaurant.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.OnlineRestaurant.BusinessLayer
{
    public interface IRestaurantBusiness
    {
        Task RestaurantReview(Review restaurantRating);
        Task<IQueryable<Review>> GetRestaurantReview(int restaurantID, int customerId);
        Task<bool> UpdateRestaurantReview(Review restaurantRating);
    }
}
