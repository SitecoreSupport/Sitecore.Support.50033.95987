using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak;
using Sitecore.ExperienceEditor.Speak.Ribbon.Controls.SelectLanguageButton;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;
using Sitecore.Resources;
using Sitecore.Support;
using Sitecore.Web;
using Sitecore.Web.UI;
using System;


namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Controls.SelectLanguageButton
{

    public class SelectLanguageButton : Sitecore.ExperienceEditor.Speak.Ribbon.Controls.SelectLanguageButton.SelectLanguageButton
    {
        public SelectLanguageButton(RenderingParametersResolver renderingParametersResolver) : base(renderingParametersResolver)
        {
        }

        protected override void InitializeControl(RenderingParametersResolver renderingParametersResolver)
        {
            Language language;
            base.InitializeControl(renderingParametersResolver);
            Language.TryParse(WebUtil.GetQueryString(Sitecore.ExperienceEditor.Speak.Constants.RequestParameters.Lang, string.Empty), out language);
            Assert.IsNotNull(language, "Could not resolve language.");
            Assert.IsNotNull(Factory.GetDatabase(WebUtil.GetQueryString(Sitecore.ExperienceEditor.Speak.Constants.RequestParameters.DatabaseName)), "Could not resolve database.");
            this.Label = language.CultureInfo.DisplayName;
            string icon = ExtendedLanguageService.GetIcon(language, Factory.GetDatabase(WebUtil.GetQueryString(Sitecore.ExperienceEditor.Speak.Constants.RequestParameters.DatabaseName)));
            this.IconPath = Images.GetThemedImageSource(icon, ImageDimension.id24x24);
        }
    }
}
