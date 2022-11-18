using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace FlightPlanner.Service.TagHelpers
{
    [HtmlTargetElement(Attributes = IncludeAttributeName)]
    [HtmlTargetElement(Attributes = ExcludeAttributeName)]
    public class IncludeTagHelper : TagHelper
    {

        public const string IncludeAttributeName = "include-if";
        public const string ExcludeAttributeName = "exclude-if";

        public override int Order => int.MinValue;


        [HtmlAttributeName(IncludeAttributeName)]
        public bool? Include { get; set; } = true;

        [HtmlAttributeName(ExcludeAttributeName)]
        public bool Exclude { get; set; } = false;


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

            output.Attributes.RemoveAll(IncludeAttributeName);
            output.Attributes.RemoveAll(ExcludeAttributeName);

            if (!ShouldRender())
            {
                output.TagName = null;
                output.SuppressOutput();
            }
        }

        private bool ShouldRender()
        {
            return (!Include.HasValue || Include.Value) && !Exclude;
        }
    }
}
