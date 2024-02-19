using Microsoft.AspNetCore.Authorization;

namespace QlBanGiay.Filters
{
	public class CustomerAuthenAttribute : AuthorizeAttribute
	{
		public string View { get; set; }
		public string Master { get; set; }

		public CustomerAuthenAttribute()
		{
			View = "error";
			Master = String.Empty;
		}
	}
}
