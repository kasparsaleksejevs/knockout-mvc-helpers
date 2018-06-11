using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace KnockMvc.Web.Models
{
    public static class GenericUrlActionHelper
    {
        /// <summary>
        /// Generates a fully qualified URL to an action method 
        /// </summary>
        public static string Action<TController>(this System.Web.Mvc.UrlHelper urlHelper, Expression<Action<TController>> action)
           where TController : System.Web.Mvc.Controller
        {
            RouteValueDictionary rvd = InternalExpressionHelper.GetRouteValues(action);
            return urlHelper.Action(null, null, rvd);
        }

        public const string HttpAttributeRouteWebApiKey = "__RouteName";

        public static string HttpRouteUrl2<TController>(this System.Web.Mvc.UrlHelper urlHelper, Expression<Action<TController>> expression)
           where TController : System.Web.Http.Controllers.IHttpController
        {
            var routeValues = expression.GetRouteValues();
            var httpRouteKey = System.Web.Http.Routing.HttpRoute.HttpRouteKey;
            if (!routeValues.ContainsKey(httpRouteKey))
            {
                routeValues.Add(httpRouteKey, true);
            }
            var url = string.Empty;
            if (routeValues.ContainsKey(HttpAttributeRouteWebApiKey))
            {
                var routeName = routeValues[HttpAttributeRouteWebApiKey] as string;
                routeValues.Remove(HttpAttributeRouteWebApiKey);
                routeValues.Remove("controller");
                routeValues.Remove("action");
                url = urlHelper.HttpRouteUrl(routeName, routeValues);
            }
            else
            {
                var path = resolvePath<TController>(routeValues, expression);
                var root = getRootPath(urlHelper);
                url = root + path;
            }
            return url;
        }

        private static string resolvePath<TController>(RouteValueDictionary routeValues, Expression<Action<TController>> expression) where TController : System.Web.Http.Controllers.IHttpController
        {
            var controllerName = routeValues["controller"] as string;
            var actionName = routeValues["action"] as string;
            routeValues.Remove("controller");
            routeValues.Remove("action");

            var method = expression.AsMethodCallExpression().Method;

            var configuration = System.Web.Http.GlobalConfiguration.Configuration;
            var apiDescription = configuration.Services.GetApiExplorer().ApiDescriptions
               .FirstOrDefault(c =>
                   c.ActionDescriptor.ControllerDescriptor.ControllerType == typeof(TController)
                   && c.ActionDescriptor.ControllerDescriptor.ControllerType.GetMethod(actionName) == method
                   && c.ActionDescriptor.ActionName == actionName
               );

            var route = apiDescription.Route;
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary(routeValues));

            var request = new System.Net.Http.HttpRequestMessage();
            request.Properties[System.Web.Http.Hosting.HttpPropertyKeys.HttpConfigurationKey] = configuration;
            request.Properties[System.Web.Http.Hosting.HttpPropertyKeys.HttpRouteDataKey] = routeData;

            var virtualPathData = route.GetVirtualPath(request, routeValues);

            var path = virtualPathData.VirtualPath;

            return path;
        }

        private static string getRootPath(System.Web.Mvc.UrlHelper urlHelper)
        {
            var request = urlHelper.RequestContext.HttpContext.Request;
            var scheme = request.Url.Scheme;
            var server = request.Headers["Host"] ?? string.Format("{0}:{1}", request.Url.Host, request.Url.Port);
            var host = string.Format("{0}://{1}", scheme, server);
            var root = host + ToAbsolute("~");
            return root;
        }

        static string ToAbsolute(string virtualPath)
        {
            return VirtualPathUtility.ToAbsolute(virtualPath);
        }
    }

    static class InternalExpressionHelper
    {
        /// <summary>
        /// Extract route values from strongly typed expression
        /// </summary>
        public static RouteValueDictionary GetRouteValues<TController>(
            this Expression<Action<TController>> expression,
            RouteValueDictionary routeValues = null)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            routeValues = routeValues ?? new RouteValueDictionary();

            var controllerType = ensureController<TController>();

            routeValues["controller"] = ensureControllerName(controllerType); ;

            var methodCallExpression = AsMethodCallExpression<TController>(expression);

            routeValues["action"] = methodCallExpression.Method.Name;

            //Add parameter values from expression to dictionary
            var parameters = buildParameterValuesFromExpression(methodCallExpression);
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    routeValues.Add(parameter.Key, parameter.Value);
                }
            }

            //Try to extract route attribute name if present on an api controller.
            if (typeof(System.Web.Http.Controllers.IHttpController).IsAssignableFrom(controllerType))
            {
                var routeAttribute = methodCallExpression.Method.GetCustomAttribute<System.Web.Http.RouteAttribute>(false);
                if (routeAttribute != null && routeAttribute.Name != null)
                {
                    routeValues[GenericUrlActionHelper.HttpAttributeRouteWebApiKey] = routeAttribute.Name;
                }
            }

            return routeValues;
        }

        private static string ensureControllerName(Type controllerType)
        {
            var controllerName = controllerType.Name;
            if (!controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Action target must end in controller", "action");
            }
            controllerName = controllerName.Remove(controllerName.Length - 10, 10);
            if (controllerName.Length == 0)
            {
                throw new ArgumentException("Action cannot route to controller", "action");
            }
            return controllerName;
        }

        internal static MethodCallExpression AsMethodCallExpression<TController>(this Expression<Action<TController>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
                throw new InvalidOperationException("Expression must be a method call.");

            if (methodCallExpression.Object != expression.Parameters[0])
                throw new InvalidOperationException("Method call must target lambda argument.");

            return methodCallExpression;
        }

        private static Type ensureController<TController>()
        {
            var controllerType = typeof(TController);

            bool isController = controllerType != null
                   && controllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                   && !controllerType.IsAbstract
                   && (
                        typeof(System.Web.Mvc.IController).IsAssignableFrom(controllerType)
                        || typeof(System.Web.Http.Controllers.IHttpController).IsAssignableFrom(controllerType)
                      );

            if (!isController)
            {
                throw new InvalidOperationException("Action target is an invalid controller.");
            }
            return controllerType;
        }

        private static RouteValueDictionary buildParameterValuesFromExpression(MethodCallExpression methodCallExpression)
        {
            RouteValueDictionary result = new RouteValueDictionary();
            ParameterInfo[] parameters = methodCallExpression.Method.GetParameters();
            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    object value;
                    var expressionArgument = methodCallExpression.Arguments[i];
                    if (expressionArgument.NodeType == ExpressionType.Constant)
                    {
                        // If argument is a constant expression, just get the value
                        value = (expressionArgument as ConstantExpression).Value;
                    }
                    else
                    {
                        try
                        {
                            // Otherwise, convert the argument subexpression to type object,
                            // make a lambda out of it, compile it, and invoke it to get the value
                            var convertExpression = Expression.Convert(expressionArgument, typeof(object));
                            value = Expression.Lambda<Func<object>>(convertExpression).Compile().Invoke();
                        }
                        catch
                        {
                            // ?????
                            value = String.Empty;
                        }
                    }
                    result.Add(parameters[i].Name, value);
                }
            }
            return result;
        }
    }
}