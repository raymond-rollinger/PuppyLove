﻿using System;
using System.Web;
using System.Web.UI;
using System.IO;

namespace ProjectTemplate
{

	public partial class uploadbutton : System.Web.UI.Page
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
					string filename = Path.GetFileName(Uploader.FileName);
					Uploader.SaveAs(Server.MapPath("~/") + "Image/" + filename);
					string[] textLines = File.ReadAllLines(Server.MapPath("~/") + "/Image/" + filename);


					StatusLabel.Text = "Success! First line of text: " + textLines[0];



				}
				catch (Exception ex)
				{
					StatusLabel.Text = "Error: " + ex.Message;
				}
			}
		}
	}
}