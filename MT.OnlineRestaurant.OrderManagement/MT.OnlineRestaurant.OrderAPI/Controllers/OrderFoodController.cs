#region References
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MT.OnlineRestaurant.BusinessEntities;
using MT.OnlineRestaurant.BusinessLayer.interfaces;
using MT.OnlineRestaurant.OrderAPI.ModelValidators;
using System;
using System.Linq;
using System.Threading.Tasks;

using LoggingManagement;
using Microsoft.Azure.ServiceBus;
using System.Text;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using System.Net;
#endregion

#region Namespace
namespace MT.OnlineRestaurant.OrderAPI.Controllers
{
    /// <summary>
    /// Food ordering controller
    /// </summary>
    [Produces("application/json")]
    public class OrderFoodController : Controller
    {
        private readonly IPlaceOrderActions _placeOrderActions;
        private readonly ILogService _logService;
        /// <summary>
        /// Inject buisiness layer dependency
        /// </summary>
        /// <param name="placeOrderActions">Instance of this interface is injected in startup</param>
        /// <param name="logService"></param>
        public OrderFoodController(IPlaceOrderActions placeOrderActions, ILogService logService)
        {
            _placeOrderActions = placeOrderActions;
            _logService = logService;
        }
        /// <summary>
        /// POST api/OrderFood
        /// To order food
        /// </summary>
        /// <param name="orderEntity">Order entity</param>
        /// <returns>Status of order</returns>
        [HttpPost]
        [Route("api/OrderFood")]
        public async Task<IActionResult> Post([FromBody] OrderEntity orderEntity)
        {
            _logService.LogMessage("Order Entity received at endpoint : api/OrderFood, User ID : " + orderEntity.CustomerId);
            int UserId = (Request.Headers.ContainsKey("CustomerId") ? int.Parse(HttpContext.Request.Headers["CustomerId"]) : 0);
            string UserToken = (Request.Headers.ContainsKey("AuthToken") ? Convert.ToString(HttpContext.Request.Headers["AuthToken"]) : "");

            OrderEntityValidator orderEntityValidator = new OrderEntityValidator(UserId, UserToken, _placeOrderActions);
            ValidationResult validationResult = orderEntityValidator.Validate(orderEntity);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString("; "));
            }
            else
            {
                var result = await Task<int>.Run(() => _placeOrderActions.PlaceOrder(orderEntity));
                if (result == 0)
                {
                    return BadRequest("Failed to place order, Please try again later");
                }
            }
            return Ok("Order placed successfully");
        }

        /// <summary>
        /// DELETE api/CancelOrder
        /// Cancel the order
        /// </summary>
        /// <param name="id">Order id</param>
        /// <returns>Status of order</returns>
        [HttpDelete]
        [Route("api/CancelOrder")]
        public IActionResult Delete(int id)
        {
            var result = _placeOrderActions.CancelOrder(id);
            if (result > 0)
            {
                return Ok("Order cancelled successfully");
            }

            return BadRequest("Failed to cancel order, Please try again later");
        }

        /// <summary>
        /// View Reports
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Reports")]
        public IActionResult Reports(int customerId)
        {
            IQueryable<CustomerOrderReport> result = _placeOrderActions.GetReports(customerId);
            if (result.Any())
            {
                return Ok(result.ToList());
            }

            return BadRequest("Failed to get the reports");
        }

        /// <summary>
        /// GetCartDetails
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="menuId"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Cart")]
        public async Task<IActionResult> GetCartDetails(int restaurantId, int menuId, int quantity, decimal price)
        {
            try
            {
                if (restaurantId <= 0 || menuId <= 0 || quantity <= 0)
                {
                    return BadRequest("Invalid input.");
                }
                var result = await _placeOrderActions.GetItemInCartAvailabilityAndPrice(restaurantId, menuId, quantity, price);
                if (result != null && string.IsNullOrEmpty(result.Message))
                {
                    return Ok(result);
                }
                return NotFound("Invalid data.");
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, "Something went wrong, please try again.");
            }
        }
    }
}
#endregion