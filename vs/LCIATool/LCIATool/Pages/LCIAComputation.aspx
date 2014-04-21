<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LCIAComputation.aspx.cs" Inherits="LCIATool.Pages.LCIAComputation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:DropDownList ID="ddlProcess" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProcess_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:DropDownList ID="ddlLCIAMethod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLCIAMethod_SelectedIndexChanged" >
        </asp:DropDownList>
        <br />
        <asp:GridView ID="gvLCIAComp" runat="server" EnableViewState="false"></asp:GridView>
    
    </div>
    </form>
</body>
</html>
