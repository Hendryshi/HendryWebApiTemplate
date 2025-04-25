using HendryTemplate.Application.Responses.User;

namespace HendryTemplate.Application.Commands.User.Create
{
	public sealed record CreateUserResponse
	{
		public UserResponse Data { get; set; }
	}
}
