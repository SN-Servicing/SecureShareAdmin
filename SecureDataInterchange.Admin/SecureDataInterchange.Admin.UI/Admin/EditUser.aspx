<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.master" AutoEventWireup="true" CodeBehind="EditUser.aspx.cs" Inherits="SecureDataInterchange.Admin.Admin.access.EditUser" %>

<%@ Register Src="../Common/UserControls/NavigationMenu.ascx" TagName="NavigationMenu" TagPrefix="uc1" %>
<%@ Register Src="../Common/UserControls/EditUserZones.ascx" TagName="EditUserZones" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <table width="758">
        <tr>
            <td style="border-bottom: 1px solid #660000;">
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Edit User Information" Font-Bold="True"
                                Font-Size="Small" ForeColor="#660000" />
                        </td>
                        <td align="right">
                            <uc1:NavigationMenu ID="NavigationMenu1" runat="server" />
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
                <asp:Label ID="Label5" runat="server" Text="Main Account Information"
                    Font-Bold="True" ForeColor="#660000" Font-Size="10pt" Visible="False"></asp:Label>
            </td>
        </tr>

        <tr>
            <td class="details" valign="top">

                <table width="100%">
                    <tr>
                        <td valign="top">
                            <asp:DetailsView AutoGenerateRows="False" DataSourceID="MemberData"
                                ID="UserInfo" runat="server" OnItemUpdating="UserInfo_ItemUpdating"
                                BorderStyle="None" CellPadding="3">

                                <Fields>
                                    <asp:BoundField DataField="UserName" HeaderText="User Name" ReadOnly="True" HeaderStyle-CssClass="detailheader" ItemStyle-CssClass="detailitem">
                                        <HeaderStyle CssClass="detailheader"></HeaderStyle>

                                        <ItemStyle CssClass="detailitem"></ItemStyle>
                                    </asp:BoundField>

                                    <asp:CommandField ButtonType="button" ShowEditButton="false" EditText="Edit User Info" />
                                </Fields>
                            </asp:DetailsView>

                            <table>
                                <tr>
                                    <td>First Name:</td>
                                    <td>
                                        <asp:TextBox ID="firstname" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Last Name:</td>
                                    <td>
                                        <asp:TextBox ID="lastname" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Account Status</td>
                                    <td >
                                        <asp:DropDownList ID="DisabledReasonListBox" runat="server" >
                                            <asp:ListItem Text="Enabled" Value="0" />
                                            <asp:ListItem Text="Disabled" Value="3" />
                                            <asp:ListItem Text="Suspended Delinquent - do not enable without approval" Value="1" />
                                            <asp:ListItem Text="Disabled by system - account config issues" Value="2" />
                                            <asp:ListItem Text="Disabled by system - lack of activity" Value="4" />
                                        </asp:DropDownList>
                                        <%--<asp:CheckBox ID="enabledflag" runat="server" Text="Is Enabled" />--%></td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:CheckBox ID="loanlevel" runat="server" Text="Loan Access" /></td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:CheckBox ID="sdi" runat="server" Text="Shared Files Access" /></td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:CheckBox ID="requests" runat="server" Text="Requests" Visible="false" /></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="update" runat="server" OnClick="btnSaveEdit_Click" Text="Update User" /></td>
                                </tr>
                            </table>
                        </td>
                        <td align="right" valign="top">
                            <table>
                                <tr>
                                    <td style="padding-bottom: 5px;">
                                        <asp:DetailsView AutoGenerateRows="False" DataSourceID="MemberData"
                                            ID="UserInfo0" runat="server" OnItemUpdating="UserInfo_ItemUpdating"
                                            BorderStyle="None" CellPadding="3">

                                            <Fields>

                                                <asp:BoundField DataField="IsOnline" HeaderText="Is Online"
                                                    ReadOnly="True" HeaderStyle-CssClass="detailheader"
                                                    ItemStyle-CssClass="detailitem">
                                                    <HeaderStyle CssClass="detailheader"></HeaderStyle>

                                                    <ItemStyle CssClass="detailitem"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="CreationDate" HeaderText="Created" ReadOnly="True"
                                                    HeaderStyle-CssClass="detailheader" ItemStyle-CssClass="detailitem">
                                                    <HeaderStyle CssClass="detailheader"></HeaderStyle>

                                                    <ItemStyle CssClass="detailitem"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="LastActivityDate" HeaderText="Last activity"
                                                    ReadOnly="True" HeaderStyle-CssClass="detailheader"
                                                    ItemStyle-CssClass="detailitem">
                                                    <HeaderStyle CssClass="detailheader"></HeaderStyle>

                                                    <ItemStyle CssClass="detailitem"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="LastLoginDate" HeaderText="Last attempted login"
                                                    ReadOnly="True" HeaderStyle-CssClass="detailheader"
                                                    ItemStyle-CssClass="detailitem">
                                                    <HeaderStyle CssClass="detailheader"></HeaderStyle>

                                                    <ItemStyle CssClass="detailitem"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="LastLockoutDate" HeaderText="Last lockout"
                                                    ReadOnly="True" HeaderStyle-CssClass="detailheader"
                                                    ItemStyle-CssClass="detailitem">
                                                    <HeaderStyle CssClass="detailheader"></HeaderStyle>

                                                    <ItemStyle CssClass="detailitem"></ItemStyle>
                                                </asp:BoundField>
                                                <asp:BoundField DataField="LastPasswordChangedDate" HeaderText="Last password change"
                                                    ReadOnly="True" HeaderStyle-CssClass="detailheader"
                                                    ItemStyle-CssClass="detailitem">
                                                    <HeaderStyle CssClass="detailheader"></HeaderStyle>

                                                    <ItemStyle CssClass="detailitem"></ItemStyle>
                                                </asp:BoundField>
                                            </Fields>
                                        </asp:DetailsView>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td>

                                                    <%--                                <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password"
                                            OnClientClick="return confirm('Are you sure you want to reset this users password?')" onclick="btnResetPassword_Click"
                                             />--%>
                                                </td>
                                                <td>
                                                    <%--                                <asp:Button ID="btnDeleteUser" runat="server" Text="Delete User"
                                            OnClick="btnDeleteUser_Click"
                                            OnClientClick="return confirm('Are you sure you want to delete this user?')" />--%>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>

                <asp:ObjectDataSource ID="MemberData" runat="server" DataObjectTypeName="System.Web.Security.MembershipUser" SelectMethod="GetUser" UpdateMethod="UpdateUser" TypeName="System.Web.Security.Membership">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="username" QueryStringField="username" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
        </tr>

        <tr>
            <td class="details"
                style="border-bottom: 1px solid #660000; padding-bottom: 5px;">

                <asp:Label ID="Label6" runat="server" Font-Bold="True" Font-Size="10pt"
                    ForeColor="#660000" Text="Allowed Zones"></asp:Label>
            </td>
        </tr>

        <tr>
            <td class="details"
                style="border-bottom: 1px solid #660000; padding-bottom: 5px;">

                <asp:Panel runat="server" ID="pnlCurrentZones">
                    <table>
                        <tr>
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="gvCurrentZones" runat="server" AutoGenerateColumns="False"
                                    BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px"
                                    CellPadding="4"
                                    OnRowDataBound="gvCurrentZones_RowDataBound"
                                    EmptyDataText="No zones found for this user"
                                    OnRowCommand="gvCurrentZones_RowCommand">
                                    <RowStyle BackColor="White" ForeColor="#330099" />
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lbDelete" OnClientClick="return confirm('Are you sure you want to delete this permission?');" runat="server" CausesValidation="False"
                                                    CommandName="Remove" Text="Delete"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ZoneTypeName" HeaderText="Zone Type" SortExpression="ZoneTypeName" />
                                        <asp:BoundField DataField="Description" HeaderText="Description"
                                            SortExpression="Description" />
                                        <asp:TemplateField HeaderText="File Zone Details">
                                            <ItemTemplate>
                                                <asp:Panel ID="Panel1" runat="server" Width="300">
                                                    <asp:Label ID="lblPrimary" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrimaryFieldName") + ": " + DataBinder.Eval(Container.DataItem, "PrimaryIDValue")

                                                                  %> '></asp:Label>
                                                    <br />
                                                    <asp:Label ID="lblSecondary" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SecondaryFieldName") + ": " +
                                                                           DataBinder.Eval(Container.DataItem, "SecondaryIDValue")

                                                                  %> '></asp:Label>
                                                </asp:Panel>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="FileCount" HeaderText="File Count"
                                            SortExpression="FileCount" />

                                        <asp:TemplateField HeaderText="" SortExpression="NotificationOptIn" ItemStyle-HorizontalAlign="Center">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("NotificationOptIn") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:ImageButton CommandArgument="emailnotification" runat="server" ID="btnEmail"
                                                    ImageUrl="~/Common/images/email.png"
                                                    ToolTip="Click here to start/stop receiving email notifications when new files are uploaded to this zone." />
                                                <asp:Label ID="lblEmailNotification" runat="server" Visible="false" Text='<%# Eval("NotificationOptIn") %>'></asp:Label>
                                                <asp:Label ID="lblZonePermissionID" runat="server" Visible="false" Text='<%# Eval("ZonePermissionID") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                    <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                                    <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                                    <HeaderStyle BackColor="#660000" Font-Bold="True" ForeColor="#FFFFCC" />
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="padding-right: 5px;">
                                            <img src="../Common/images/email.png" />
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label2" runat="server" ForeColor="#660000"
                                                Text="= Receive email updates when new files are uploaded (by other users)."
                                                Font-Bold="False" Font-Size="X-Small" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding-right: 5px;">
                                            <img src="../Common/images/cross.png" />
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="Label4" runat="server" ForeColor="#660000"
                                                Text="= Do not receive email updates about files."
                                                Font-Bold="False" Font-Size="X-Small" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td align="left">
                                            <asp:Label ID="Label7" Visible="false" runat="server" ForeColor="#660000"
                                                Text="&nbsp;&nbsp;(click on the icon to change mode)"
                                                Font-Bold="False" Font-Size="X-Small"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>

                <asp:Panel runat="server" ID="pnlEditZones" Visible="false">

                    <uc2:EditUserZones ID="ucEditUserZones" runat="server" EnableViewState="True" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="details">
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Button ID="btnBack" runat="server" Text="<< Back" OnClick="btnBack_Click"
                                CausesValidation="False" />
                        </td>
                        <td align="right">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:Button ID="btnEditUser" runat="server" Text="Edit User Zones"
                                            OnClick="btnEditUser_Click" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel"
                                            OnClick="btnCancelEdit_Click" Visible="False" CausesValidation="False" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnSaveEdit" runat="server" Text="Update"
                                            OnClick="btnSaveEdit_Click" Visible="False" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>