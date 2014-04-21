<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Import.aspx.cs" Inherits="DataImport.Import" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <h3>Import database data from XML.</h3>
<div>
    <table>
        <tr>
            <td>Select LCIA XML File : </td>
            <td>
                <asp:FileUpload ID="FileUpload1" runat="server" />
                </td>
            <td>
                <asp:Button ID="Button1" runat="server" Text="Import Data" OnClick="btnImport_Click" />
            </td>
        </tr>
    </table>
    <div>
        <br />
        <asp:Label ID="lblMessage" runat="server"  Font-Bold="true" />
        <br />
    
    </div>
</div>
        <div>
    <table>
        <tr>
            <td>Select Process XML File : </td>
            <td>
                <asp:FileUpload ID="uplProcess" runat="server" />
                </td>
            <td>
                <asp:Button ID="Button2" runat="server" Text="Import Data" OnClick="btnProcessImport_Click" />
            </td>
        </tr>
    </table>
    <div>
        <br />
        <asp:Label ID="lblProcessUploadError" runat="server"  Font-Bold="true" />
        <br />
    
    </div>
</div>
    </form>
</body>
</html>


<!DOCTYPE html>


