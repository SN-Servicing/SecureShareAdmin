<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUserZones.ascx.cs" Inherits="SecureDataInterchange.Admin.Admin.access.EditUserZones" %>

<asp:Panel runat="server" ID="pnlEditZones" Visible="true">
    <table>
        <tr>
            <td>
                <asp:Label ID="lblSelectZoneType" runat="server" Text="Add New User Zone:"></asp:Label>
            </td>
            <td>
                <asp:RadioButtonList ID="rdoSelectZoneType" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rdoSelectZoneType_SelectedIndexChanged" AutoPostBack="True">
                    <asp:ListItem Selected="True">Current Zone</asp:ListItem>
                    <asp:ListItem>New Zone</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>

    <asp:MultiView ID="mvZone" runat="server">
        <asp:View runat="server" ID="mvvNewZone">
            <table>
                <tr>
                    <td><asp:Label ID="Label2" runat="server" Text="Zone Type:"></asp:Label></td>
                    <td>
                        <asp:DropDownList ID="ddlZoneType" runat="server" DataTextField="Name" DataValueField="FileSharingZoneTypeID" AutoPostBack="True" OnSelectedIndexChanged="ddlZoneType_SelectedIndexChanged"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblPrimaryText" runat="server" Text="Primary Value"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtPrimaryText" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPrimaryText" Display="Dynamic" ErrorMessage="* Required"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtPrimaryText" Display="Dynamic" ErrorMessage="Invalid (100 characters max)" ValidationExpression="(.|[\r\n]){1,100}"></asp:RegularExpressionValidator>
                        <asp:Label ID="lblPrimaryObjectIDError" runat="server" ForeColor="#CC0000" Visible="False"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblSecondaryText" runat="server" Text="Secondary Value"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtSecondaryText" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvSecondaryValue" runat="server" ControlToValidate="txtSecondaryText" Display="Dynamic" ErrorMessage="* Required"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revSecondaryValue" runat="server" ControlToValidate="txtSecondaryText" Display="Dynamic" ErrorMessage="Invalid (100 characters max)" ValidationExpression="(.|[\r\n]){1,100}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td><asp:Label ID="Label4" runat="server" Text="Description"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Width="400px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtDescription" Display="Dynamic" ErrorMessage="* Required"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtDescription" Display="Dynamic" ErrorMessage="Invalid Description (1000 characters max)" ValidationExpression="(.|[\r\n]){1,1000}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
            </table>
        </asp:View>

        <asp:View runat="server" ID="mvvAddCurrentZone">
            <table width="100%">
                <tr>
                    <td align="left">
                        <fieldset style="width: 700px">
                            <legend style="font-size: x-small">Zone Filter</legend>
                            <table align="left">
                                <tr>
                                    <td><asp:Label ID="Label5" runat="server" Text="Zone Type" Font-Size="X-Small"></asp:Label></td>
                                    <td align="left">
                                        <asp:DropDownList ID="ddlZoneTypeFilter" runat="server" AutoPostBack="True" DataTextField="Name" DataValueField="FileSharingZoneTypeID" OnSelectedIndexChanged="ddlZoneTypeFilter_SelectedIndexChanged" Font-Size="X-Small"></asp:DropDownList>
                                    </td>
                                    <td style="padding-left: 10px"><asp:Label ID="lblZoneSearch" runat="server" Font-Size="X-Small" Text="Search"></asp:Label></td>
                                    <td><asp:TextBox ID="txtZoneSearch" runat="server" Font-Size="X-Small" Width="220px" ToolTip="Search description and object IDs. Separate multiple terms with spaces."></asp:TextBox></td>
                                    <td style="padding-left: 10px" valign="bottom"><asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" Text="Search" Font-Size="X-Small" CausesValidation="False" /></td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="gvAvailableZones" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" CellPadding="3" OnRowDataBound="gvAvailableZones_RowDataBound" EmptyDataText="Enter search terms to find zones" OnRowCommand="gvAvailableZones_RowCommand">
                            <RowStyle BackColor="White" ForeColor="#330099" />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate><asp:CheckBox ID="chkInclude" runat="server" /></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ID" SortExpression="ID" Visible="False">
                                    <EditItemTemplate><asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("ZoneID") %>'></asp:TextBox></EditItemTemplate>
                                    <ItemTemplate><asp:Label ID="lblID" runat="server" Text='<%# Bind("ZoneID") %>'></asp:Label></ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ZoneTypeName" HeaderText="Zone Type" SortExpression="ZoneTypeName" />
                                <asp:TemplateField HeaderText="Description" SortExpression="Description" ControlStyle-Width="200px">
                                    <EditItemTemplate><asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Description") %>'></asp:TextBox></EditItemTemplate>
                                    <ItemTemplate><asp:HyperLink ID="hlDescription" runat="server" NavigateUrl="~/Common/UserControls/EditUserZones.ascx" Text='<%# Bind("Description") %>'>hc</asp:HyperLink></ItemTemplate>
                                    <ControlStyle Width="200px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="File Zone Details" SortExpression="PrimaryFieldName">
                                    <EditItemTemplate><asp:Label ID="Label1" runat="server" Text='<%# Eval("PrimaryFieldName") %>'></asp:Label></EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Panel ID="Panel1" runat="server" Width="275">
                                            <asp:Label ID="lblPrimary" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrimaryFieldName") + ": " + DataBinder.Eval(Container.DataItem, "PrimaryIDValue") %>'></asp:Label>
                                            <asp:Label ID="Label1" runat="server" Text="&nbsp;&nbsp;&nbsp;  "></asp:Label>
                                            <asp:Label ID="lblSecondary" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "SecondaryFieldName") + ": " + DataBinder.Eval(Container.DataItem, "SecondaryIDValue") %>'></asp:Label>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="FileCount" HeaderText="File Count" SortExpression="FileCount" />
                                <asp:TemplateField HeaderText="" SortExpression="NotificationOptIn" ItemStyle-HorizontalAlign="Center">
                                    <EditItemTemplate><asp:Label ID="Label1" runat="server" Text='<%# Eval("NotificationOptIn") %>'></asp:Label></EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:ImageButton CommandArgument="emailnotification" runat="server" ID="btnEmail" ImageUrl="~/Common/images/Email.png" ToolTip="Click here to start/stop receiving email notifications when new files are uploaded to this zone." />
                                        <asp:Label ID="lblEmailNotification" runat="server" Visible="false" Text='<%# Eval("NotificationOptIn") %>'></asp:Label>
                                        <asp:Label ID="lblZoneID" runat="server" Visible="false" Text='<%# Eval("ZoneID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                            <HeaderStyle BackColor="#660000" Font-Bold="True" ForeColor="#FFFFCC" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Panel ID="pnlLegend" runat="server">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="padding-right: 5px;"><img src="../Common/images/Email.png" /></td>
                                    <td style="padding-right:10px;"><asp:LinkButton ID="btnYesMail0" runat="server" OnClick="btnYesMail_Click" Text="Enable all" Font-Size="XX-Small" /></td>
                                    <td style="padding-right:5px;"><img src="../Common/images/cross.png" /></td>
                                    <td><asp:LinkButton ID="btnNoMail0" runat="server" OnClick="btnNoMail_Click" Text="Disable all" Font-Size="XX-Small" /></td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
</asp:Panel>
