<%@ Page Language="C#" Inherits="ProjectTemplate.upload_button" %>
<!DOCTYPE html>
<html>
<head runat="server">
	<title>upload_button</title>
</head>
<body>
     <form id="form1" runat="server">
        <fieldset>
            <legend>
                Upload Profile Photo
            </legend>
        <asp:FileUpload ID="Uploader" runat="server" accept="image/*" />
        <asp:Button ID="UploadSubmit" runat="server" OnClick="UploadSubmit_Click" Text="Upload" Height="40px" BackColor="White" BorderStyle="None" ForeColor="Blue" />
        </fieldset>
        <asp:Label ID="StatusLabel" runat="server" Font-Italic="True"></asp:Label>
        <br />
        <asp:RadioButton ID="UserPhotoRadioBtn" runat="server" Checked="True" Text="User Photo" GroupName="radioBtnGroup" /><br>
        <asp:RadioButton ID="PetPhotoRadioBtn" runat="server" Checked="False" Text="Pet Photo" GroupName="radioBtnGroup" /><br>    
        <br />
       
        <asp:HyperLink ID="HomePageLink" runat="server" NavigateUrl="./AccountPage.html" BackColor="White" ForeColor="Blue" Target="_parent" Text="Back To Home Page" />
     </form>
</body>
</html>
