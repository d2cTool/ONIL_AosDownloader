<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Site.master" CodeBehind="content.aspx.cs" Inherits="TestHttpSrv.View.MyWebForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContent" runat="Server">
    <div class="container">
        <div class="row">
            <div class="col">
                <div class="panel panel-default">
                    <div class="panel-heading">Base content:</div>
                    <div class="panel-body">
                        <asp:TreeView ID="BaseTreeView" runat="server" ImageSet="XPFileExplorer" ShowLines="true" OnSelectedNodeChanged="Select_Change" NodeIndent="25">
                            <NodeStyle BorderColor="GrayText" Font-Names="Merriweather" Font-Size="8pt" ForeColor="Black" HorizontalPadding="2px"/>
                            <ParentNodeStyle Font-Bold="False" />
                        </asp:TreeView>
                    </div>
                </div>
            </div>
            <div class="col">
                <div class="panel panel-default">
                    <div class="panel-heading">Clients info:</div>
                    <div class="panel-body">
                        <asp:GridView ID="dgvUsers" runat="server" CssClass="table table-hover table-striped" GridLines="None" 
                            Font-Names="Merriweather" Font-Size="8pt" AutoGenerateColumns="False">
                            <Columns>
                                <asp:BoundField DataField="Ip" HeaderText="Ip" />
                                <asp:BoundField DataField="Port" HeaderText="Port" />
                                <asp:BoundField DataField="RegistrationDate" HeaderText="Date" />
                                <asp:BoundField DataField="RegistrationTime" HeaderText="Time" />
                            </Columns>
                            <RowStyle CssClass="cursor-pointer" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
