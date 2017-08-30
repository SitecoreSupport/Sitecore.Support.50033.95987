using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Framework.Commands.Ribbon;
using Sitecore.Support;
using System;
using System.Globalization;

namespace Sitecore.Support.Shell.Framework.Commands.Ribbon
{

    [Serializable]
    public class Languages : Sitecore.Shell.Framework.Commands.Ribbon.Languages
    {
        public override string GetHeader(CommandContext context, string header)
        {
            Assert.ArgumentNotNull(context, "context");
            Assert.ArgumentNotNull(header, "header");
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                CultureInfo cultureInfo = item.Language.CultureInfo;
                if (cultureInfo.IsNeutralCulture)
                {
                    cultureInfo = Language.CreateSpecificCulture(cultureInfo.Name);
                }
                using (new ThreadCultureSwitcher(Context.Language.CultureInfo))
                {
                    string displayName = cultureInfo.DisplayName;
                    if (!string.IsNullOrEmpty(displayName))
                    {
                        return displayName;
                    }
                }
            }
            return base.GetIcon(context, header);
        }
    }
}
