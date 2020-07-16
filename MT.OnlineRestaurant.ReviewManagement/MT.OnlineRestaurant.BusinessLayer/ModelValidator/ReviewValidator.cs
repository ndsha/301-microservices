using FluentValidation;
using MT.OnlineRestaurant.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MT.OnlineRestaurant.BusinessLayer.ModelValidator
{
    public class ReviewValidator : AbstractValidator<Review>
    {
        public ReviewValidator()
        {
            RuleFor(m => m.Id)
                .Equal(0)
                .WithMessage("Should not provide Rating Id.");

            RuleFor(m => m.CustomerId)
                .GreaterThan(0)
                .WithMessage("Customer Id is not valid.");

            RuleFor(m => m.RestaurantId)
                .GreaterThan(0)
                .WithMessage("Restaurant Id is not valid.");
            
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
