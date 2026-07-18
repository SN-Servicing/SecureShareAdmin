<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EditZone.aspx.cs" Inherits="SecureDataInterchange.Admin.Admin.EditZone" %>
<%@ Register src="../Common/UserControls/NavigationMenu.ascx" tagname="NavigationMenu" tagprefix="uc1" %>
<%@ Register src="../Common/UserControls/EditUserZones.ascx" tagname="EditUserZones" tagprefix="uc2" %>
<%@ Register src="../Common/UserControls/UserList.ascx" tagname="UserList" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


<asp:Panel DefaultButton="btnSaveZone" runat="server" ID="pnlMain">
<table width="758" >
    <tr>
	    <td style="border-bottom: 1px solid #660000;">
	        <table width="100%">
	            <tr>
	                <td>
	                    <asp:Label ID="Label3" runat="server" Text="Edit Zone" Font-Bold="True" 
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
            <asp:GridView ID="gvAspNetUsers" runat="server"
                                BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" 
                                CellPadding="4"
                                EmptyDataText="No users found for this zone" 
                                AutoGenerateColumns="False" 
                DataKeyNames="ZonePermissionID" TabIndex="88" 
                onrowcommand="gvAspNetUsers_RowCommand">
                                <RowStyle BackColor="White" ForeColor="#330099" />
                                <Columns>
                                    <asp:TemplateField ShowHeader="False">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="LinkButton1"
                                                OnClientClick="return confirm('Are you sure you want to remove this users access to this zone?');"
                                                CommandArgument='<%# Eval("ZonePermissionID") %>'
                                             runat="server" CausesValidation="False" 
                                                CommandName="Remove" Text="Remove"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="UserName" HeaderText="User Name" />
                                    <asp:BoundField DataField="LastLoginDate" HeaderText="Last Login Date" />
                                    <asp:BoundField DataField="ZonePermissionID" Visible="false" />
                                </Columns>
                                <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                                <HeaderStyle BackColor="#660000" Font-Bold="True" ForeColor="#FFFFCC" />
                            </asp:GridView>
            </td>
    </tr>
    <tr>
        <td style="border-bottom: 1px solid #660000; padding-top: 10px;">
            <asp:Label ID="Label5" runat="server" Text="Add Users" Font-Bold="True"
                Font-Size="Small" ForeColor="#660000"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <uc3:UserList ID="ucUserList" runat="server" />
        </td>
    </tr>
    <tr>
        <td>
            &nbsp;</td>
    </tr>
    <tr>
        <td style="border-top:1px solid #660000;">
            <table width="100%">
                <tr>
                    <td>
                        <asp:HyperLink ID="hlBack" runat="server"><< Back</asp:HyperLink>
                    </td>
                    <td align="right">
                        <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="#3366FF" 
                            Visible="False"></asp:Label>
		            </td>
                    <td align="right">
                        <asp:Button ID="btnRemoveZone" runat="server" onclick="btnRemoveZone_Click" 
                            onclientclick="return confirm('Are you sure you want to delete this zone?');" 
                            TabIndex="87" Text="Delete Zone"  />
                    </td>
                    <td align="right" width="100">
                        <asp:Button runat="server" ID="btnSaveZone"
                        Text="Save Zone" onclick="btnSaveZone_Click" TabIndex="10"  />
		            </td>
                </tr>
            </table></td>
    </tr>
</table>
</asp:Panel>


</asp:Content>
