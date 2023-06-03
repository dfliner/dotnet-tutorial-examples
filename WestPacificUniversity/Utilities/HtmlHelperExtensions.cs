using Microsoft.AspNetCore.Mvc.Rendering;

namespace WestPacificUniversity.Utilities;

public static class HtmlHelperExtensions
{
    public static string NavbarMenuIsActive(
        this IHtmlHelper html,
        string? controllers = null,
        string? actions = null,
        string cssClass = "active")
    {
        string? currentController = html.ViewContext.RouteData.Values["controller"] as string;
        string? currentAction = html.ViewContext.RouteData.Values["action"] as string;

        string[] acceptedControllers = (controllers ?? currentController ?? "").Split(',');
        string[] acceptedActions = (actions ?? currentAction ?? "").Split(',');

        return
            (acceptedControllers.Contains(currentController) && acceptedActions.Contains(currentAction))
            ? cssClass
            : "";
    }
}
