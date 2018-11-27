using AosP2PClient.DBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestHttpSrv.View
{
    public partial class MyWebForm : Page
    {
        private BaseManager baseManager = MyMvcApplication.BaseManager;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DirectoryInfo rootInfo = new DirectoryInfo(baseManager.GetPathToBase());
                TreeNode rootNode = new TreeNode
                {
                    Text = "base",
                    Value = "base",
                    Target = "_blank",
                    NavigateUrl = "~/"
                };
                PopulateTreeView(rootInfo, rootNode);
                BaseTreeView.Nodes.Add(rootNode);
            }
        }

        private void PopulateTreeView(DirectoryInfo dirInfo, TreeNode treeNode)
        {
            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                TreeNode directoryNode = new TreeNode
                {
                    Text = directory.Name,
                    Value = directory.FullName,
                    Expanded = false
                };

                treeNode.ChildNodes.Add(directoryNode);
                PopulateTreeView(directory, directoryNode);
            }

            foreach (FileInfo file in dirInfo.GetFiles("*.zip*", SearchOption.TopDirectoryOnly))
            {
                TreeNode fileNode = new TreeNode
                {
                    Text = file.Name,
                    Value = file.FullName,
                    Target = "_blank"//,
                    //NavigateUrl = (new Uri(Server.MapPath("~/"))).MakeRelativeUri(new Uri(file.FullName)).ToString()
                };
                treeNode.ChildNodes.Add(fileNode);
            }
        }

        protected void Select_Change(object sender, EventArgs e)
        {
            var fileName = FileAndPathManipulation.MakeRelativePath(baseManager.GetPathToBase(), BaseTreeView.SelectedNode.Value);
            var bFile = baseManager.GetFile(fileName);
            var clients = bFile?.GetClients();
            if (clients != null && clients.Count > 0)
            {
                dgvUsers.DataSource = clients;
                dgvUsers.DataBind();
                dgvUsers.UseAccessibleHeader = true;
                dgvUsers.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            else
            {
                dgvUsers.DataSource = new List<ClientInfo>();
                dgvUsers.DataBind();
            }
        }
    }
}