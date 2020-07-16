using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
//using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCaching;
using MT.OnlineRestaurant.BusinessEntities;
using MT.OnlineRestaurant.BusinessLayer;
using MT.OnlineRestaurant.BusinessLayer.ModelValidator;

namespace MT.OnlineRestaurant.ReviewManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewContoller : ControllerBase
    {
        private readonly IRestaurantBusiness _restaurantBusiness;

        public ReviewContoller(IRestaurantBusiness restaurantBusiness)
        {
            _restaurantBusiness = restaurantBusiness;
        }

        [HttpGet("{restaurantId}")]
        public async Task<IActionResult> GetAsync([FromRoute] int restaurantId, [FromQuery] int customerId)
        {
            try
            {
                if (restaurantId <= 0)
                {
                    return BadRequest("Restaurant Id must be valid value.");
                }

                var restaurantReviews = await _restaurantBusiness.GetRestaurantReview(restaurantId, customerId);
                if (restaurantReviews != null && restaurantReviews.Any())
                {
                    return Ok(restaurantReviews);
                }

                return NotFound("No Reviews found for the Restaurant.");
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error occurred while fetching the data.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Review restaurantReview)
        {
            try
            {
                ReviewValidator validator = new ReviewValidator();
                ValidationResult validationResult = await validator.ValidateAsync(restaurantReview);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.ToString("; "));
                }

                await _restaurantBusiness.RestaurantReview(restaurantReview);

                return Ok("Submitted the review sucessfully.");
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error occurred while submitting the review. Please try again.");
            }
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> PutAsync([FromRoute] int reviewId, [FromBody] Review restaurantReview)
        {
            try
            {
                UpdateReviewValidator validator = new UpdateReviewValidator();
                ValidationResult validationResult = await validator.ValidateAsync(restaurantReview);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.ToString("; "));
                }

                if (reviewId < 0 || reviewId != restaurantReview.Id)
                {
                    return BadRequest("Review Id is not valid.");
                }


                var response = await _restaurantBusiness.UpdateRestaurantReview(restaurantReview);
                if (response)
                {
                    return Ok("Updated the review successfully.");
                }
                return NotFound("Cannot find the review.");
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error occurred while updating the review. Please try again.");
            }
        }

    }
}
