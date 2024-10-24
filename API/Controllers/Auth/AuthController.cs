using Application.CQRS.Auth;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Auth;

public class AuthController : BaseController
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
    {
        return HttpResult(await Mediator.Send(new SignUpCommand(signUpDto)));
    }
}