<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.master" AutoEventWireup="true" CodeBehind="ViewZones.aspx.cs" Inherits="SecureDataInterchange.Admin.Admin.access.ViewSharedFiles" %>
<%@ Register src="../Common/UserControls/NavigationMenu.ascx" tagname="NavigationMenu" tagprefix="uc1" %>


<%@ Register src="../Common/UserControls/EditUserZones.ascx" tagname="EditUserZones" tagprefix="uc2" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<table width="758">

<tr>
    <td style="border-bottom:1px solid #660000;">
    
        <table width="100%">
            <tr>
                <td>
	                <asp:Label ID="Label3" runat="server" Text="View Zones" Font-Bold="True" 
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
<td class="details" valign="top" style="padding-left:15px;">

</td>

</tr>
<tr>
<td class="details" valign="top" style="padding-bottom:10px;">

    <uc2:EditUserZones ID="ucUserZones" runat="server" />
	
</td>

</tr></table>


</asp:Content>
