using FluentValidation;
using MT.OnlineRestaurant.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.OnlineRestaurant.BusinessLayer.ModelValidator
{
    public class UpdateReviewValidator : AbstractValidator<Review>
    {
        public UpdateReviewValidator()
        {
            RuleFor(m => m.Id)
                .GreaterThan(0)
                .WithMessage("Review Id is not valid.");

            RuleFor(m => m.CustomerId)
                .Equal(0)
                .WithMessage("Customer Id should not be provided.");

            RuleFor(m => m.RestaurantId)
                .Equal(0)
                .WithMessage("Restaurant Id should not be provided.");

            RuleFor(m => m.Rating)
                .GreaterThan(0)
                .LessThanOrEqualTo(10)
                .WithMessage("Rating is not valid.");

            RuleFor(m => m.UserComments)
                .NotEmpty()
                .NotNull()
                .WithMessage("User Comments must be valid.");
        }
    }
}
