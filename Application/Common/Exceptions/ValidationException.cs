using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string propertyName, string errorMessage)
        : base("One or more validation failures have occurred.")
    {
        var validationFailure = new ValidationFailure
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage
        };

        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new string[] { errorMessage } },
        };
    }

    public IDictionary<string, string[]> Errors { get; }
}