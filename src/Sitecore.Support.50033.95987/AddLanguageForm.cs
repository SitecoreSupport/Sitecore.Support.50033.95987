using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.IO;
using Sitecore.Resources;
using Sitecore.Web.UI;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Xml;

namespace Sitecore.Support.Shell.Applications.Globalization.AddLanguage
{
    public class AddLanguageForm : WizardForm
    {
        protected Edit Charset;
        protected Edit Codepage;
        protected Edit CustomCode;
        protected Edit Encoding;
        protected Edit Flag;
        protected Scrollbox Flags;
        protected Edit Language;
        protected Combobox Predefined;
        protected Edit Region;
        protected Edit Spellchecker;
        private const string THE_NAME_0_DOES_NOT_DENOTE_A_SPECIFIC_CULTURE_AS_DEFINED_BY_ASP_NET = "The name \"{0}\" does not denote a Specific Culture as defined by ASP.NET.";

        protected override bool ActivePageChanging(string page, ref string newpage)
        {
            Assert.ArgumentNotNull(page, "page");
            Assert.ArgumentNotNull(newpage, "newpage");
            if (newpage == "LastPage")
            {
                string str = this.Spellchecker.Value;
                if (!string.IsNullOrEmpty(str) && !FileUtil.Exists(FileUtil.MakePath("/sitecore/shell/controls/rich text editor/Dictionaries/", str)))
                {
                    SheerResponse.Alert("The spell checker dictionary does not exist.", new string[0]);
                    return false;
                }
                base.BackButton.Disabled = true;
                return this.CreateLanguage();
            }
            if (newpage == "EncodingPage")
            {
                if (this.Language.Value.Length == 0)
                {
                    SheerResponse.Alert("Specify a language identifier.", new string[0]);
                    return false;
                }
                string languageName = this.Language.Value;
                if (!string.IsNullOrEmpty(this.Region.Value))
                {
                    languageName = languageName + "-" + this.Region.Value;
                }
                if (!string.IsNullOrEmpty(this.CustomCode.Value))
                {
                    languageName = languageName + "-" + this.CustomCode.Value;
                }
                if (LanguageManager.IsLanguageNameDefined(Context.ContentDatabase, languageName))
                {
                    SheerResponse.Alert("The language is already defined in this database.", new string[0]);
                    return false;
                }
                CultureInfo info = null;
                bool flag = true;
                try
                {
                    info = Sitecore.Globalization.Language.CreateCultureInfo(languageName);
                }
                catch
                {
                    flag = false;
                }
                if (!flag)
                {
                    SheerResponse.Alert("The name \"{0}\" is not a valid or supported culture identifier.", new string[] { languageName });
                    return false;
                }
                if ((info != null) && info.IsNeutralCulture)
                {
                    try
                    {
                        flag = CultureInfo.CreateSpecificCulture(languageName) != null;
                    }
                    catch
                    {
                        flag = false;
                    }
                    if (!flag)
                    {
                        SheerResponse.Alert("The name \"{0}\" does not denote a Specific Culture as defined by ASP.NET.", new string[] { languageName });
                        return false;
                    }
                }
            }
            return base.ActivePageChanging(page, ref newpage);
        }

        private bool CreateLanguage()
        {
            try
            {
                TemplateItem template = Client.ContentDatabase.Templates[TemplateIDs.Language];
                Error.AssertTemplate(template, "Language");
                Item itemNotNull = Client.GetItemNotNull(ItemIDs.LanguageRoot);
                string name = this.Language.Value;
                if (!string.IsNullOrEmpty(this.Region.Value))
                {
                    name = name + "-" + this.Region.Value;
                }
                if (!string.IsNullOrEmpty(this.CustomCode.Value))
                {
                    name = name + "-" + this.CustomCode.Value;
                }
                Item item3 = itemNotNull.Add(name, template);
                if (item3 != null)
                {
                    item3.Editing.BeginEdit();
                    item3["Regional ISO Code"] = name;
                    item3["ISO"] = this.Language.Value;
                    item3["Code page"] = this.Codepage.Value;
                    item3["Encoding"] = this.Encoding.Value;
                    item3["Charset"] = this.Charset.Value;
                    item3["Dictionary"] = this.Spellchecker.Value;
                    item3.Appearance.Icon = this.Flag.Value;
                    item3.Editing.EndEdit();
                    Log.Audit(this, "Add language: {0}", new string[] { name });
                }
                else
                {
                    SheerResponse.ShowError("Failed to create the language.", string.Empty);
                    return false;
                }
            }
            catch (Exception exception)
            {
                SheerResponse.ShowError(exception);
                return false;
            }
            return true;
        }

