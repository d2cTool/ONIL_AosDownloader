<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Site.master" CodeBehind="clients.aspx.cs" Inherits="TestHttpSrv.Static.clients" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContent" runat="Server">
    <div class="container">
        <div class="row">
            <div class="col">
                <div class="panel panel-default">
                    <div class="panel-heading">Clients:</div>
                    <div class="panel-body">
                        <asp:GridView ID="dgvStats" runat="server" Font-Names="Merriweather" Font-Size="8pt" 
                            CssClass="table table-hover table-striped" GridLines="None" AutoGenerateColumns="False">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                <asp:BoundField DataField="Size" HeaderText="Size (bytes)" />
                                <asp:BoundField DataField="Md5" HeaderText="Md5" />
                                <asp:BoundField DataField="ClientsCount" HeaderText="Clients" />
                            </Columns>
                            <RowStyle CssClass="cursor-pointer" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

