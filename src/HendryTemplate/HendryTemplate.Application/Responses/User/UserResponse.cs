using System.ComponentModel.DataAnnotations;

namespace HendryTemplate.Application.Responses.User
{
    public class UserResponse
    {
        #region fields
        [Required]
        public string Id { get; set; }
		[Required]
		public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
		#endregion
	}
}