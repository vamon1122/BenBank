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
    
    [ToolboxData("<{0}:PersonTable runat=server></{0}:PersonTable>")]
    public class PersonTable : Table
    {
        Table MyTable = new Table();
        TableRow MyRow = new TableRow();
        TableCell MyCell0 = new TableCell();
        TableCell MyCell1 = new TableCell();

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        [field: NonSerializedAttribute()]
        public Table Text
        {
            get
            {

                /*String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);*/

                Table s = (Table)ViewState["MyTable"];
                return s;
            }

            set
            {
                ViewState["MyTable"] = value;
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


                Table PersonTable = new Table();
                TableRow MyRow = new TableRow();
                TableCell ForenameCell = new TableCell();
                TableCell SurnameCell = new TableCell();
                TableCell IdCell = new TableCell();

                ForenameCell.Text = Forename;
                SurnameCell.Text = Surname;
                IdCell.Text = Id;

                MyRow.Cells.Add(ForenameCell);
                MyRow.Cells.Add(SurnameCell);
                MyRow.Cells.Add(IdCell);
                
                PersonTable.Rows.Add(MyRow);

                



                ViewState["MyTable"] = PersonTable;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write(MyTable);
        }
    }
}
