using Umbraco.Cms.Web.Common.PublishedModels;

namespace Umbraco26.Models.ViewModels
{
    public class LoginPageViewModel : PageViewModel<Login>
    {
        /// <summary>
        /// The local page to go back to after logging in, from <c>?returnUrl=</c>; without one the member stays on the
        /// login page, which sends them on to My account.
        /// </summary>
        public string? ReturnUrl { get; init; }
    }
}
