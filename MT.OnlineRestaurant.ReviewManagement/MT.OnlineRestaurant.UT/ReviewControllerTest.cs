using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MT.OnlineRestaurant.BusinessLayer;
using MT.OnlineRestaurant.DataLayer.EntityFrameworkModel;
using MT.OnlineRestaurant.DataLayer.Repository;
using MT.OnlineRestaurant.ReviewManagement.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc;
using MT.OnlineRestaurant.BusinessEntities;
using System;

namespace MT.OnlineRestaurant.UT
{
    [TestClass]
    public class ReviewControllerTest
    {
        private readonly Mock<IReviewRepository> _mockReviewRepository;
        private readonly IRestaurantBusiness _restaurantBusiness;
        private readonly Review _restaurantRating;
        private readonly Review _updateRating;
        private readonly List<TblRating> _tblRatings;
        
        public ReviewControllerTest()
        {
            _mockReviewRepository = new Mock<IReviewRepository>();
            _restaurantBusiness = new RestaurantBusiness(_mockReviewRepository.Object);
            _tblRatings = new List<TblRating>()
            {
                new TblRating(){ Id = 1234, Rating = "9", Comments = "good restaurant.", CustomerId = 112, RestaurantId = 233 },
                new TblRating(){ Id = 1235, Rating = "5", Comments = "ok restaurant.", CustomerId = 124, RestaurantId = 233 }
            };
            _restaurantRating = new Review()
            {
                RestaurantId = 221,
                CustomerId = 121,
                Rating = 7,
                UserComments = "review comments"
            };
            _updateRating = new Review()
            {
                Id = 123,
                Rating = 7,
                UserComments = "review comments"
            };
        }

        [TestMethod]
        public async Task GetAsync_OkResult()
        {
            //Arrange
            _mockReviewRepository.Setup(x => x.GetRestaurantReview(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(_tblRatings);
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.GetAsync(13,0);
            var contentResult = actionResult as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
            Assert.IsNotNull(contentResult.Value);
        }

        [TestMethod]
        public async Task GetAsync_BadRequestResult()
        {
            //Arrange
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.GetAsync(0,0);
            var contentResult = actionResult as BadRequestObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task GetAsync_NotFoundResult()
        {
            var tblRatings = new List<TblRating>();
            _mockReviewRepository.Setup(x => x.GetRestaurantReview(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(tblRatings);
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.GetAsync(1234,0);
            var contentResult = actionResult as NotFoundObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundObjectResult));
            Assert.AreEqual(StatusCodes.Status404NotFound, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task GetAsync_InternalServerErrorResult()
        {
            _mockReviewRepository.Setup(x => x.GetRestaurantReview(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception());
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.GetAsync(1234, 0);
            var contentResult = actionResult as ObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status500InternalServerError, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task PostAsync_OkResult()
        {
            _mockReviewRepository.Setup(x => x.RestaurantReview(It.IsAny<TblRating>())).Verifiable();
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.PostAsync(_restaurantRating);
            var contentResult = actionResult as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Value);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task PostAsync_BadRequestResult()
        {
            var rating = new Review();
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.PostAsync(rating);
            var contentResult = actionResult as BadRequestObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task PostAsync_InternalServerErrorResult()
        {
            _mockReviewRepository.Setup(x => x.RestaurantReview(It.IsAny<TblRating>())).ThrowsAsync(new Exception());
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.PostAsync(_restaurantRating);
            var contentResult = actionResult as ObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status500InternalServerError, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task PutAsync_OkResult()
        {
            _mockReviewRepository.Setup(x => x.UpdateRestaurantReview(It.IsAny<TblRating>())).ReturnsAsync(true);
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.PutAsync(_updateRating.Id, _updateRating);
            var contentResult = actionResult as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Value);
            Assert.AreEqual(StatusCodes.Status200OK, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task PutAsync_BadRequestResult()
        {
            var rating = new Review();
            _mockReviewRepository.Setup(x => x.UpdateRestaurantReview(It.IsAny<TblRating>())).ReturnsAsync(true);
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.PutAsync(12, rating);
            var contentResult = actionResult as BadRequestObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(StatusCodes.Status400BadRequest, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task PutAsync_NotFoundResult()
        {
            _mockReviewRepository.Setup(x => x.UpdateRestaurantReview(It.IsAny<TblRating>())).ReturnsAsync(false);
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.PutAsync(_updateRating.Id, _updateRating);
            var contentResult = actionResult as NotFoundObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundObjectResult));
            Assert.AreEqual(StatusCodes.Status404NotFound, contentResult.StatusCode);
        }

        [TestMethod]
        public async Task PutAsync_InternalServerErrorResult()
        {
            _mockReviewRepository.Setup(x => x.UpdateRestaurantReview(It.IsAny<TblRating>())).ThrowsAsync(new Exception());
            var controller = new ReviewContoller(_restaurantBusiness);

            //Act
            var actionResult = await controller.PutAsync(_updateRating.Id, _updateRating);
            var contentResult = actionResult as ObjectResult;

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status500InternalServerError, contentResult.StatusCode);
        }
    }
}
