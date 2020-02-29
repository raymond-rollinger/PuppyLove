using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace ProjectTemplate
{
    public partial class upload_button : System.Web.UI.Page
    {
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void UploadSubmit_Click(object sender, EventArgs e)
		{
			if (Uploader.HasFile)
			{
				try
				{
                    ProjectServices puppyLove = new ProjectServices();
                    string destinationProfile;
                                       
                    string filename = Path.GetFileName(Uploader.FileName);
					Uploader.SaveAs(Server.MapPath("~/") + "Image/" + filename);
                    //string[] textLines = File.ReadAllLines(Server.MapPath("~/") + "/Image/" + filename);

                    if (PetPhotoRadioBtn.Checked)
                    {
                        destinationProfile = puppyLove.UploadPhoto(filename, true);
                        
                    }
                    else
                    {
                        destinationProfile = puppyLove.UploadPhoto(filename, false);
                    }

                    StatusLabel.Text = $"Success! {filename} uploaded to {destinationProfile}'s profile.";
                    //StatusLabel.Text = "Success! First line of text: " + textLines[0];

				}
				catch (Exception ex)
				{
					StatusLabel.Text = "Error: " + ex.Message;
				}
			}
		}

	}
}
