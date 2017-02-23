using Sitecore.ExperienceEditor.Speak.Ribbon.Requests.ChangeLanguage;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Resources;
using Sitecore.Web.UI;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.ChangeLanguage
{

    public class LanguageIconLabelRequest :
        Sitecore.ExperienceEditor.Speak.Ribbon.Requests.ChangeLanguage.LanguageIconLabelRequest
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue()
            {
                Value = (object)new
                {
                    icon =
                    Images.GetThemedImageSource(
                        base.RequestContext.Item.Language.GetIcon(base.RequestContext.Item.Database),
                        ImageDimension.id24x24),
                    label = base.RequestContext.Item.Language.CultureInfo.DisplayName
                }
            };
        }
    }
}
