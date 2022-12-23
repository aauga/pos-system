namespace Application.Common.Models;

public record ValidationFailure
{
    public string PropertyName { get; init; }
    public string ErrorMessage { get; init; }
}