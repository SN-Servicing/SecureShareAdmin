<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ViewUsers.aspx.cs" Inherits="SecureDataInterchange.Admin.Admin.ViewUsers" %>
<%@ Register src="../Common/UserControls/NavigationMenu.ascx" tagname="NavigationMenu" tagprefix="uc2" %>
<%@ Register src="../Common/UserControls/UserList.ascx" tagname="UserList" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<table width="758">
        <tr>
            <td style="border-bottom: 1px solid #660000;">
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="View Users" Font-Bold="True" Font-Size="Small"
                                ForeColor="#660000"></asp:Label>
                        </td>
                        <td align="right">
                            <uc2:NavigationMenu ID="NavigationMenu1" runat="server" />
                            </td>
                    </tr>
                </table>
            </td>
        </tr>
         <tr>
                <td class="details" valign="top" style="padding-left: 15px; padding-bottom: 10px;">
                    <uc3:UserList ID="ucUserList" runat="server" />
                </td>
        </tr>
    </table>


</asp:Content>
