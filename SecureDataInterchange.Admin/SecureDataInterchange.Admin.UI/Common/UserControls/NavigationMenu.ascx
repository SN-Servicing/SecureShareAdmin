<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationMenu.ascx.cs" Inherits="SecureDataInterchange.Admin.Admin.access.NavigationMenu" %>

<style type="text/css">
    tr.navMenu > td {
        padding: 0 0.5em;
        font-size: xx-small;
    }
</style>

<table cellpadding="0" cellspacing="0">
    <tr class="navMenu">
        <td>
            <a href="ViewUsers.aspx">Users</a>
        </td>
        <td>|</td>
        <td>
            <a href="adduser.aspx">Add User</a>
        </td>
        <td>|</td>
        <td>
            <a href="ViewZones.aspx">Zones</a>
        </td>
        <td>|</td>
        <td>
            <a href="addzone.aspx">Add New Zone</a>
        </td>
    </tr>
</table>
