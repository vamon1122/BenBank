using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BankAPI;

namespace BankWebGui
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ExamplePersonControl1.Text = "Penis and balls";

            BankAPI.DataManager.LoadDb();

            //PersonTable1.Text = BankAPI.DataManager.People[0].Id.ToString();
            ExamplePersonControl1.Person = DataManager.People.Find(x => x.Id.ToString().Contains("af9a51c6-a385-47a9-a587-050178cda12d"));
            PersonTable2.Person = DataManager.People.Find(x => x.Id.ToString().Contains("af9a51c6-a385-47a9-a587-050178cda12d"));
        }

        protected void Button_ExampPersCtrl_Click(object sender, EventArgs e)
        {
            ExamplePersonControl1.Text = TextBox_ExampPersCtrl.Text;
        }

        protected void Button_PersonTable_Click(object sender, EventArgs e)
        {

        }
    }
}