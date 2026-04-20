using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Owin.Security.Providers.GitHub;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;

[assembly: OwinStartup(typeof(WebBanGauBong.Startup))]
namespace WebBanGauBong
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình Cookie chính cho ứng dụng 
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/User/LoginPage"),
                CookieSameSite = Microsoft.Owin.SameSiteMode.None, 
                CookieSecure = CookieSecureOption.Always
            });

            // Cấu hình Cookie tạm thời 
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Cấu hình Google Login
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["GoogleClientId"],
                ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"],
            });

            // Cấu hình GitHub Login
            var githubOptions = new Owin.Security.Providers.GitHub.GitHubAuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["GithubClientId"],
                ClientSecret = ConfigurationManager.AppSettings["GithubClientSecret"],
                Provider = new GitHubAuthenticationProvider
                {
                    OnAuthenticated = async context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("GitHubAccessToken", context.AccessToken));
                    }
                }
            };
            //githubOptions.Scope.Add("user:email");
            app.UseGitHubAuthentication(githubOptions);
        }

    }
}