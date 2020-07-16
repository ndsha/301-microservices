using MT.OnlineRestaurant.DataLayer.EntityFrameworkModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.OnlineRestaurant.DataLayer.Repository
{
    public interface IReviewRepository
    {
        Task<IList<TblRating>> GetRestaurantReview(int restaurantID, int customerId);
        Task RestaurantReview(TblRating tblRating);
        Task<bool> UpdateRestaurantReview(TblRating tblRating);
    }
}
