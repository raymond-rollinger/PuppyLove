﻿<%@ Page Language="C#" Inherits="ProjectTemplate.uploadbutton" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title>uploadbutton</title>
</head>
<body>
	<form id="form1" runat="server">
        <fieldset>
            <legend>
                    Upload a Photo!
            </legend>
        <asp:FileUpload ID="Uploader" runat="server" accept="image/*" />
        <asp:Button ID="UploadSubmit" runat="server" OnClick="UploadSubmit_Click" Text="Upload" Height="40px" BackColor="White" BorderStyle="None" ForeColor="Blue" />
        </fieldset>
    <asp:Label ID="StatusLabel" runat="server" Font-Italic="True"></asp:Label>
        <br />
    </form>
</body>
</html>
