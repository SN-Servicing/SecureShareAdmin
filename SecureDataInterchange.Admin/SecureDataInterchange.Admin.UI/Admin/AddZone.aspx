<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AddZone.aspx.cs" Inherits="SecureDataInterchange.Admin.Admin.AddZone" %>
<%@ Register src="../Common/UserControls/NavigationMenu.ascx" tagname="NavigationMenu" tagprefix="uc1" %>
<%@ Register src="../Common/UserControls/EditUserZones.ascx" tagname="EditUserZones" tagprefix="uc2" %>
<%@ Register src="../Common/UserControls/UserList.ascx" tagname="UserList" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<table width="758" >
    <tr>
	    <td style="border-bottom: 1px solid #660000;">
	        <table width="100%">
	            <tr>
	                <td>
	                    <asp:Label ID="Label3" runat="server" Text="Add New Zone" Font-Bold="True" 
                            Font-Size="Small" ForeColor="#660000"></asp:Label>
	                </td>
	                <td align="right">
                        <uc1:NavigationMenu ID="NavigationMenu1" runat="server" />
                    </td>
	            </tr>
	        </table>
    	                      

	    </td>
    </tr>
    <tr>
        <td class="details" >
        

            <uc2:EditUserZones ID="ucEditUserZones" runat="server" />
        </td>
        
    </tr>
    <tr>
        <td style="border-bottom: 1px solid #660000; padding-top: 10px;">
	                    <asp:Label ID="Label4" runat="server" Text="Zone Users" Font-Bold="True" 
                            Font-Size="Small" ForeColor="#660000"></asp:Label>
	                </td>
    </tr>
    <tr>
        <td>
            <uc3:UserList ID="ucUserList" runat="server" />
            
        </td>
    </tr>
    <tr>
        <td style="border-top:1px solid #660000;">
            <table width="100%">
                <tr>
                    <td>
                        <asp:Button ID="btnBack" runat="server" Text="&lt;&lt; Zone List" onclick="btnBack_Click" 
                                CausesValidation="False" ></asp:Button>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="#3366FF"
                            Visible="False"></asp:Label>
                        <asp:Button runat="server" ID="btnAddZone" onclick="btnAddUser_Click" 
                        Text="Add Zone"  />
		            </td>
                </tr>
            </table></td>
    </tr>
</table>



</asp:Content>
