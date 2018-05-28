using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BankAPI;

namespace BankCustomControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ExamplePersonControl runat=server></{0}:ExamplePersonControl>")]
    public class ExamplePersonControl : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }

        public Person Person
        {
            /*get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }*/

            set
            {
                string Forename = value.Forename;
                string Surname = value.Surname;
                string Name = value.Name;
                string Id = value.Id.ToString();


                ViewState["Text"] = String.Format("Forename  \"{0}\" | Surname = \"{1}\" | Id = \"{2}\"",Forename, Surname, Id);
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write(Text);
        }
    }
}
