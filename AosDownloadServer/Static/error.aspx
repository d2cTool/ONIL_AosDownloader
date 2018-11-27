<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Site.master" CodeBehind="error.aspx.cs" Inherits="TestHttpSrv.Static.error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyContent" runat="Server">
    <div class="container">
        <div class="row">
            <div class="col">
                <div class="panel panel-default">
                    <div class="panel-heading">Errors info:</div>
                    <div class="panel-body">
                        <asp:GridView ID="TotalErrorInfoGView" runat="server" CssClass="table table-hover table-striped" OnSelectedIndexChanged="Select_Change"
                            GridLines="None" Font-Names="Merriweather" Font-Size="8pt" AutoGenerateColumns="False" DataKeyNames="Ip" AutoGenerateSelectButton="True">
                            <Columns>
                                <asp:BoundField DataField="Ip" HeaderText="Ip" />
                                <asp:BoundField DataField="Count" HeaderText="Error count" />
                            </Columns>
                            <RowStyle CssClass="cursor-pointer" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <div class="col">
                <div class="panel panel-default">
                    <div class="panel-heading">Error details:</div>
                    <div class="panel-body">
                        <asp:GridView ID="ErrorDetailGView" runat="server" CssClass="table table-hover table-striped" 
                            GridLines="None" Font-Names="Merriweather" Font-Size="8pt" AutoGenerateColumns="False">
                            <Columns>
                                <asp:BoundField DataField="Date" HeaderText="Date" />
                                <asp:BoundField DataField="Time" HeaderText="Time" />
                                <asp:BoundField DataField="Level" HeaderText="Level" />
                                <asp:BoundField DataField="Message" HeaderText="Message" />
                                <asp:BoundField DataField="Callsite" HeaderText="Callsite" />
                                <asp:BoundField DataField="Exception" HeaderText="Exception" />
                            </Columns>
                            <RowStyle CssClass="cursor-pointer" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
