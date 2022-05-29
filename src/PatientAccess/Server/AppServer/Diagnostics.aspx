<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Diagnostics.aspx.cs" Inherits="PatientAccess.Diag" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Patient Access :: Diagnostics</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="ServerStatus" runat="server" />
        <br />
        <hr />
        <asp:Literal ID="ServerInfo" runat="server" />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
