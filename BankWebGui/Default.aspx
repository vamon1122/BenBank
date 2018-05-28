<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BankWebGui.Default" %>

<%@ Register Assembly="BankCustomControls" Namespace="BankCustomControls" TagPrefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <label>Bank Manager!</label>
            <br />
            <label>Update ExamplePersonControl</label>

            <div>
                <asp:TextBox runat="server" ID="TextBox_ExampPersCtrl"></asp:TextBox>
                <asp:Button runat="server" Text="Update" OnClick="Button_ExampPersCtrl_Click" />
            </div>
            <label>Update PersonTable </label>
            <div>
                <asp:TextBox runat="server" ID="TextBox_PersonTable"></asp:TextBox>
                <asp:Button runat="server" Text="Update" OnClick="Button_PersonTable_Click" />
            </div>

        </div>
    </form>
    <cc1:ExamplePersonControl ID="ExamplePersonControl1" runat="server" />
    <cc1:PersonTable ID="PersonTable2" runat="server"></cc1:PersonTable>
</body>

</html>
