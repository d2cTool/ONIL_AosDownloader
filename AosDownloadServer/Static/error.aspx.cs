using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using TestHttpSrv.Logger;

namespace TestHttpSrv.Static
{
    public partial class error : Page
    {
        private LogEntryManager LogManager = MyMvcApplication.logEntryManager;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LogManager.Errors.Count > 0)
            {
                TotalErrorInfoGView.DataSource = LogManager.Errors.Values;
                TotalErrorInfoGView.DataBind();
                TotalErrorInfoGView.UseAccessibleHeader = true;
                TotalErrorInfoGView.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            else
            {
                TotalErrorInfoGView.DataSource = LogManager.Errors.Values;
                TotalErrorInfoGView.DataBind();
            }
        }
        protected void Select_Change(object sender, EventArgs e)
        {
            string ip = TotalErrorInfoGView.SelectedValue as string;
            if (LogManager.Errors[ip].DetailedInfos.Count > 0)
            {

                ErrorDetailGView.DataSource = LogManager.Errors[ip].DetailedInfos;
                ErrorDetailGView.DataBind();
                ErrorDetailGView.UseAccessibleHeader = true;
                ErrorDetailGView.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            else
            {
                ErrorDetailGView.DataSource = LogManager.Errors[ip].DetailedInfos;
                ErrorDetailGView.DataBind();
            }
        }
    }
}