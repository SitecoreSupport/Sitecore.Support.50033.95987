using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using System;

namespace Sitecore.Support
{
    public static class ExtendedLanguageService
    {
        public static string GetIcon(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return GetIcon(item.Language, item.Database);
        }

        public static string GetIcon(Language language, Database database)
        {
            Assert.ArgumentNotNull(language, "language");
            Assert.ArgumentNotNull(database, "database");
            string icon = language.GetIcon(database);
            if (string.IsNullOrEmpty(icon))
            {
                LanguageDefinition languageDefinition = LanguageDefinitions.GetLanguageDefinition(language);
                if (languageDefinition != null)
                {
                    icon = languageDefinition.Icon;
                }
            }
            if (string.IsNullOrEmpty(icon))
            {
                icon = "Flags/16x16/flag_generic.png";
            }
            return icon;
        }
    }
}
