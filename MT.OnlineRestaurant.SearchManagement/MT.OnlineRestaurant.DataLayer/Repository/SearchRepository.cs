using System;
using System.Collections.Generic;
using System.Text;
using MT.OnlineRestaurant.DataLayer.EntityFrameWorkModel;
using System.Linq;
using MT.OnlineRestaurant.DataLayer.DataEntity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace MT.OnlineRestaurant.DataLayer.Repository
{
    public class SearchRepository : ISearchRepository
    {
        private readonly RestaurantManagementContext db;
        public SearchRepository(RestaurantManagementContext connection)
        {
            db = connection;
        }

        #region Interface Methods
        public IQueryable<MenuDetails> GetRestaurantMenu(int restaurantID)
        {
            List<MenuDetails> menudetails = new List<MenuDetails>();
            try
            {
                if (db != null)
                {
                    var menudetail = (from offer in db.TblOffer
                                      join menu in db.TblMenu
                                      on offer.TblMenuId equals menu.Id into TableMenu
                                      from menu in TableMenu.ToList()
                                      join cuisine in db.TblCuisine on menu.TblCuisineId equals cuisine.Id
                                      where offer.TblRestaurantId == restaurantID
                                      select new MenuDetails
                                      {
                                          tbl_Offer = offer,
                                          tbl_Cuisine = cuisine,
                                          tbl_Menu = menu

                                      }).ToList();
                    foreach (var item in menudetail)
                    {
                        MenuDetails menuitem = new MenuDetails
                        {
                            tbl_Cuisine = item.tbl_Cuisine,
                            tbl_Menu = item.tbl_Menu,
                            tbl_Offer = item.tbl_Offer
                        };
                        menudetails.Add(menuitem);
                    }
                }
                return menudetails.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<TblRating> GetRestaurantRating(int restaurantID)
        {
            // List<TblRating> restaurant_Rating = new List<TblRating>();
            try
            {
                if (db != null)
                {
                    return (from rating in db.TblRating
                            join restaurant in db.TblRestaurant on
                            rating.TblRestaurantId equals restaurant.Id
                            where rating.TblRestaurantId == restaurantID
                            orderby rating.Rating descending
                            select new TblRating
                            {
                                Rating = rating.Rating,
                                Comments = rating.Comments,
                                TblRestaurant = restaurant,
                                TblRestaurantId = rating.TblRestaurantId,
                                TblCustomerId = rating.TblCustomerId
                            }).AsQueryable();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TblRestaurant GetResturantDetails(int restaurantID)
        {
            TblRestaurant resturantInformation = new TblRestaurant();

            try
            {
                if (db != null)
                {
                    resturantInformation = (from restaurant in db.TblRestaurant
                                            join location in db.TblLocation on restaurant.TblLocationId equals location.Id
                                            where restaurant.Id == restaurantID
                                            select new TblRestaurant
                                            {
                                                Id = restaurant.Id,
                                                Name = restaurant.Name,
                                                Address = restaurant.Address,
                                                ContactNo = restaurant.ContactNo,
                                                TblLocation = location,
                                                CloseTime = restaurant.CloseTime,
                                                OpeningTime = restaurant.OpeningTime,
                                                Website = restaurant.Website,
                                            }).FirstOrDefault();

                }

                return resturantInformation;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public IQueryable<TblRestaurantDetails> GetTableDetails(int restaurantID)
        {
            try
            {
                if (db != null)
                {
                    return (from restaurantDetails in db.TblRestaurantDetails
                            join restaurant in db.TblRestaurant
                            on restaurantDetails.TblRestaurantId equals restaurant.Id
                            where restaurantDetails.TblRestaurantId == restaurantID
                            select new TblRestaurantDetails
                            {
                                TableCapacity = restaurantDetails.TableCapacity,
                                TableCount = restaurantDetails.TableCount,
                                TblRestaurant = restaurant
                            }).AsQueryable();

                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<RestaurantSearchDetails> GetRestaurantsBasedOnLocation(LocationDetails location_Details)
        {
            List<RestaurantSearchDetails> restaurants = new List<RestaurantSearchDetails>();
            try
            {
                restaurants = GetRetaurantBasedOnLocationAndName(location_Details);
                return restaurants.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IQueryable<RestaurantSearchDetails> GetRestaurantsBasedOnMenu(AddtitionalFeatureForSearch searchDetails)
        {
            List<RestaurantSearchDetails> restaurants = new List<RestaurantSearchDetails>();
            try
            {
                restaurants = GetRestaurantDetailsBasedOnRating(searchDetails);
                return restaurants.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //public IQueryable<RestaurantSearchDetails> SearchForRestaurant(SearchForRestautrant searchDetails)
        //{
        //    List<RestaurantSearchDetails> searchedRestaurantBasedOnRating = new List<RestaurantSearchDetails>();
        //    searchedRestaurantBasedOnRating = GetRestaurantDetailsBasedOnRating(searchDetails.search).ToList();

        //    List<RestaurantSearchDetails> restaurantsBasedOnLocation = new List<RestaurantSearchDetails>();
        //    restaurantsBasedOnLocation = GetRetaurantBasedOnLocationAndName(searchDetails.location).ToList();

        //    List<RestaurantSearchDetails> restaurantInfo = new List<RestaurantSearchDetails>();
        //    restaurantInfo = restaurantsBasedOnLocation.Intersect(searchedRestaurantBasedOnRating, new RestaurantComparer()).ToList();

        //    return restaurantInfo.AsQueryable();
        //}

        public IQueryable<RestaurantSearchDetails> SearchForRestaurant(SearchForRestautrant searchDetails)
        {
            List<RestaurantSearchDetails> restaurants = new List<RestaurantSearchDetails>();
            try
            {
                var restaurantFilter = (from restaurant in db.TblRestaurant
                                        join location in db.TblLocation on restaurant.TblLocationId equals location.Id
                                        join rating in db.TblRating on restaurant.Id equals rating.TblRestaurantId
                                        select new { TblRestaurant = restaurant, TblLocation = location, TblRating = rating });

                if (!string.IsNullOrEmpty(searchDetails.search.cuisine))
                {
                    restaurantFilter = (from filteredRestaurant in restaurantFilter
                                        join offer in db.TblOffer on filteredRestaurant.TblRestaurant.Id equals offer.TblRestaurantId
                                        join menu in db.TblMenu on offer.TblMenuId equals menu.Id
                                        join cuisine in db.TblCuisine on menu.TblCuisineId equals cuisine.Id
                                        where cuisine.Cuisine.Contains(searchDetails.search.cuisine)
                                        select filteredRestaurant).Distinct();
                }
                if (!string.IsNullOrEmpty(searchDetails.search.Menu))
                {
                    restaurantFilter = (from filteredRestaurant in restaurantFilter
                                        join offer in db.TblOffer on filteredRestaurant.TblRestaurant.Id equals offer.TblRestaurantId
                                        join menu in db.TblMenu on offer.TblMenuId equals menu.Id
                                        where menu.Item.Contains(searchDetails.search.Menu)
                                        select filteredRestaurant).Distinct();
                }

                if (searchDetails.search.rating > 0)
                {
                    restaurantFilter = (from filteredRestaurant in restaurantFilter
                                        where filteredRestaurant.TblRating.Rating.Contains(searchDetails.search.rating.ToString())
                                        select filteredRestaurant).Distinct();
                }

                if (!string.IsNullOrEmpty(searchDetails.location.restaurant_Name))
                {
                    restaurantFilter = restaurantFilter.Where(a => a.TblRestaurant.Name.Contains(searchDetails.location.restaurant_Name));
                }

                if (!(double.IsNaN(searchDetails.location.xaxis)) && !(double.IsNaN(searchDetails.location.yaxis)))
                {
                    var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
                    Parallel.ForEach(restaurantFilter, parallelOptions, restaurant =>
                    {
                        double distance = Distance(searchDetails.location.xaxis, searchDetails.location.yaxis, Convert.ToDouble(restaurant.TblLocation.X), Convert.ToDouble(restaurant.TblLocation.Y));
                        if ((int.Parse(searchDetails.location.distance.ToString()) == 0 || distance < int.Parse(searchDetails.location.distance.ToString()))
                            && (searchDetails.location.xaxis == 0 || searchDetails.location.xaxis == Convert.ToDouble(restaurant.TblLocation.X))
                            && (searchDetails.location.yaxis == 0 || searchDetails.location.yaxis == Convert.ToDouble(restaurant.TblLocation.Y)))
                        {
                            RestaurantSearchDetails tblRestaurant = new RestaurantSearchDetails
                            {
                                restauran_ID = restaurant.TblRestaurant.Id,
                                restaurant_Name = restaurant.TblRestaurant.Name,
                                restaurant_Address = restaurant.TblRestaurant.Address,
                                restaurant_PhoneNumber = restaurant.TblRestaurant.ContactNo,
                                restraurant_Website = restaurant.TblRestaurant.Website,
                                closing_Time = restaurant.TblRestaurant.CloseTime,
                                opening_Time = restaurant.TblRestaurant.OpeningTime,
                                xaxis = Convert.ToDouble(restaurant.TblLocation.X),
                                yaxis = Convert.ToDouble(restaurant.TblLocation.Y),
                                rating = Convert.ToInt32(restaurant.TblRating.Rating)
                            };
                            restaurants.Add(tblRestaurant);
                        }
                    });
                }
                restaurants = restaurants.OrderByDescending(x => x.rating).ThenBy(x => x.restaurant_Name).ToList();
                return restaurants.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Recording the customer rating the restaurants
        /// </summary>
        /// <param name="tblRating"></param>
        public void RestaurantRating(TblRating tblRating)
        {
            //tblRating.UserCreated = ,
            //tblRating.UserModified=,
            tblRating.RecordTimeStampCreated = DateTime.Now;

            db.Set<TblRating>().Add(tblRating);
            db.SaveChanges();

        }
        public TblMenu ItemInStock(int restaurantID, int menuID)
        {
            try
            {
                TblMenu menuObj = new TblMenu();
                if (db != null)
                {
                    //    menuObj = (from m in db.TblMenu
                    //               join offer in db.TblOffer on m.Id equals offer.TblMenuId
                    //               join restaurant in db.TblRestaurantDetails on offer.TblRestaurantId equals restaurant.TblRestaurantId
                    //               where restaurant.TblRestaurantId == restaurantID && m.Id == menuID
                    //               select new TblMenu
                    //               {
                    //                   quantity = m.quantity
                    //               }).FirstOrDefault();                   
                    //}
                    menuObj = (from offer in db.TblOffer
                               join menu in db.TblMenu
                               on offer.TblMenuId equals menu.Id
                               join rest in db.TblRestaurantDetails
                               on offer.TblRestaurantId equals rest.TblRestaurantId
                               where rest.TblRestaurantId == restaurantID && menu.Id == menuID
                               select new TblMenu
                               {
                                   quantity = menu.quantity
                               }).FirstOrDefault();
                }
                return menuObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TblOffer ItemPriceChaned(int restaurantID, int menuID)
        {
            try
            {
                TblOffer offerObj = new TblOffer();
                if(db!= null)
                {
                    offerObj = (from offer in db.TblOffer
                                where offer.TblRestaurantId == restaurantID && offer.TblMenuId == menuID
                                select new TblOffer
                                {
                                    Price = offer.Price
                                }).FirstOrDefault();
                }
                return offerObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region private methods
        private List<RestaurantSearchDetails> GetRestaurantDetailsBasedOnRating(AddtitionalFeatureForSearch searchList)
        {
            List<RestaurantSearchDetails> restaurants = new List<RestaurantSearchDetails>();
            try
            {
                var restaurantFilter = (from restaurant in db.TblRestaurant
                                        join location in db.TblLocation on restaurant.TblLocationId equals location.Id
                                        select new { TblRestaurant = restaurant, TblLocation = location });

                //var restaurantFilter = (from restaurant in db.TblRestaurant
                //                        join location in db.TblLocation on restaurant.TblLocationId equals location.Id
                //                        join rating in db.TblRating on restaurant.Id equals rating.TblRestaurantId
                //                        select new { TblRestaurant = restaurant, TblLocation = location, TblRating = rating });

                if (!string.IsNullOrEmpty(searchList.cuisine))
                {
                    restaurantFilter = (from filteredRestaurant in restaurantFilter
                                        join offer in db.TblOffer on filteredRestaurant.TblRestaurant.Id equals offer.TblRestaurantId
                                        join menu in db.TblMenu on offer.TblMenuId equals menu.Id
                                        join cuisine in db.TblCuisine on menu.TblCuisineId equals cuisine.Id
                                        where cuisine.Cuisine.Contains(searchList.cuisine)
                                        select filteredRestaurant).Distinct();
                }
                if (!string.IsNullOrEmpty(searchList.Menu))
                {
                    restaurantFilter = (from filteredRestaurant in restaurantFilter
                                        join offer in db.TblOffer on filteredRestaurant.TblRestaurant.Id equals offer.TblRestaurantId
                                        join menu in db.TblMenu on offer.TblMenuId equals menu.Id
                                        where menu.Item.Contains(searchList.Menu)
                                        select filteredRestaurant).Distinct();
                }

                if (searchList.rating > 0)
                {
                    restaurantFilter = (from filteredRestaurant in restaurantFilter
                                        join rating in db.TblRating on filteredRestaurant.TblRestaurant.Id equals rating.TblRestaurantId
                                        where rating.Rating.Contains(searchList.rating.ToString())
                                        select filteredRestaurant).Distinct();

                    //restaurantFilter = (from filteredRestaurant in restaurantFilter
                    //                    where filteredRestaurant.TblRating.Rating.Contains(searchList.rating.ToString())
                    //                    select filteredRestaurant).Distinct();
                }

                foreach (var item in restaurantFilter)
                //var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
                //Parallel.ForEach(restaurantFilter, parallelOptions, item =>
                {
                    RestaurantSearchDetails restaurant = new RestaurantSearchDetails
                    {
                        restauran_ID = item.TblRestaurant.Id,
                        restaurant_Name = item.TblRestaurant.Name,
                        restaurant_Address = item.TblRestaurant.Address,
                        restaurant_PhoneNumber = item.TblRestaurant.ContactNo,
                        restraurant_Website = item.TblRestaurant.Website,
                        closing_Time = item.TblRestaurant.CloseTime,
                        opening_Time = item.TblRestaurant.OpeningTime,
                        //xaxis = (double)item.TblLocation.X,
                        xaxis = Convert.ToDouble(item.TblLocation.X),
                        //yaxis = (double)item.TblLocation.Y
                        yaxis = Convert.ToDouble(item.TblLocation.Y),
                        //rating = Convert.ToInt32(item.TblRating.Rating)
                    };
                    restaurants.Add(restaurant);
                    //});
                }
                //if (restaurants.Any())
                //{
                //    restaurants = restaurants.OrderByDescending(x => x.rating).OrderBy(y => y.restauran_ID).ToList();
                //}

                return restaurants;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<RestaurantSearchDetails> GetRetaurantBasedOnLocationAndName(LocationDetails location_Details)
        {
            List<RestaurantSearchDetails> restaurants = new List<RestaurantSearchDetails>();
            try
            {

                var restaurantInfo = (from restaurant in db.TblRestaurant
                                      join location in db.TblLocation on restaurant.TblLocationId equals location.Id
                                      select new { TblRestaurant = restaurant, TblLocation = location });

                //var restaurantInfo = (from restaurant in db.TblRestaurant
                //                      join location in db.TblLocation on restaurant.TblLocationId equals location.Id
                //                      join rating in db.TblRating on restaurant.Id equals rating.TblRestaurantId
                //                      select new { TblRestaurant = restaurant, TblLocation = location, TblRating = rating });

                if (!string.IsNullOrEmpty(location_Details.restaurant_Name))
                {
                    restaurantInfo = restaurantInfo.Where(a => a.TblRestaurant.Name.Contains(location_Details.restaurant_Name));
                }

                if (!(double.IsNaN(location_Details.xaxis)) && !(double.IsNaN(location_Details.yaxis)))
                {
                    foreach (var place in restaurantInfo)
                    //var options = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
                    //Parallel.ForEach(restaurantInfo, options, place =>
                    {
                        //double distance = Distance(location_Details.xaxis, location_Details.yaxis, (double)place.TblLocation.X, (double)place.TblLocation.Y);
                        double distance = Distance(location_Details.xaxis, location_Details.yaxis, Convert.ToDouble(place.TblLocation.X), Convert.ToDouble(place.TblLocation.Y));
                        if ((int.Parse(location_Details.distance.ToString()) == 0 || distance < int.Parse(location_Details.distance.ToString()))
                            && (location_Details.xaxis == 0 || location_Details.xaxis == Convert.ToDouble(place.TblLocation.X))
                            && (location_Details.yaxis == 0 || location_Details.yaxis == Convert.ToDouble(place.TblLocation.Y)))
                        {
                            RestaurantSearchDetails tblRestaurant = new RestaurantSearchDetails
                            {
                                restauran_ID = place.TblRestaurant.Id,
                                restaurant_Name = place.TblRestaurant.Name,
                                restaurant_Address = place.TblRestaurant.Address,
                                restaurant_PhoneNumber = place.TblRestaurant.ContactNo,
                                restraurant_Website = place.TblRestaurant.Website,
                                closing_Time = place.TblRestaurant.CloseTime,
                                opening_Time = place.TblRestaurant.OpeningTime,
                                xaxis = Convert.ToDouble(place.TblLocation.X),
                                //xaxis = (double)place.TblLocation.X,
                                yaxis = Convert.ToDouble(place.TblLocation.Y),
                                //yaxis = (double)place.TblLocation.Y
                                //rating = Convert.ToInt32(place.TblRating.Rating)
                            };
                            restaurants.Add(tblRestaurant);
                        }
                        //});
                    }
                }
                //if (restaurants.Any())
                //{
                //    restaurants = restaurants.OrderByDescending(x => x.rating).OrderBy(y => y.restauran_ID).ToList();
                //}

                return restaurants;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private double Distance(double currentLatitude, double currentLongitude, double latitude, double longitude)
        {
            double theta = currentLatitude - latitude;
            double dist = Math.Sin(GetRadius(currentLatitude)) * Math.Sin(GetRadius(longitude)) + Math.Cos(GetRadius(currentLatitude)) * Math.Cos(GetRadius(latitude)) * Math.Cos(GetRadius(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = (dist * 60 * 1.1515) / 0.6213711922;          //miles to kms
            return (dist);
        }

        private double rad2deg(double dist)
        {
            return (dist * Math.PI / 180.0);
        }

        private double GetRadius(double Latitude)
        {
            return (Latitude * 180.0 / Math.PI);
        }
        #endregion
    }
}
