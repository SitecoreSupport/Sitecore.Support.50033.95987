using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell;
using Sitecore.Shell.Applications.ContentManager.Galleries;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XmlControls;
using System;
using System.Globalization;

namespace Sitecore.Support
{

    public class GalleryLanguagesForm : GalleryForm
    {
        protected Scrollbox Languages;
        protected GalleryMenu Options;

        private static Item GetCurrentItem()
        {
            string queryString = WebUtil.GetQueryString("db");
            string path = WebUtil.GetQueryString("id");
            Language language = Language.Parse(WebUtil.GetQueryString("la"));
            Sitecore.Data.Version version = Sitecore.Data.Version.Parse(WebUtil.GetQueryString("vs"));
            Database database = Factory.GetDatabase(queryString);
            Assert.IsNotNull(database, queryString);
            return database.GetItem(path, language, version);
        }

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (message.Name != "event:click")
            {
                base.Invoke(message, true);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                Item currentItem = GetCurrentItem();
                if (currentItem != null)
                {
                    using (new ThreadCultureSwitcher(Context.Language.CultureInfo))
                    {
                        foreach (Language language in currentItem.Languages)
                        {
                            string icon = string.Empty;
                            ID languageItemId = LanguageManager.GetLanguageItemId(language, currentItem.Database);
                            if (!ItemUtil.IsNull(languageItemId))
                            {
                                Item item2 = currentItem.Database.GetItem(languageItemId);
                                if (item2 != null)
                                {
                                    icon = item2["__icon"];
                                }
                                if (((item2 == null) || !item2.Access.CanRead()) || (item2.Appearance.Hidden && !UserOptions.View.ShowHiddenItems))
                                {
                                    continue;
                                }
                            }
                            XmlControl control = ControlFactory.GetControl("Gallery.Languages.Option") as XmlControl;
                            Assert.IsNotNull(control, typeof(XmlControl));
                            Context.ClientPage.AddControl(this.Languages, control);
                            Item item3 = currentItem.Database.GetItem(currentItem.ID, language);
                            if (item3 != null)
                            {
                                int length = item3.Versions.GetVersionNumbers(false).Length;

                                string str2 = (length == 1)
                                        ? Translate.Text("1 version.")
                                        : Translate.Text("{0} versions.", new object[] {length.ToString()});
                                CultureInfo cultureInfo = language.CultureInfo;
                                    control["Header"] = cultureInfo.DisplayName + " : " + cultureInfo.NativeName;
                                    control["Description"] = str2;
                                    control["Click"] = $"item:load(id={currentItem.ID},language={language},version=0)";
                                    LanguageDefinition languageDefinition = LanguageDefinitions.GetLanguageDefinition(language);
                                    if (languageDefinition != null)
                                    {
                                        icon = languageDefinition.Icon;
                                    }
                                    control["Icon"] = icon;
                                    if (language.Name.Equals(WebUtil.GetQueryString("la"), StringComparison.OrdinalIgnoreCase))
                                    {
                                        control["ClassName"] = "scMenuPanelItemSelected";
                                    }
                                    else
                                    {
                                        control["ClassName"] = "scMenuPanelItem";
                                    }
                                }
                            }
                        }
                        Item item = Client.CoreDatabase.GetItem("/sitecore/content/Applications/Content Editor/Menues/Languages");
                        if (item != null)
                        {
                            this.Options.AddFromDataSource(item, string.Empty);
                        }
                    }
                }
            }
        }
    }
