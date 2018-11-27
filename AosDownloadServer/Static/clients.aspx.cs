using AosP2PClient.DBase;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestHttpSrv.Static
{
    public partial class clients : Page
    {
        private BaseManager baseManager = MyMvcApplication.BaseManager;
        protected void Page_Load(object sender, EventArgs e)
        {
            var files = baseManager.GetAllFiles();
            if ( files.Count > 0)
            {
                dgvStats.DataSource = files;
                dgvStats.DataBind();
                dgvStats.UseAccessibleHeader = true;
                dgvStats.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}