namespace Access.Auth.Service.Host.Models
{
	public class ErrorViewModel
	{
		public string RequestId { get; set; }

		public bool ShowRequestId => string.IsNullOrEmpty(this.RequestId) == false;
	}
}
