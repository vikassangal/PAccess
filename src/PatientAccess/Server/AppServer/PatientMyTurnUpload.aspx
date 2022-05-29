<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PatientMyTurnUpload.aspx.cs" Inherits="PatientAccess.PatientMyTurnUpload" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PAS</title>
    <style>
        .center {
            margin: auto;
            width: 60%;
            padding: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="center">
            <h2>Patient Bulk registration in Patient Access </h2>
            <br />
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button ID="bulkUpload" runat="server" Text="Register Patients" OnClick="bulkUpload_Click" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" />

            <br />
            <div>
                <asp:Label ID="lbl_msg" runat="server"></asp:Label>
            </div>
            <br />

            <div>
                <asp:GridView ID="GridView1" runat="server" AllowPaging="true">
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