        protected void Flag_Changed()
        {
            string image = StringUtil.GetString(new string[] { this.Flag.Value, "Flags/24x24/flag_generic.png" });
            SheerResponse.SetImageSrc("FlagImage", Images.GetThemedImageSource(image, ImageDimension.id48x48), 0x30, 0x30, string.Empty, string.Empty);
            SheerResponse.SetReturnValue(true);
        }

        protected void Flags_Changed(string src)
        {
            Assert.ArgumentNotNullOrEmpty(src, "src");
            this.Flag.Value = src;
            SheerResponse.SetImageSrc("FlagImage", Images.GetThemedImageSource(src, ImageDimension.id48x48), 0x30, 0x30, string.Empty, string.Empty);
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                this.RenderPredefined();
                this.RenderFlags();
            }
        }

        protected void Predefined_Changed()
        {
            int @int = MainUtil.GetInt(this.Predefined.Value, -1);
            if (@int == -1)
            {
                this.Language.Value = string.Empty;
                this.Region.Value = string.Empty;
                this.CustomCode.Value = string.Empty;
                this.Codepage.Value = string.Empty;
                this.Encoding.Value = string.Empty;
                this.Charset.Value = string.Empty;
                this.Spellchecker.Value = string.Empty;
                this.Flag.Value = string.Empty;
                this.Flag_Changed();
            }
            else
            {
                XmlNode configNode = Factory.GetConfigNode("languageDefinitions");
                XmlNode childNode = XmlUtil.GetChildNode("languages", configNode);
                XmlNode node = XmlUtil.GetChildNode(@int, childNode);
                if (node != null)
                {
                    this.Language.Value = XmlUtil.GetAttribute("id", node);
                    this.Region.Value = XmlUtil.GetAttribute("region", node);
                    this.CustomCode.Value = XmlUtil.GetAttribute("custom", node);
                    this.Codepage.Value = XmlUtil.GetAttribute("codepage", node);
                    this.Encoding.Value = XmlUtil.GetAttribute("encoding", node);
                    this.Charset.Value = XmlUtil.GetAttribute("charset", node);
                    this.Spellchecker.Value = XmlUtil.GetAttribute("spellchecker", node);
                    this.Flag.Value = XmlUtil.GetAttribute("icon", node);
                    this.Flag_Changed();
                }
            }
        }

        private void RenderFlags()
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());
            string prefix = "flags";
            string[] files = ZippedIcon.GetFiles(prefix, "/sitecore/shell/themes/standard/" + prefix + ".zip");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            foreach (string str2 in files)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(str2);
                if (fileNameWithoutExtension.StartsWith("flag_", StringComparison.InvariantCultureIgnoreCase))
                {
                    fileNameWithoutExtension = StringUtil.Mid(fileNameWithoutExtension, 5);
                }
                fileNameWithoutExtension = StringUtil.Capitalize(fileNameWithoutExtension.Replace("_", " "));
                string str4 = "Flags/24x24/" + str2;
                ImageBuilder builder = new ImageBuilder
                {
                    Src = str4,
                    Width = 0x18,
                    Height = 0x18,
                    Class = "scFlagIcon",
                    Alt = fileNameWithoutExtension
                };
                writer.Write("<a href=\"#\" class=\"scFlag\" onclick='javascript:return scForm.invoke(" + StringUtil.EscapeJavascriptString("Flags_Changed(\"" + str4 + "\")") + ")'>");
                writer.Write(builder.ToString());
                writer.Write("</a>");
            }
            this.Flags.InnerHtml = writer.InnerWriter.ToString();
        }

        private void RenderPredefined()
        {
            ListItem child = new ListItem();
            this.Predefined.Controls.Add(child);
            child.Header = string.Empty;
            child.Value = "-1";
            XmlNode configNode = Factory.GetConfigNode("languageDefinitions");
            XmlNode childNode = XmlUtil.GetChildNode("languages", configNode);
            List<XmlNode> childNodes = XmlUtil.GetChildNodes("language", childNode);
            if (childNodes != null)
            {
                for (int i = 0; i < childNodes.Count; i++)
                {
                    XmlNode node = childNodes[i];
                    string attribute = XmlUtil.GetAttribute("id", node);
                    string str2 = XmlUtil.GetAttribute("region", node);
                    string str3 = XmlUtil.GetAttribute("custom", node);
                    string name = attribute;
                    if (!string.IsNullOrEmpty(str2))
                    {
                        name = name + "-" + str2;
                    }
                    if (!string.IsNullOrEmpty(str3))
                    {
                        name = name + "-" + str3;
                    }
                    child = new ListItem();
                    this.Predefined.Controls.Add(child);
                    CultureInfo cultureInfo = Sitecore.Globalization.Language.CreateCultureInfo(name);
                    child.Header = Sitecore.Globalization.Language.GetDisplayName(cultureInfo);
                    child.Value = i.ToString();
                }
            }
        }
    }
}
