using System;
using System.Web.UI.WebControls;
using TestHttpSrv.Stats;

namespace TestHttpSrv.Static
{
    public partial class stats : System.Web.UI.Page
    {
        private readonly LogEntryList logEntryList = MyMvcApplication.LogEntryList;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (logEntryList.sortedLogEntries.Count > 0)
            {
                dgvStats.DataSource = logEntryList.sortedLogEntries;
                dgvStats.DataBind();
                dgvStats.UseAccessibleHeader = true;
                dgvStats.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}