using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Routing;
using RestApp.Common;
using System.Text;
using System.Drawing;

namespace System.Web.Mvc.Html
{
    public static class ApHtmmlHelper
    {
        public static MvcHtmlString ApTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TValue>> expression, object htmlAttributes, bool isEditable = true, ApEnums.InputClass InputSize = ApEnums.InputClass.none)
        {
            MvcHtmlString html = default(MvcHtmlString);

            RouteValueDictionary routeValues = new RouteValueDictionary(htmlAttributes);

            if (!isEditable)
            {
                routeValues.Add("readonly", "true");
            }

            switch (InputSize)
            {
                case ApEnums.InputClass.none:
                    break;
                case ApEnums.InputClass.mini:
                    routeValues.Add("class", "input-mini");
                    break;
                case ApEnums.InputClass.small:
                    routeValues.Add("class", "input-small");
                    break;
                case ApEnums.InputClass.medium:
                    routeValues.Add("class", "input-medium");
                    break;
                case ApEnums.InputClass.large:
                    routeValues.Add("class", "input-large");
                    break;
                case ApEnums.InputClass.xlarge:
                    routeValues.Add("class", "input-xlarge");
                    break;
                case ApEnums.InputClass.xxlarge:
                    routeValues.Add("class", "input-xxlarge");
                    break;
                default:
                    break;
            }

            html = Html.InputExtensions.TextBoxFor(htmlHelper, expression, routeValues);

            return html;
        }

        public static MvcHtmlString ApDropDownList(this HtmlHelper htmlHelper, 
            string name, 
            IEnumerable<SelectListItemGroup> selectList, 
            string optionLabel = "", 
            object htmlAttributes = null,
            bool multiple = false, 
            int? size = null)
        {
            StringBuilder sb = new StringBuilder();
            // Ordenar por grupos y despues por abecedario
            selectList = selectList.OrderBy(u => u.Group).ThenBy(u => u.Text);

            var optionsSinGrupo = selectList.Where(u => string.IsNullOrWhiteSpace(u.Group));
            var grupos = selectList.Where(u => !string.IsNullOrWhiteSpace(u.Group)).Select(u => u.Group).Distinct();

            if (!String.IsNullOrWhiteSpace(optionLabel))
            {
                sb.AppendLine(ListItemToOption(new SelectListItemGroup() { Text = optionLabel, Value = "0" }));
            }

            foreach (var option in optionsSinGrupo)
            {
                sb.AppendLine(ListItemToOption(option));
            }
            
            StringBuilder sbGrupo = new StringBuilder();

            foreach (var grupo in grupos)
            {
                StringBuilder sbOption = new StringBuilder();

                var optionsEnGrupo = selectList.Where(u => u.Group == grupo);

                foreach (var option in optionsEnGrupo)
                {
                    sbOption.AppendLine(ListItemToOption(option));
                }

                TagBuilder tagGruop = new TagBuilder("optgroup")
                {
                    InnerHtml = ((object)sbOption).ToString()
                };

                if (!String.IsNullOrWhiteSpace(grupo))
                    tagGruop.Attributes["label"] = grupo;

                sb.AppendLine(tagGruop.ToString(TagRenderMode.Normal));
            }

            TagBuilder tagSelect = new TagBuilder("select")
            {
                InnerHtml = ((object)sb).ToString()
            };

            var dicHtmlAttributes = (IDictionary<string, object>)HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            if (dicHtmlAttributes.ContainsKey("class"))
            {
                dicHtmlAttributes["class"] += " chzn-select";
            }
            else
            {
                dicHtmlAttributes.Add("class", "chzn-select");
            }

            tagSelect.MergeAttributes<string, object>(dicHtmlAttributes);

            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            tagSelect.MergeAttribute("name", fullHtmlFieldName, true);

            tagSelect.GenerateId(fullHtmlFieldName);
            if (multiple)
                tagSelect.MergeAttribute("multiple", "multiple");

            if (size.HasValue)
                tagSelect.MergeAttribute("size", size.Value.ToString());

            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out modelState) && modelState.Errors.Count > 0)
                tagSelect.AddCssClass(HtmlHelper.ValidationInputCssClassName);

            ModelMetadata metadata = (ModelMetadata)null;
            tagSelect.MergeAttributes<string, object>(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

            return new MvcHtmlString(tagSelect.ToString(TagRenderMode.Normal));

            //sb.AppendFormat("<select{0}{1}{2}>", 
            //    (multiple ? " multiple='multiple'" : ""), 
            //    size.HasValue ? size.Value.ToString(" size='{0}'") : "",
            //    routeValues.ContainsKey("class") ? (" class='" + routeValues["class"].ToString() +"'") : "");
            //sb.Append(Environment.NewLine);

            //sb.AppendFormat("<option value='0'>{0}</option>",
            //    String.IsNullOrWhiteSpace(optionLabel) ? "" : optionLabel);
            //sb.Append(Environment.NewLine);

            //foreach (var item in selectList)
            //{
            //    if (hacerGrupos && (ultimoGrupo != item.Group && grupoAbierto))
            //    {
            //        sb.Append("</optgroup>");
            //        sb.Append(Environment.NewLine);
            //        grupoAbierto = false;
            //    }

            //    if (hacerGrupos && ultimoGrupo != item.Group)
            //    {
            //        sb.AppendFormat("<optgroup label='{0}'>",
            //            item.Group);
            //        sb.Append(Environment.NewLine);
            //        ultimoGrupo = item.Group;
            //        grupoAbierto = true;
            //    }

            //    sb.AppendFormat("<option{3}{4}{2}{5} value='{0}'>{1}</option>", 
            //        item.Value, 
            //        item.Text, 
            //        (item.Selected ? " selected='selected'" : ""),
            //        (item.Disabled ? " disabled='disabled'" : ""),
            //        (!string.IsNullOrWhiteSpace(item.Style) ? " style='" + item.Style + "'" : ""),
            //        (!string.IsNullOrWhiteSpace(item.Class) ? " class='" + item.Class + "'" : ""));
            //    sb.Append(Environment.NewLine);
            //}

            //sb.Append("</select>");
            //sb.Append(Environment.NewLine);

            //return new MvcHtmlString(sb.ToString());
        }

        private static string ListItemToOption(SelectListItemGroup item)
        {
            TagBuilder tagOption = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };

            if (item.Value != null)
                tagOption.Attributes["value"] = item.Value;

            if (item.Selected)
                tagOption.Attributes["selected"] = "selected";

            if (item.Disabled)
                tagOption.Attributes["disabled"] = "disabled";

            if (!String.IsNullOrWhiteSpace(item.Class))
                tagOption.Attributes["class"] = item.Class;

            if (!String.IsNullOrWhiteSpace(item.Style))
                tagOption.Attributes["style"] = item.Style;

            return tagOption.ToString(TagRenderMode.Normal);
        }
    }
}