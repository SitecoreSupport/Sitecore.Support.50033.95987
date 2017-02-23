using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.ExperienceEditor.Speak.Ribbon.Requests.ChangeLanguage;
using System;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.ChangeLanguage
{
    public class GetLanguages : Sitecore.ExperienceEditor.Speak.Ribbon.Requests.ChangeLanguage.GetLanguages
    {
        protected override string GetTitle(Item item) =>
            LanguageManager.GetLanguage(item.Name).CultureInfo.DisplayName;
    }
}
