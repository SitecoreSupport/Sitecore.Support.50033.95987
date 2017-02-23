using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Shell.Framework.Commands.Ribbon;
using Sitecore.Support;
using System;

namespace Sitecore.Support.Shell.Framework.Commands.Ribbon
{

    [Serializable]
    public class Languages : Sitecore.Shell.Framework.Commands.Ribbon.Languages
    {
        public override string GetIcon(CommandContext context, string icon)
        {
            Assert.ArgumentNotNull(context, "context");
            Assert.ArgumentNotNullOrEmpty(icon, "icon");
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                string str = ExtendedLanguageService.GetIcon(item.Language, item.Database);
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            return base.GetIcon(context, icon);
        }
    }
}
