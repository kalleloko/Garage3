using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

/// <summary>
/// Renders a dropdown select element.
/// Supports enums via asp-for, explicit enum type via items, or custom IEnumerable<SelectListItem>.
/// </summary>
namespace Garage3.TagHelpers
{
    [HtmlTargetElement("dropdown")]
    public class DropdownTagHelper : TagHelper
    {

        /// <summary>
        /// Injected automatically by ASP.NET Core. Provides access to ViewData, ModelState, etc.
        /// </summary>
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        /// <summary>
        /// Optional model binding for MVC scenarios.
        /// If set, the dropdown options can come from the enum type of the model property.
        /// Also determines the selected value from ModelState or Model.
        /// </summary>
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Items for the dropdown.
        /// Can be:
        /// 1. Type of an enum (e.g. typeof(VehicleType))  
        /// 2. IEnumerable<SelectListItem> (e.g. ViewBag.CustomItems)
        /// Ignored if asp-for is used and no manual override is needed.
        /// </summary>
        [HtmlAttributeName("items")]
        public object Items { get; set; }

        /// <summary>
        /// Optional. If true, sorts the dropdown options alphabetically by text.
        /// </summary>
        [HtmlAttributeName("sort")]
        public bool Sort { get; set; } = false;

        /// <summary>
        /// Optional. Manual override for the selected value.
        /// If set, this takes precedence over asp-for/ModelState.
        /// </summary>
        [HtmlAttributeName("selected")]
        public string? Selected { get; set; }

        /// <summary>
        /// Optional. Overrides the HTML 'name' attribute for the select.
        /// Required if asp-for is not used.
        /// </summary>
        [HtmlAttributeName("name")]
        public string? Name { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;

            // 1 bestäm name/id
            string name;
            if (For != null)
            {
                name = For.Name;
            }
            else if (!string.IsNullOrEmpty(Name))
            {
                name = Name;
            }
            else
            {
                throw new InvalidOperationException(
                    "Dropdown requires either 'asp-for' or 'name' attribute.");
            }

            output.Attributes.SetAttribute("name", name);
            output.Attributes.SetAttribute("id", name);

            // 2 rendera ev. child content (manuella <option>)
            var childContent = await output.GetChildContentAsync();
            output.Content.AppendHtml(childContent);

            // 3 bygg options
            var options = ResolveItems();

            // 4 bestäm selected-värde
            string? selectedValue = Selected;

            if (selectedValue == null && For != null && ViewContext != null)
            {
                selectedValue = ViewContext.ViewData.ModelState.TryGetValue(For.Name, out var entry)
                    ? entry.AttemptedValue
                    : For.Model?.ToString();
            }

            // 5️⃣ rendera alla options
            foreach (var item in options)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = item.Value;
                option.InnerHtml.Append(item.Text);

                if (!string.IsNullOrEmpty(selectedValue) && item.Value == selectedValue)
                {
                    option.Attributes["selected"] = "selected";
                }

                output.Content.AppendHtml(option);
            }
        }

        private List<SelectListItem> ResolveItems()
        {
            // 1️ Custom items vinner alltid
            if (Items is IEnumerable<SelectListItem> customList)
            {
                return Sort ? customList.OrderBy(i => i.Text).ToList() : customList.ToList();
            }

            // 2️ items är enum typ
            if (Items is Type enumType && enumType.IsEnum)
            {
                return EnumToSelectList(enumType);
            }

            // 3️ asp-for binder till enum
            if (For != null)
            {
                var modelType = For.ModelExplorer.ModelType;
                Type? enumType2 = modelType.IsEnum ? modelType : Nullable.GetUnderlyingType(modelType);
                if (enumType2 != null && enumType2.IsEnum)
                    return EnumToSelectList(enumType2);
            }

            throw new InvalidOperationException(
                "Dropdown requires either 'items' (IEnumerable<SelectListItem> or enum Type) or 'asp-for' bound to an enum.");
        }

        private List<SelectListItem> EnumToSelectList(Type enumType)
        {
            var items = Enum.GetValues(enumType)
                .Cast<object>()
                .Select(value =>
                {
                    var member = enumType.GetMember(value.ToString()).First();
                    var display = member.GetCustomAttribute<DisplayAttribute>();
                    return new SelectListItem
                    {
                        Value = value.ToString(),
                        Text = display?.Name ?? value.ToString()
                    };
                })
                .ToList();

            return Sort ? items.OrderBy(i => i.Text).ToList() : items;
        }

    }
}
