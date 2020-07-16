using Microsoft.EntityFrameworkCore;
using MT.OnlineRestaurant.DataLayer.EntityFrameworkModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MT.OnlineRestaurant.DataLayer.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly RestaurantManagementContext db;
        public ReviewRepository(RestaurantManagementContext connection)
        {
            db = connection;
        }
        public async Task<IList<TblRating>> GetRestaurantReview(int restaurantID, int customerId)
        {
            try
            {
                if (db != null)
                {
                    IList<TblRating> ratings;
                    if (customerId == 0)
                    {
                        ratings = await (from rating in db.TblRating
                                             where rating.RestaurantId == restaurantID
                                             orderby rating.Rating descending
                                             select rating).ToListAsync();
                    }
                    else
                    {
                        ratings = await (from rating in db.TblRating
                                         where rating.RestaurantId == restaurantID
                                         && rating.CustomerId == customerId
                                         orderby rating.Rating descending
                                         select rating).ToListAsync();
                    }
                    return ratings;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task RestaurantReview(TblRating tblRating)
        {
            try
            {
                tblRating.RecordTimeStampCreated = DateTime.Now;

                await db.Set<TblRating>().AddAsync(tblRating);
                await db.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateRestaurantReview(TblRating tblRating)
        {
            try
            {
                var ratingFromDb = await (from rating in db.TblRating
                                      where rating.Id == tblRating.Id
                                      select rating).FirstOrDefaultAsync();
                if(ratingFromDb == null)
                {
                    return false;
                }
                ratingFromDb.Rating = tblRating.Rating;
                ratingFromDb.Comments = tblRating.Comments;
                ratingFromDb.RecordTimeStamp = DateTime.Now;
                db.Set<TblRating>().Update(ratingFromDb);
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
