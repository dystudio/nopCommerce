namespace Nop.Plugin.Misc.RestService.Common
{
    public class UserAccessModel
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ServerUrl { get; set; }

        public string RedirectUrl { get; set; }
    }

    public class AccessTokenViewModel
    {
        public string AccessToken { get; set; }
    }
}