using MT.OnlineRestaurant.BusinessEntities;
using MT.OnlineRestaurant.DataLayer.EntityFrameworkModel;
using MT.OnlineRestaurant.DataLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.OnlineRestaurant.BusinessLayer
{
    public class RestaurantBusiness : IRestaurantBusiness
    {
        private readonly IReviewRepository _reviewRepository;
        public RestaurantBusiness(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }
        public async Task RestaurantReview(Review restaurantRating)
        {
            try
            {
                if (restaurantRating != null)
                {
                    TblRating rating = new TblRating()
                    {
                        Rating = restaurantRating.Rating.ToString(),
                        RestaurantId = restaurantRating.RestaurantId,
                        Comments = restaurantRating.UserComments,
                        CustomerId = restaurantRating.CustomerId
                    };

                    await _reviewRepository.RestaurantReview(rating);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<IQueryable<Review>> GetRestaurantReview(int restaurantID, int customerId)
        {
            try
            {
                var reviews = await _reviewRepository.GetRestaurantReview(restaurantID, customerId);

                List<Review> restaurantReviews = new List<Review>();
                foreach (var item in reviews)
                {
                    Review review = new Review
                    {
                        Id = item.Id,
                        Rating = Convert.ToInt32(item.Rating),
                        RestaurantId = item.RestaurantId,
                        UserComments = item.Comments,
                        CustomerId = item.CustomerId,
                    };
                    restaurantReviews.Add(review);
                }
                return restaurantReviews.AsQueryable();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateRestaurantReview(Review restaurantRating)
        {
            try
            {
                if (restaurantRating != null)
                {
                    TblRating rating = new TblRating()
                    {
                        Id = restaurantRating.Id,
                        Rating = restaurantRating.Rating.ToString(),
                        Comments = restaurantRating.UserComments,
                    };

                    return await _reviewRepository.UpdateRestaurantReview(rating);
                }
                return false;
            }
            catch
            {
                throw;
            }
        }
    }
}
