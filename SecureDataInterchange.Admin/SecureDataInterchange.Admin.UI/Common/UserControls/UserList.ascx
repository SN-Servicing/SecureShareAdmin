<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserList.ascx.cs"
    Inherits="SecureDataInterchange.Admin.Common.UserControls.UserList" %>

<table>
    <tr>
    
        <td  style="padding-top: 5px;" align="left">
                <fieldset style="width: 500px;" >
                <legend style="font-size: x-small">User Filter</legend>
                    <asp:Panel ID="pnlUserSearch" runat="server" DefaultButton="InitiateSearchButton">
                    <table align="left">
                        <tr>
                            <td align="left">
                            <div>
                                Enter all of part of a user's email address (a.k.a. user name) and click the &quot;Search&quot; button.
                            </div>
                            <div>
                                <asp:TextBox ID="userNameSearchTextBox" runat="server" Style="width: 30em" />
                                <asp:Button ID="InitiateSearchButton" OnClick="InitiateSearchButton_Click" runat="server" Text="Search" CausesValidation="False" />
                            </div>

                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                </fieldset> 
            </td>
        </tr>
        
        <tr>

        <td>
        <asp:GridView runat="server" ID="gvUsers" AutoGenerateColumns="False" CssClass="list"
                AlternatingRowStyle-CssClass="odd" BackColor="White" BorderColor="#CC9966" BorderStyle="None"
                BorderWidth="1px" CellPadding="3" EmptyDataText="No users found" OnRowDataBound="Users_RowDataBound"
                AllowPaging="True" PageSize="50" PagerSettings-PageButtonCount="10"
                PagerSettings-Mode="NumericFirstLast" OnPageIndexChanging="gvUsers_PageIndexChanging">
                        <RowStyle BackColor="White" ForeColor="#330099" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:Label ID="lblHideLabel" runat="server" ></asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox AutoPostBack="false" ID="chkInclude" Visible="true" runat="server" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox ID="CheckBox1" runat="server" />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <HeaderTemplate>User Name</HeaderTemplate>
                                <ItemTemplate>
                            <asp:HyperLink ID="hlUser" runat="server"
                                NavigateUrl='<%# "~/Admin/EditUser.aspx?userName=" + Eval("UserName") %>'
                                Text='<%# Eval("UserName") %>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:BoundField DataField="creationdate" HeaderText="Created" DataFormatString="{0:d}" />
                    <asp:BoundField DataField="Comment" HeaderText="Disabled?" />
                        </Columns>
                        <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" Font-Size="X-Small" />
                        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                        <HeaderStyle BackColor="#660000" Font-Bold="True" ForeColor="#FFFFCC" />
                        <AlternatingRowStyle CssClass="odd"></AlternatingRowStyle>
                    </asp:GridView>
            <asp:Label ID="lblNoUsers" runat="server" Text="No Users Found" Visible="False" Font-Names="Arial"
                Font-Size="Medium" ForeColor="#CC0000"></asp:Label>
        </td>
    </tr>
</table>






