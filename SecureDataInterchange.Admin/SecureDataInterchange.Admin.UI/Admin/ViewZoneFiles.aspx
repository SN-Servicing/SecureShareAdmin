<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.master" AutoEventWireup="true" CodeBehind="ViewZoneFiles.aspx.cs" Inherits="SecureDataInterchange.Admin.Admin.access.ViewZoneFiles" EnableEventValidation="false" %>
<%@ Register src="../Common/UserControls/NavigationMenu.ascx" tagname="NavigationMenu" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<table width="758">

<tr>
    <td style="border-bottom:1px solid #660000;">
        <table width="100%">
            <tr>
                <td>
	                <asp:Label ID="Label3" runat="server" Text="View Shared Files" Font-Bold="True" 
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

    <table width="100%">
        <tr>
            <td><asp:Label ID="lblZone" runat="server" Text="Label"></asp:Label></td>
            <td style="padding-left:10px;">
                <asp:HyperLink ID="hlEditZone" runat="server">Edit Zone</asp:HyperLink>
            </td>
        </tr>
    </table>

</td>
</tr>
<tr>
<td class="details" valign="top">

    <asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="False" 
        BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" 
        CellPadding="4"
        EmptyDataText="No files available." onrowdatabound="gvFiles_RowDataBound" 
        Font-Size="8pt">
        <RowStyle BackColor="White" ForeColor="#330099" />
        <Columns>

            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ToolTip="Delete file" ImageUrl="~/Common/images/Trash2.png" ID="lbDelete" runat="server" CausesValidation="False" 
                      OnClientClick="return confirm('Are you sure you want to delete this file?');" OnClick="ibTrash_Click" CommandArgument=<%# Eval("ID") %> CommandName="Delete" Text="Delete"></asp:ImageButton>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="File Name">
                <ItemTemplate>
                    <asp:Label ID="lblFileName" runat="server" Text='<%# Eval("Name") %>'></asp:Label><br />
                    <asp:Label ID="lblDescription" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" 
                SortExpression="Name" Visible="False" ItemStyle-Width="100">
                <ItemStyle Width="100px"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Description" HeaderText="Description" ReadOnly="True" SortExpression="Description" Visible="False" />
            <asp:BoundField DataField="DiskFileName" HeaderText="DiskFileName" ReadOnly="True" SortExpression="DiskFileName" Visible="False" />
            <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" SortExpression="ID" Visible="False" />
            <asp:BoundField DataField="UploadedBy" HeaderText="Uploaded By" ReadOnly="True" SortExpression="UploadedBy" />
            <asp:BoundField DataField="ZoneID" HeaderText="ZoneID" ReadOnly="True" SortExpression="ZoneID" Visible="False" />
            <asp:BoundField DataField="CreatedOnUTC" HeaderText="Uploaded On" ReadOnly="True" SortExpression="CreatedOnUTC" Visible="True" ItemStyle-Width="100" />
            <asp:BoundField DataField="FileSizeBytes" HeaderText="File Size (in kb)" ReadOnly="True" SortExpression="FileSizeBytes" ItemStyle-Width="75" Visible="True" DataFormatString="{0:#,##.#}" />

        </Columns>
        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
        <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
        <HeaderStyle BackColor="#660000" Font-Bold="True" ForeColor="#FFFFCC" />
    </asp:GridView>

</td>
</tr>
<tr>
<td class="details" valign="top">
    <asp:Button ID="btnBack" runat="server" Text="<< Back" CausesValidation="False" 
        onclick="btnBack_Click" />
</td>
</tr>
</table>

</asp:Content>
