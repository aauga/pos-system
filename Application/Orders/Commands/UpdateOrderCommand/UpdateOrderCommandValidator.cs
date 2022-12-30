using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Orders.Commands.UpdateOrderCommand;

public class UpdateOrderCartCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCartCommandValidator()
    {
        RuleFor(x => x.orderBodyDto.CustomerId).NotEmpty();
        RuleFor(x => x.orderBodyDto.Total).GreaterThanOrEqualTo(0);
        RuleFor(x => x.orderBodyDto.Tip).GreaterThanOrEqualTo(0);
        RuleFor(x => x.orderBodyDto.Delivery).Length(0, 20);
        RuleFor(x => x.orderBodyDto.Date).NotEmpty();
    }
}

