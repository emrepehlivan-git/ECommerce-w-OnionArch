using Ardalis.Result;
using ECommerce.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace ECommerce.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IValidatableRequest
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any() || request is not IValidatableRequest)
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .Select(e => new ValidationError(e.ErrorMessage, e.PropertyName, e.ErrorCode, ValidationSeverity.Error))
            .ToList();

        if (failures.Any())
            return (TResponse)Result.Invalid(failures);

        return await next();
    }
}