<%@ Page Title="" Language="C#" MasterPageFile="~/Common/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="SecureDataInterchange.Admin.Common.Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <table>
        <tr>
            <td style="padding:15px;">
                <table cellpadding="0" width="700px"  >
                    <tr>
                        <td style="padding-bottom:10px;"><asp:Label ID="lblError" runat="server" Text="An Error Has Occured" Font-Size="Medium" Font-Bold="True" Font-Underline="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td ><asp:Label ID="lblError0" runat="server" Text="The error has been logged and the appropriate parties will address the problem shortly. 
                        Use your browsers 'Back' button to return to your previous location.
                        <br /><br />If this problem continues, please contact TASC@snsc.com"></asp:Label>
                    
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
   
    

</asp:Content>
