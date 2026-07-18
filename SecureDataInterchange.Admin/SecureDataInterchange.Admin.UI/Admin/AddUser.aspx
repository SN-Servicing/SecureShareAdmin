<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.master" AutoEventWireup="true"
    CodeBehind="AddUser.aspx.cs" Inherits="SecureDataInterchange.Admin.Admin.access.AddUser" %>

<%@ Register Src="../Common/UserControls/NavigationMenu.ascx" TagName="NavigationMenu"
    TagPrefix="uc1" %>
<%@ Register Src="../Common/UserControls/EditUserZones.ascx" TagName="EditUserZones"
    TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <table width="758">
        <tr>
            <td style="border-bottom: 1px solid #660000;">
                <table width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Add User" Font-Bold="True"
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
            <td class="details">

                <table width="100%">


                    <tr>
                        <td colspan="1">
                            <table>

                                <tr>
                                    <td class="detailheader">Email
                                    </td>
                                    <td>
                                        <asp:TextBox ID="email" runat="server" Width="200px"></asp:TextBox>
                                        <asp:RequiredFieldValidator
                                            ID="RequiredFieldValidator1" runat="server"
                                            ErrorMessage="* Required" ControlToValidate="email"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                            ControlToValidate="email" ErrorMessage="Invalid Email"
                                            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="detailheader">
                            First name<asp:TextBox ID="firstname" runat="server" Width="200px"></asp:TextBox>
                            Last name<asp:TextBox ID="lastname" runat="server" Width="200px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="detailheader">Permissions
                            <asp:CheckBox ID="loanlevel" runat="server" Text="Loan Level" />
                            <asp:CheckBox ID="sdi" runat="server" Text="SDI" />
                            <asp:CheckBox ID="requests" runat="server" Text="Requests" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="1" style="border-bottom: 1px solid #660000; padding-top: 15px;">
                            <asp:Label ID="ConfirmationMessage1" runat="server" Text="User Zone Information"
                                ForeColor="#660000"></asp:Label>
                        </td>
                        <tr>
                            <td colspan="1">
                                <uc2:EditUserZones ID="ucEditUserZones" runat="server" />
                            </td>

                            <tr>
                                <td colspan="1" style="border-top: 1px solid #660000; padding-top: 5px;">
                                    <table width="100%" align="right">
                                        <tr>
                                            <td>
                                                <asp:Button ID="btnBack" runat="server" Text="&lt;&lt; User List" OnClick="btnBack_Click"
                                                    CausesValidation="False"></asp:Button>
                                            </td>
                                            <td align="center">
                                                <asp:Label ID="lblError" runat="server" ForeColor="#CC0000"></asp:Label>
                                            </td>
                                            <td align="right" valign="middle">
                                                <asp:Button runat="server" ID="btnAddUser" OnClick="btnAddUser_Click"
                                                    Text="Add User" />
                                                &nbsp;</td>
                                        </tr>
                                    </table>
                                    &nbsp;
                                </td>
                            </tr>
                </table>

                <asp:ObjectDataSource ID="MemberData" runat="server" DataObjectTypeName="System.Web.Security.MembershipUser"
                    SelectMethod="GetUser" UpdateMethod="UpdateUser" TypeName="System.Web.Security.Membership">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="username" QueryStringField="username" />
                    </SelectParameters>
                </asp:ObjectDataSource>

            </td>

        </tr>
    </table>

</asp:Content>
