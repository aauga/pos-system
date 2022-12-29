using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Payments.Commands.CreateOrderPaymentCommand;

public class CreateOrderPaymentCommandValidator : AbstractValidator<CreateOrderPaymentCommand>
{
    public CreateOrderPaymentCommandValidator()
    {
        RuleFor(x => x.orderId).NotEmpty();
    }
}

