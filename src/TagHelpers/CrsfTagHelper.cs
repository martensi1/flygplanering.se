using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Service.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-crsf-token")]
    public class CrsfTagHelper : TagHelper
    {
        public override int Order => 0;


        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            string crsfToken = GetCrsfToken();

            AddAttribute(output, "type", "hidden");
            AddAttribute(output, "name", "__RequestVerificationToken");
            AddAttribute(output, "value", crsfToken);
        }

        private string GetCrsfToken()
        {
            var context = ViewContext.HttpContext;
            var antiforgery = (IAntiforgery)context.RequestServices
                .GetService(typeof(IAntiforgery));

            return antiforgery.GetAndStoreTokens(context).RequestToken;
        }

        private void AddAttribute(TagHelperOutput output, string name, string value)
        {
            output.Attributes.RemoveAll(name);
            output.Attributes.Add(name, value);
        }
    }
}
