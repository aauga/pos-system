using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Carts.Commands.UpdateOrderCartCommand;

public class UpdateOrderCartCommandValidator : AbstractValidator<UpdateOrderCartCommand>
{
    public UpdateOrderCartCommandValidator()
    {
        RuleFor(x => x.cartBodyDto.Discount).InclusiveBetween(0, 100);
        RuleFor(x => x.cartBodyDto.Quantity).GreaterThanOrEqualTo(1);
    }
}

