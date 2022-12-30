using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Deliveries.Commands.CreateOrderDeliveryCommand;

public class CreateOrderDeliveryCommandValidator : AbstractValidator<CreateOrderDeliveryCommand>
{
    public CreateOrderDeliveryCommandValidator()
    {
        RuleFor(x => x.orderId).NotEmpty();
    }
}

