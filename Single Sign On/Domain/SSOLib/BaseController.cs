using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using SSOLib;
using System.Configuration;
using System.Web;
using SSOLib.Service;
using Microsoft.AspNet.Identity;
using ARS.ITCS.ControlPanel;
using Microsoft.AspNet.Identity.Owin;
using ARS.ITCS.Core;
using ARS.ITCS.ControlPanel.ViewModels;

namespace SSOLib
{
    public class BaseController : Controller
    {
        /// <summary>
        /// The sign in manager
        /// </summary>
        private ApplicationSignInManager _signInManager;
        /// <summary>
        /// The user manager
        /// </summary>
        private ApplicationUserManager _userManager;

        /// <summary>
        /// Gets the sign in manager.
        /// </summary>
        /// <value>The sign in manager.</value>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        /// <summary>
        /// Gets the user manager.
        /// </summary>
        /// <value>The user manager.</value>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// The unit of work
        /// </summary>
        protected IUnitOfWork _UnitOfWork;

        /// <summary>
        /// Gets the logged in user.
        /// </summary>
        /// <value>The logged in user.</value>
        public LoggedInUser LoggedInUser
        {
            get
            {
                var authManager = this.HttpContext.GetOwinContext().Authentication;
                if (authManager != null)
                {
                    var user = authManager.User;
                    if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
                    {
                        LoggedInUser loggedInUser = (LoggedInUser)Session["LoggedInUser"];
                        return loggedInUser;
                    }
                }
                return null;
            }
        }

        public BaseController()
        {

        }
       /* /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        protected BaseController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
           // MemoryCacheManager._unitOfWork = unitOfWork;
        }
        */
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        protected BaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }


        private string Action;
        private string Token;
        private string LoginUrl = ConfigurationManager.AppSettings[AppConstants.Urls.LOGIN_URL].ToLower();
        private string DefaultUrl = ConfigurationManager.AppSettings[AppConstants.Urls.DEFAULT_URL].ToLower();
        private string SSOSiteUrlConfig = ConfigurationManager.AppSettings[AppConstants.Urls.SSO_SITE_URL];
        private string LoginPageName;
        private string DefaultPageName;
        private string RequestFilePath;
        private string RequestId;
        private string ReturnUrl;
        string Path;

        protected bool IsCachingEnabled = false;

        protected WebUser CurrentUser
        {
            get
            {
                return SessionAPI.CurrentUser;
            }
            set
            {
                SessionAPI.CurrentUser = value;
            }
        }

        /// <summary>
        /// The sign in manager
        /// </summary>
        public object MemoryCacheManager { get; private set; }

        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (LoggedInUser != null)
            {
            }

                SetCachingPreferences();

            //Read QueryString parameter values
            LoadParameters();

            if (Request.HttpMethod == "POST")
            {
                //If this is a postback, do not redirect to SSO site. 
                //Rather, hit a web method to the SSO site to know user's logged in status
                //and proceed based on the status
                HandlePostbackRequest();
             //   base.OnLoad(e);
                return;
            }

            //If the current request is marked not to be redirected to SSO site, do not proceed
            if (SessionAPI.RequestRedirectFlag == false)
            {
                SessionAPI.ClearRedirectFlag();
              //  base.OnLoad(e);
                return;
            }

            if (string.IsNullOrEmpty(RequestId))
            {
                //Absence of Request Paramter RequestId means current request is not redirected from SSO site. 
                //So, redirect to SSO site with ReturnUrl
                RedirectToSSOSite();
                return;
            }
            else
            {
                //Current request is redirected from the SSO site. So, check user status
                //And redirect to appropriate page
                ValidateUserStatusAndRedirect();
            }

            //if (LoggedInUser != null)
            //{
            //    ViewBag.FullName = LoggedInUser.FullName;
            //    ViewBag.UserId = LoggedInUser.UserId;
            //    ViewBag.UserName = LoggedInUser.UserName;
            //    ViewBag.Timeout = int.Parse(ConfigurationManager.AppSettings["SessionTimeout"]) * 60000; // Minutes to Milliseconds
            //}

