<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Options.aspx.cs" Inherits="SecureDataInterchange.Admin.Admin.Options1" %>
<%@ Register src="../Common/UserControls/NavigationMenu.ascx" tagname="NavigationMenu" tagprefix="uc1" %>
<%@ Register src="../Common/UserControls/EditUserZones.ascx" tagname="EditUserZones" tagprefix="uc2" %>



<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



    <table width="758">

<tr>
    <td style="border-bottom:1px solid #660000;">
    
        <table width="100%">
            <tr>
                <td>
	                <asp:Label ID="Label3" runat="server" Text="My Options" Font-Bold="True" 
                        Font-Size="Small" ForeColor="#660000"></asp:Label>
	                
                </td>
                <td align="right">
                    <uc1:navigationmenu ID="NavigationMenu1" runat="server" />
</td>
            </tr>
        </table>
    </td>
</tr>

<tr>
<td class="details" valign="top">

    <asp:Label ID="lblMessage" runat="server" Text="Label"></asp:Label>

    </td>

</tr>

<tr>
<td class="details" valign="top">
    <asp:Label ID="Label5" runat="server" Text="Email Notifications" 
        Font-Bold="True" ForeColor="#660000" Font-Size="10pt" Visible="True"></asp:Label>
</td>
 
</tr>

<tr>
<td class="details" valign="top">


</td>
</tr>

</table>










</asp:Content>
