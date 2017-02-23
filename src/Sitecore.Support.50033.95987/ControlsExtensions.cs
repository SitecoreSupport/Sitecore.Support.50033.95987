
using Sitecore.Diagnostics;
using Sitecore.Mvc;
using Sitecore.Mvc.Presentation;
using System;
using System.Runtime.CompilerServices;
using System.Web;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Controls.SelectLanguageButton
{
    public static class ControlsExtensions
    {
        public static HtmlString SelectLanguageButton(this Mvc.Controls controls, Rendering rendering)
        {
            Assert.ArgumentNotNull(controls, "controls");
            Assert.ArgumentNotNull(rendering, "rendering");
            Sitecore.Support.ExperienceEditor.Speak.Ribbon.Controls.SelectLanguageButton.SelectLanguageButton button = new Sitecore.Support.ExperienceEditor.Speak.Ribbon.Controls.SelectLanguageButton.SelectLanguageButton(controls.GetParametersResolver(rendering));
            return new HtmlString(button.Render());
        }
    }
}
