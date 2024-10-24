using Application.DTOs;
using Application.Shared;
using CSharpFunctionalExtensions;
using MediatR;

namespace Application.CQRS.Auth;

public sealed record SignUpCommand(SignUpDto signUpDto) : IRequest<Result<bool, ApplicationError>>;

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, Result<bool, ApplicationError>>
{
    public async Task<Result<bool, ApplicationError>> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        return Result.Success<bool, ApplicationError>(true);
    }
}