using MT.OnlineRestaurant.BusinessEntities;
using System;
using System.Collections.Generic;
using MT.OnlineRestaurant.DataLayer.Repository;
using MT.OnlineRestaurant.DataLayer.EntityFrameWorkModel;
using MT.OnlineRestaurant.DataLayer.DataEntity;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using MT.OnlineRestaurant.BusinessLayer.ServiceBus;

namespace MT.OnlineRestaurant.BusinessLayer
{
    public class RestaurantBusiness : IRestaurantBusiness
    {
        private readonly ISearchRepository search_Repository;

        public RestaurantBusiness(ISearchRepository _searchRepository)
        {
            search_Repository = _searchRepository;
        }

        public IQueryable<RestaurantMenu> GetRestaurantMenus(int restaurantID)
        {
            IQueryable<MenuDetails> menuDetails;
            List<RestaurantMenu> restaurant_Menu = new List<RestaurantMenu>();
            try
            {
                menuDetails = search_Repository.GetRestaurantMenu(restaurantID);
                foreach (var menu in menuDetails)
                {
                    RestaurantMenu menuInfo = new RestaurantMenu
                    {
                        menu_ID = menu.tbl_Offer.Id,
                        dish_Name = menu.tbl_Menu.Item,
                        price = menu.tbl_Offer.Price,
                        running_Offer = menu.tbl_Offer.Discount,
                        cuisine = menu.tbl_Cuisine.Cuisine
                    };
                    restaurant_Menu.Add(menuInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return restaurant_Menu.AsQueryable();
        }

        public IQueryable<RestaurantRating> GetRestaurantRating(int restaurantID)
        {
            try
            {
                List<RestaurantRating> restaurantRatings = new List<RestaurantRating>();
                IQueryable<TblRating> rating;
                rating = search_Repository.GetRestaurantRating(restaurantID);
                foreach (var item in rating)
                {
                    RestaurantRating ratings = new RestaurantRating
                    {
                        rating = item.Rating,
                        RestaurantId = item.TblRestaurantId,
                        user_Comments = item.Comments,
                        customerId = item.TblCustomerId,
                    };
                    restaurantRatings.Add(ratings);
                }
                return restaurantRatings.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RestaurantInformation GetResturantDetails(int restaurantID)
        {
            try
            {
                TblRestaurant restaurant = new TblRestaurant();
                restaurant = search_Repository.GetResturantDetails(restaurantID);
                if (restaurant != null)
                {
                    RestaurantInformation resturant_Information = new RestaurantInformation
                    {
                        restaurant_ID = restaurant.Id,
                        restaurant_Name = restaurant.Name,
                        restaurant_Address = restaurant.Address,
                        restaurant_ContactNo = restaurant.ContactNo,
                        closing_Time = restaurant.CloseTime,
                        opening_Time = restaurant.OpeningTime,
                        website = restaurant.Website,
                        xaxis = Convert.ToDouble(restaurant.TblLocation.X),
                        yaxis = Convert.ToDouble(restaurant.TblLocation.Y)
                    };
                    return resturant_Information;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<RestaurantTables> GetTableDetails(int restaurantID)
        {
            try
            {
                List<RestaurantTables> TableInfo = new List<RestaurantTables>();
                IQueryable<TblRestaurantDetails> restaurantTableCount;
                restaurantTableCount = search_Repository.GetTableDetails(restaurantID);
                foreach (var item in restaurantTableCount)
                {
                    RestaurantTables table = new RestaurantTables
                    {
                        restaurant_Name = item.TblRestaurant.Name,
                        table_Capacity = item.TableCapacity,
                        total_Count = item.TableCount
                    };
                    TableInfo.Add(table);
                }
                return TableInfo.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<RestaurantInformation> SearchRestaurantByLocation(BusinessEntities.LocationDetails locationDetails)
        {
            try
            {
                List<RestaurantInformation> restaurant_Info = new List<RestaurantInformation>();
                IQueryable<RestaurantSearchDetails> searched_Restaurant;
                DataLayer.DataEntity.LocationDetails location_Details = new DataLayer.DataEntity.LocationDetails
                {
                    distance = locationDetails.distance,
                    restaurant_Name = locationDetails.restaurant_Name,
                    xaxis = locationDetails.xaxis,
                    yaxis = locationDetails.yaxis
                };

                searched_Restaurant = search_Repository.GetRestaurantsBasedOnLocation(location_Details);
                foreach (var restaurants in searched_Restaurant)
                {
                    RestaurantInformation restaurant_Details = new RestaurantInformation
                    {
                        restaurant_ID = restaurants.restauran_ID,
                        restaurant_Name = restaurants.restaurant_Name,
                        restaurant_Address = restaurants.restaurant_Address,
                        restaurant_ContactNo = restaurants.restaurant_PhoneNumber,
                        closing_Time = restaurants.closing_Time,
                        opening_Time = restaurants.opening_Time,
                        website = restaurants.restraurant_Website,
                        xaxis = restaurants.xaxis,
                        yaxis = restaurants.yaxis,
                        rating = restaurants.rating
                    };
                    restaurant_Info.Add(restaurant_Details);
                }
                return restaurant_Info.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<RestaurantInformation> GetRestaurantsBasedOnMenu(AdditionalFeatureForSearch additionalFeatureForSearch)
        {
            try
            {
                List<RestaurantInformation> restaurant_Info = new List<RestaurantInformation>();
                IQueryable<RestaurantSearchDetails> searched_Restaurant;
                DataLayer.DataEntity.AddtitionalFeatureForSearch searchCritera = new DataLayer.DataEntity.AddtitionalFeatureForSearch
                {
                    cuisine = (string.IsNullOrEmpty(additionalFeatureForSearch.cuisine) ? "" : additionalFeatureForSearch.cuisine),
                    Menu = (string.IsNullOrEmpty(additionalFeatureForSearch.Menu) ? "" : additionalFeatureForSearch.Menu),
                    rating = additionalFeatureForSearch.rating > 0 ? additionalFeatureForSearch.rating : 0
                };

                searched_Restaurant = search_Repository.GetRestaurantsBasedOnMenu(searchCritera);
                if (searched_Restaurant != null)
                {
                    foreach (var restaurants in searched_Restaurant)
                    {
                        RestaurantInformation restaurant_Details = new RestaurantInformation
                        {
                            restaurant_ID = restaurants.restauran_ID,
                            restaurant_Name = restaurants.restaurant_Name,
                            restaurant_Address = restaurants.restaurant_Address,
                            restaurant_ContactNo = restaurants.restaurant_PhoneNumber,
                            closing_Time = restaurants.closing_Time,
                            opening_Time = restaurants.opening_Time,
                            website = restaurants.restraurant_Website,
                            xaxis = restaurants.xaxis,
                            yaxis = restaurants.yaxis,
                            rating = restaurants.rating
                        };
                        restaurant_Info.Add(restaurant_Details);
                    }
                }
                return restaurant_Info.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<RestaurantInformation> SearchForRestaurant(SearchForRestaurant searchDetails)
        {
            try
            {
                List<RestaurantInformation> restaurant_Info = new List<RestaurantInformation>();
                IQueryable<RestaurantSearchDetails> searched_Restaurant;
                SearchForRestautrant searchCriteria = new SearchForRestautrant()
                {
                    location = searchDetails.location == null ? new DataLayer.DataEntity.LocationDetails() :
                                    new DataLayer.DataEntity.LocationDetails()
                                    {
                                        distance = searchDetails.location.distance,
                                        restaurant_Name = string.IsNullOrEmpty(searchDetails.location.restaurant_Name) ? "" : searchDetails.location.restaurant_Name,
                                        xaxis = searchDetails.location.xaxis,
                                        yaxis = searchDetails.location.yaxis
                                    },
                    search = searchDetails.search == null ? new AddtitionalFeatureForSearch() :
                                    new AddtitionalFeatureForSearch()
                                    {
                                        cuisine = string.IsNullOrEmpty(searchDetails.search.cuisine) ? "" : searchDetails.search.cuisine,
                                        Menu = string.IsNullOrEmpty(searchDetails.search.Menu) ? "" : searchDetails.search.Menu,
                                        rating = searchDetails.search.rating > 0 ? searchDetails.search.rating : 0
                                    }
                };

                searched_Restaurant = search_Repository.SearchForRestaurant(searchCriteria);

                if (searched_Restaurant != null)
                {
                    foreach (var restaurants in searched_Restaurant)
                    {
                        RestaurantInformation restaurant_Details = new RestaurantInformation
                        {
                            restaurant_ID = restaurants.restauran_ID,
                            restaurant_Name = restaurants.restaurant_Name,
                            restaurant_Address = restaurants.restaurant_Address,
                            restaurant_ContactNo = restaurants.restaurant_PhoneNumber,
                            closing_Time = restaurants.closing_Time,
                            opening_Time = restaurants.opening_Time,
                            website = restaurants.restraurant_Website,
                            xaxis = restaurants.xaxis,
                            yaxis = restaurants.yaxis,
                            rating = restaurants.rating
                        };
                        restaurant_Info.Add(restaurant_Details);
                    }
                }
                return restaurant_Info.AsQueryable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Recording the customer rating the restaurants
        /// </summary>
        /// <param name=""></param>
        public void RestaurantRating(RestaurantRating restaurantRating)
        {
            if (restaurantRating != null)
            {
                TblRating rating = new TblRating()
                {
                    Rating = restaurantRating.rating,
                    TblRestaurantId = restaurantRating.RestaurantId,
                    Comments = restaurantRating.user_Comments,
                    TblCustomerId = restaurantRating.customerId
                };

                search_Repository.RestaurantRating(rating);
            }
        }
        public int ItemInStock(int restaurantID, int menuID)
        {
            RestaurantMenu menuObj = new RestaurantMenu();
            TblMenu menu = search_Repository.ItemInStock(restaurantID, menuID);
            menuObj.quantity = menu.quantity;
            return 0;
        }

        //public async Task GetItemInCartAvailabilityAndPrice()
        //{
        //    var request = await ServiceBusHelper.RegisterOnMessageHandlerAndReceiveMessages();
        //    var response = new CartItemsEntity()
        //    {
        //        IsItemAvailable = true,
        //        IsItemPriceChanged = false,
        //        ItemId = 123,
        //        ItemNewPrice = 345,
        //        ItemOldPrice = 345,
        //        ItemQuantity = 111,
        //        RestaurantId = 12
        //    };
        //    var helper = new ServiceBusHelper();
        //    await helper.SendMessagesAsync(response);
        //}
    }
}