            base.OnActionExecuting(filterContext);
        }
         

        /// <summary>
        /// Set caching parameters. By defualt caching is disabled
        /// </summary>
        private void SetCachingPreferences()
        {
            if (!IsCachingEnabled)
            {
                //No caching
                Response.Cache.SetExpires(DateTime.Now);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetValidUntilExpires(false);
                Response.Expires = 0;
            }
        }

        /// <summary>
        /// Redirect the current request to SSO site for authentication check
        /// </summary>
        /// <param name="Path"></param>
        private void RedirectToSSOSite()
        {
            string originalRequestUrl = Path;

            //Clean up all current QueryString parameters before redirecting to SSO site
            originalRequestUrl = UriUtil.RemoveParameter(originalRequestUrl, AppConstants.UrlParams.REQUEST_ID);
            originalRequestUrl = UriUtil.RemoveParameter(originalRequestUrl, AppConstants.UrlParams.ACTION);
            originalRequestUrl = UriUtil.RemoveParameter(originalRequestUrl, AppConstants.UrlParams.TOKEN);
            string ssoSiteUrl = string.Format(SSOSiteUrlConfig, HttpUtility.UrlEncode(originalRequestUrl));

            //Redirect to SSO site
            Response.Redirect(ssoSiteUrl);
        }

        /// <summary>
        /// Validates Token and RequestId and redirect to appropriate URL accordingly
        /// </summary>
        private void ValidateUserStatusAndRedirect()
        {
            UserStatus userStatus = AuthUtil.Instance.GetUserStauts(Token, RequestId);
            if (!userStatus.UserLoggedIn)
            {
                //User is not logged in at SSO site. So, return the Login page to user
                RedirectToLoginPage();
                return;
            }
            if (!userStatus.RequestIdValid)
            {
                //Current RequestId is not valid. That means, this is a page refresh and hence, redirect to SSO site
                RedirectToSSOSite();
                return;
            }
            if (CurrentUser == null || CurrentUser.Token != Token)
            {
                //Retrieve the user if the user is not found in session, or, the current user in session
                //is not the one who is currently logged onto the SSO site
                CurrentUser = AuthUtil.Instance.GetUserByToken(Token);
                if (CurrentUser.Token != Token || CurrentUser == null)
                {
                    RedirectToSSOSite();
                    return;
                }

            }

            //User is already logged in at SSO site. So, if user originally hit the Login page, redirect him/her to the default page
            if (RequestFilePath.Contains(LoginPageName))
            {
                RedirectToDefaultPageForLoginPage();
            }
        }

        /// <summary>
        /// Handle PostBack request and redirect to SSO site or log out, based upon the situation
        /// </summary>
        private void HandlePostbackRequest()
        {
            //This is a PostBack request

            if (RequestFilePath.Contains(LoginPageName))
            {
                //Among the private pages, the button click on the Login page should not redirect the current request to the SSO site.
                return;
            }
            else
            {
                //See whether user is still logged onto the SSO site.
                if (!AuthUtil.Instance.IsUserLoggedIn(Token))
                {
                    //User is not available at SSO site. So, redirect to the Login page.
                    //Before redirecting, make sure that, this redirect request is not redirected to the SSO site
                    //for authentication check.
                    RedirectToLoginPage();
                }
                else
                {
                    //User is logged onto the SSO site. So, return and proceed the postback event
                    return;
                }
            }
        }

        /// <summary>
        /// Redirect to Default page with current request params if current Request is for Login page and user is logged in
        /// </summary>
        /// <param name="Urlpath"></param>
        protected void RedirectToDefaultPageForLoginPage()
        {
            string RedirectUrl = string.IsNullOrEmpty(ReturnUrl) ? DefaultUrl : ReturnUrl;
            if (RedirectUrl.Contains(LoginPageName))
            {
                RedirectUrl = DefaultUrl;
            }
            RedirectUrl = string.Format("{0}{1}", RedirectUrl, Request.Url.Query);
            SessionAPI.RequestRedirectFlag = false;
            Response.Redirect(RedirectUrl);
        }

        /// <summary>
        /// Redirect to Login page
        /// </summary>
        /// <param name="Urlpath"></param>
        protected void RedirectToLoginPage()
        {
            //Before redirecting to login URL, remove the Token and RequestId parameter value from the QueryString (If they are there)
            //that were appended by the SSO sites. Reason is, these two parameter values are now expired. 
            //From the login screen, user will log in and the SSO site will re-generate the Token and RequestId
            string originalRequestUrl = Request.Url.OriginalString;
            originalRequestUrl = UriUtil.RemoveParameter(originalRequestUrl, AppConstants.UrlParams.REQUEST_ID);
            originalRequestUrl = UriUtil.RemoveParameter(originalRequestUrl, AppConstants.UrlParams.TOKEN);


            //Current request is redirected from SSO site. So, do not further redirect to SSO site
            SessionAPI.RequestRedirectFlag = false;
            Response.Redirect(VirtualPathUtility.ToAbsolute(string.Format("{0}?{1}={2}", LoginUrl, AppConstants.UrlParams.RETURN_URL, HttpUtility.UrlEncode(originalRequestUrl))));
        }

        /// <summary>
        /// Loads all request parameter valuess
        /// </summary>
        private void LoadParameters()
        {
            Action = Request.Params[AppConstants.UrlParams.ACTION];
            Token = Request.Params[AppConstants.UrlParams.TOKEN];
            RequestId = Request.Params[AppConstants.UrlParams.REQUEST_ID];
            LoginPageName = VirtualPathUtility.GetFileName(LoginUrl).ToLower();
            DefaultPageName = VirtualPathUtility.GetFileName(DefaultUrl).ToLower();
            RequestFilePath = Request.FilePath.ToLower();
            Path = Request.Url.AbsoluteUri.ToLower();
            ReturnUrl = Request.Params[AppConstants.UrlParams.RETURN_URL];
        }

        /// <summary>
        /// Performs login action onto server
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        protected bool Login(string UserName, string Password)
        {

            CurrentUser = AuthUtil.Instance.Authenticate(UserName, Password);

            if (CurrentUser != null)
            {
                string returnUrl = Request.Params[AppConstants.UrlParams.RETURN_URL];
                if (string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = UriUtil.GetAbsolutePathForRelativePath(DefaultUrl);
                }
                else
                {
                    returnUrl = UriUtil.RemoveParameter(returnUrl, AppConstants.UrlParams.ACTION);
                }
                string ssoSiteUrl = string.Format(SSOSiteUrlConfig, HttpUtility.UrlEncode(returnUrl));

                Response.Redirect(string.Format("{0}&{1}={2}", ssoSiteUrl, AppConstants.UrlParams.TOKEN, CurrentUser.Token));
            }

            return false;
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        protected void Logout()
        {
            if (CurrentUser == null)
            {
                RedirectToLoginPage();
                return;
            }
            string currentURL = Request.Url.OriginalString;
            currentURL = UriUtil.RemoveParameter(currentURL, AppConstants.UrlParams.REQUEST_ID);
            currentURL = UriUtil.RemoveParameter(currentURL, AppConstants.UrlParams.TOKEN);

            string ssoSiteUrl = string.Format(SSOSiteUrlConfig, HttpUtility.UrlEncode(currentURL));
            string LogoutUrl = string.Format("{0}&{1}={2}&{3}={4}", ssoSiteUrl, AppConstants.UrlParams.ACTION, AppConstants.ParamValues.LOGOUT, AppConstants.UrlParams.TOKEN, SessionAPI.CurrentUser.Token);
            CurrentUser = null;
            Response.Redirect(LogoutUrl);

        }


    }
}




