using HendryTemplate.Application.Commands.User.Create;
using HendryTemplate.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HendryTemplate.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ApiControllerBase
    {
        public UserController(IMediator mediator, ILogger<ApiControllerBase> logger, IWebHostEnvironment env)
                : base(mediator, logger, env) { }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<ActionResult<CreateUserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var response = await SendAsync(request, cancellationToken);
            return MapToResult(response);
        }

        //[AllowAnonymous]
        //[HttpPost("login")]
        //public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        //{
        //    var response = await SendAsync(request, cancellationToken);
        //    return MapToResult(response);
        //}
    }
}
