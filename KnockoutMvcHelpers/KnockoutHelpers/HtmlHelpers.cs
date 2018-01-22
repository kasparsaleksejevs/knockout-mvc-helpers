using KnockoutHelpers.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KnockoutHelpers
{
    public static class HtmlHelpers
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null)
        {
            string cssClass = "active";
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (string.IsNullOrEmpty(controller))
                controller = currentController;

            if (string.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ? cssClass : string.Empty;
        }

        public static MvcHtmlString RawJson(this HtmlHelper html, object obj)
        {
            return MvcHtmlString.Create(JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        public static MvcHtmlString KoFormGroupFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, int editDivSize = 10)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Expression must be a member expression");

            var data = "<div class=\"form-group\" data-bind=\"validationElement: {0}\">{1}<div class=\"col-sm-" + editDivSize.ToString() + "\">{2}</div></div>";
            var label = helper.LabelFor(expression, new { @class = "col-sm-2 control-label" });
            var input = helper.KoEditorFor(expression);
            return MvcHtmlString.Create(string.Format(CultureInfo.CurrentUICulture, data, memberExpression.Member.Name, label, input));
        }

        public static MvcHtmlString KoLabelFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Expression must be a member expression");

            return helper.LabelFor(expression, new { @class = "col-sm-2 control-label" });
        }

        public static MvcHtmlString KoHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Expression must be a member expression");

            var knockout = GetKnockoutDataBind(memberExpression.Member as PropertyInfo);
            return helper.HiddenFor(expression, new { data_bind = knockout });
        }

        public static MvcHtmlString KoEditorFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Expression must be a member expression");

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new Exception("memberExpression.Member is not a PropertyInfo");

            MvcHtmlString editorControl = null;
            var knockout = GetKnockoutDataBind(propertyInfo);

            var propertyType = propertyInfo.PropertyType;
            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                var checkboxControl = helper.EditorFor(expression, new { htmlAttributes = new { data_bind = knockout } });
                editorControl = MvcHtmlString.Create("<div class=\"checkbox\"><label>" + checkboxControl + "</label></div>");
            }
            else
                editorControl = helper.EditorFor(expression, new { htmlAttributes = new { @class = "form-control", data_bind = knockout } });

            return editorControl;
        }

        /// <summary>
        /// Determines whether this is debug build.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <returns>
        ///   <c>True</c> if this is debug  build; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDebugBuild(this HtmlHelper helper)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        private static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            var attributes = provider.GetCustomAttributes(typeof(T), true);
            return attributes.Length > 0 ? attributes[0] as T : null;
        }

        private static string GetKnockoutDataBind(PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            var knockoutExtensions = new List<string>();

            var isRequired = propertyInfo.GetAttribute<RequiredAttribute>() != null;
            if (isRequired)
                knockoutExtensions.Add("required: true");

            var stringLengthAttr = propertyInfo.GetAttribute<StringLengthAttribute>();
            if (stringLengthAttr != null)
            {
                if (stringLengthAttr.MinimumLength > 0)
                    knockoutExtensions.Add("minLength: " + stringLengthAttr.MinimumLength.ToString());

                knockoutExtensions.Add("maxLength: " + stringLengthAttr.MaximumLength.ToString());
            }

            // setting default knockout binding first:
            var knockout = "value: " + propertyInfo.Name;
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                knockout = "datePicker: " + propertyInfo.Name;
            else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                knockout = "checked: " + propertyInfo.Name;

            // ToDo: nullable
            if (propertyType == typeof(int) || propertyType == typeof(decimal))
            {
                var numericPrecision = 0;
                var precisionAttr = propertyInfo.GetAttribute<PrecisionAttribute>();
                if (precisionAttr != null)
                    numericPrecision = precisionAttr.Scale;

                knockoutExtensions.Add("number: true");
                var numberRange = propertyInfo.GetAttribute<RangeAttribute>();
                if (numberRange != null)
                {
                    knockoutExtensions.Add("min: " + numberRange.Minimum.ToString());
                    knockoutExtensions.Add("max: " + numberRange.Maximum.ToString());
                }

                knockoutExtensions.Add("numeric: " + numericPrecision.ToString());
            }

            //[Range(0, 360)]
            //[Precision(18, 7)]

            var dataTypeAttr = propertyInfo.GetAttribute<DataTypeAttribute>();
            if (dataTypeAttr != null)
            {
                if (dataTypeAttr.DataType == DataType.MultilineText)
                    knockout = "summernote: " + propertyInfo.Name;
            }

            // https://github.com/Knockout-Contrib/Knockout-Validation/wiki/Native-Rules
            if (knockoutExtensions.Count > 0)
                knockout += ".extend({" + string.Join(", ", knockoutExtensions) + "}), valueUpdate: ['blur', 'keyup']";

            return knockout;
        }
    }
}