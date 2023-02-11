using System.Web.Mvc;

public class CustomAuthorize : AuthorizeAttribute
{
    private readonly string redirectUrl = string.Empty;
    public CustomAuthorize() : base()
    {
    }

    public CustomAuthorize(string redirectUrl) : base()
    {
        this.redirectUrl = redirectUrl;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        if (!filterContext.HttpContext.Request.IsAuthenticated)
        {
            string authUrl = redirectUrl; //passed from attribute

            //if null, get it from config
            if (string.IsNullOrEmpty(authUrl))
                authUrl = "~/Account/Login";

            if (!string.IsNullOrEmpty(authUrl))
                filterContext.HttpContext.Response.Redirect(authUrl);
        }

        //else do normal process
        base.HandleUnauthorizedRequest(filterContext);
    }
}