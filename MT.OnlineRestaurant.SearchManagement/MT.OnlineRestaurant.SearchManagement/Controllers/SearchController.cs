using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MT.OnlineRestaurant.BusinessEntities;
using MT.OnlineRestaurant.BusinessLayer;


namespace MT.OnlineRestaurant.SearchManagement.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class SearchController : Controller
    {
        private readonly IRestaurantBusiness business_Repo;
        public SearchController(IRestaurantBusiness _business_Repo)
        {
            business_Repo = _business_Repo;
        }

        [HttpGet]
        [Route("ResturantDetail")]
        public async Task<IActionResult> GetResturantDetail([FromQuery] int RestaurantID)
        {
            try
            {
                if (RestaurantID <= 0)
                {
                    return BadRequest();
                }

                RestaurantInformation resturantInformation = await Task<RestaurantInformation>.Run(() => business_Repo.GetResturantDetails(RestaurantID));
                if (resturantInformation != null)
                {
                    return Ok(resturantInformation);
                }

                return NotFound();
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, string.Empty);
            }

        }

        [HttpGet]
        [Route("ResturantMenuDetail")]
        public async Task<IActionResult> GetResturantMenuDetail([FromQuery] int RestaurantID)
        {
            try
            {
                if (RestaurantID <= 0)
                {
                    return BadRequest();
                }

                IQueryable<RestaurantMenu> restaurantMenuDetails = await Task<IQueryable<RestaurantMenu>>.Run(() => business_Repo.GetRestaurantMenus(RestaurantID));
                if (restaurantMenuDetails != null && restaurantMenuDetails.Any())
                {
                    return Ok(restaurantMenuDetails);
                }

                return NotFound();
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, string.Empty);
            }
        }

        [HttpGet]
        [Route("ResturantRating")]
        public async Task<IActionResult> GetResturantRating([FromQuery] int RestaurantID)
        {
            try
            {
                if (RestaurantID <= 0)
                {
                    return BadRequest();
                }

                IQueryable<RestaurantRating> restaurantRatings = await Task<IQueryable<RestaurantRating>>.Run(() => business_Repo.GetRestaurantRating(RestaurantID));
                if (restaurantRatings != null && restaurantRatings.Any())
                {
                    return Ok(restaurantRatings);
                }

                return NotFound();
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, string.Empty);
            }
        }

        [HttpPost]
        [Route("ResturantRating")]
        public async Task<IActionResult> ResturantRating([FromQuery] RestaurantRating restaurantRating)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                await Task.Run(() => business_Repo.RestaurantRating(restaurantRating));

                return Ok("Submitted the reviewes");
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, string.Empty);
            }
        }

        [HttpGet]
        [Route("ResturantTable")]
        public async Task<IActionResult> GetResturantTableDetails([FromQuery] int RestaurantID)
        {
            try
            {
                if (RestaurantID <= 0)
                {
                    return BadRequest();
                }

                IQueryable<RestaurantTables> restaurant_TableDetails = await Task<IQueryable<RestaurantTables>>.Run(() => business_Repo.GetTableDetails(RestaurantID));
                if (restaurant_TableDetails != null && restaurant_TableDetails.Any())
                {
                    return Ok(restaurant_TableDetails);
                }
                return NotFound();
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, string.Empty);
            }
        }

        [HttpPost]
        [Route("SearchRestaurantBasedOnDistance")]
        public async Task<IActionResult> SearchRestaurantBasedOnDistance([FromBody] LocationDetails locationDetails)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                IQueryable<RestaurantInformation> restaurantDetails = await Task<IQueryable<RestaurantInformation>>.Run(() => business_Repo.SearchRestaurantByLocation(locationDetails));
                if (restaurantDetails != null && restaurantDetails.Any())
                {
                    return Ok(restaurantDetails);
                }

                return NotFound();
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, string.Empty);
            }
        }

        [HttpPost]
        [Route("SearchRestaurantBasedOnMenu")]
        public async Task<IActionResult> SearchRestaurantBasedOnMenu([FromBody] AdditionalFeatureForSearch additionalFeatureForSearch)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                IQueryable<RestaurantInformation> restaurantDetails = await Task<IQueryable<RestaurantInformation>>.Run(() => business_Repo.GetRestaurantsBasedOnMenu(additionalFeatureForSearch));
                if (restaurantDetails != null && restaurantDetails.Any())
                {
                    return Ok(restaurantDetails);
                }

                return NotFound();
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, string.Empty);
            }
        }

        [HttpPost]
        [Route("SearchForRestaurant")]
        public async Task<IActionResult> SearchForRestaurant([FromBody] SearchForRestaurant searchDetails)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                IQueryable<RestaurantInformation> restaurantDetails = await Task<IQueryable<RestaurantInformation>>.Run(() => business_Repo.SearchForRestaurant(searchDetails));
                if (restaurantDetails != null && restaurantDetails.Any())
                {
                    return Ok(restaurantDetails);
                }

                return NotFound();
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, string.Empty);
            }
        }

        [HttpGet]
        [Route("OrderDetail")]
        public async Task<IActionResult> OrderDetail([FromQuery] int restaurantID, int menuID)
        {
            try
            {
                if (restaurantID <= 0 || menuID <= 0)
                {
                    return BadRequest();
                }

                //int query_result = business_Repo.ItemInStock(restaurantID, menuID);
                int query_result = await Task<int>.Run(() => business_Repo.ItemInStock(restaurantID, menuID));
                if (query_result > 0)
                {
                    return Ok(restaurantID);
                }
                return NotFound();
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "error");
            }
        }
    }
}