using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Carts.Commands.CreateOrderCartCommand;

public class CreateOrderCartCommandValidator : AbstractValidator<CreateOrderCartCommand>
{
    public CreateOrderCartCommandValidator()
    {
        RuleFor(x => x.cartItemIdDto.ItemId).NotEmpty();
        RuleFor(x => x.cartItemIdDto.Discount).InclusiveBetween(0, 100);
        RuleFor(x => x.cartItemIdDto.Quantity).GreaterThanOrEqualTo(1);
    }
}

