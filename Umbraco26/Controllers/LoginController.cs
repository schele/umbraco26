using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco26.Models.ViewModels;

namespace Umbraco26.Controllers
{
    /// <summary>
    /// The member login page. The forms post to Umbraco's own login and two-factor controllers, which show this page
    /// again with errors, or with the code form when the member has two-factor login turned on.
    /// </summary>
    public class LoginController(PageControllerDependencies dependencies, IMemberManager memberManager)
        : BasePageController<Login, LoginPageViewModel>(dependencies)
    {
        /// <summary>Sends members who are already logged in, such as right after logging in here, on to My account.</summary>
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public override IActionResult Index()
        {
            if (memberManager.IsLoggedIn()
                && CurrentPage is Login page
                && page.AncestorOrSelf<Start>()?.FirstChild<Account>()?.Url() is string accountUrl)
            {
                return Redirect(accountUrl);
            }

            return base.Index();
        }

        protected override LoginPageViewModel Build(Login page)
        {
            var returnUrl = Request.Query["returnUrl"].ToString();

            return new() { Content = page, ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null };
        }
    }
}
