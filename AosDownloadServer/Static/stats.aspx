<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Site.master" CodeBehind="stats.aspx.cs" Inherits="TestHttpSrv.Static.stats" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContent" runat="Server">
    <div class="container">
        <div class="row">
            <div class="col">
                <div class="panel panel-default">
                    <div class="panel-heading">Statistics:</div>
                    <div class="panel-body">
                        <asp:GridView ID="dgvStats" runat="server" Font-Names="Merriweather" Font-Size="8pt" CssClass="table table-hover table-striped" GridLines="None" AutoGenerateColumns="False">
                            <Columns>
                                <asp:BoundField DataField="Ip" HeaderText="Ip" />
                                <asp:BoundField DataField="Date" HeaderText="Date" />
                                <asp:BoundField DataField="Time" HeaderText="Time" />
                                <asp:BoundField DataField="Request" HeaderText="Request" />
                            </Columns>
                            <RowStyle CssClass="cursor-pointer" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
