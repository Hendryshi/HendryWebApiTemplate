using FluentResults;
using MediatR;
using HendryTemplate.Application.Commands.User.Models;

namespace HendryTemplate.Application.Commands.User.Create
{
    public class CreateUserRequest : IRequest<Result<CreateUserResponse>>
    {
        public UserCommand Data { get; set; }
    }
}
