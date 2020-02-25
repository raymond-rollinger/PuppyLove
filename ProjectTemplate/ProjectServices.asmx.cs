using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace ProjectTemplate
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class ProjectServices : System.Web.Services.WebService
	{
		////////////////////////////////////////////////////////////////////////
		///replace the values of these variables with your database credentials
		////////////////////////////////////////////////////////////////////////
		private string dbID = "ciscapstoners";
		private string dbPass = "!!Ciscapstoners";
		private string dbName = "ciscapstoners";
		////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////
		///call this method anywhere that you need the connection string!
		////////////////////////////////////////////////////////////////////////
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
		}

        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public bool LogOn(string uName, string pass)
        {
            //we return this flag to tell them if they logged in or not
            
            bool success = false;
            
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            string sqlSelect = "SELECT accountID, userName, IsAdmin FROM accounts WHERE userName=@userValue and password=@passValue";

            //set up our connection object to be ready to use our connection string
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            //set up our command object to use our connection, and our query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@userValue", HttpUtility.UrlDecode(uName));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(pass));

            //a data adapter acts like a bridge between our command object and 
            //the data we are trying to get back and put in a table object
            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
            //here's the table we want to fill with the results from our query
            DataTable sqlDt = new DataTable();
            //here we go filling it!
            sqlDa.Fill(sqlDt);
            //check to see if any rows were returned.  If they were, it means it's 
            //a legit account
            if (sqlDt.Rows.Count > 0)
            {
                //if we found an account, store the id and admin status in the session
                //so we can check those values later on other method calls to see if they 
                //are 1) logged in at all, and 2) and admin or not
                Session["userName"] = sqlDt.Rows[0]["userName"];
                Session["IsAdmin"] = sqlDt.Rows[0]["IsAdmin"];
                Session["accountID"] = sqlDt.Rows[0]["accountID"];

                success = true;
            }
            //return the result!
            return success;
        }

        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public void SignUp(string userName, string password, string firstName, string lastName, string email)
        {
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            string sqlAddAcct = "INSERT INTO accounts(userName, firstName, lastName, email, password) VALUES(@userValue, @firstNameValue, @lastNameValue, @emailValue, @passValue)";
            //"SELECT userName, password FROM accounts WHERE userName=@idValue and password=@passValue";
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlAddAcct, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@userValue", HttpUtility.UrlDecode(userName));
            sqlCommand.Parameters.AddWithValue("@firstNameValue", HttpUtility.UrlDecode(firstName));
            sqlCommand.Parameters.AddWithValue("@lastNameValue", HttpUtility.UrlDecode(lastName));
            sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
            sqlCommand.Parameters.AddWithValue("@passValue", HttpUtility.UrlDecode(password));

            sqlConnection.Open();

            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (Exception)
            {

            }
            sqlConnection.Close();
            //return the result!
        }

        [WebMethod(EnableSession = true)]
		public bool LogOff()
		{
			//if they log off, then we remove the session.  That way, if they access
			//again later they have to log back on in order for their ID to be back
			//in the session!
			Session.Abandon();
            return true;

		}

		/////////////////////////////////////////////////////////////////////////
		//don't forget to include this decoration above each method that you want
		//to be exposed as a web service!
		[WebMethod(EnableSession = true)]
		/////////////////////////////////////////////////////////////////////////
		public string TestConnection()
		{
			try
			{
				string testQuery = "select * from accounts";

				////////////////////////////////////////////////////////////////////////
				///here's an example of using the getConString method!
				////////////////////////////////////////////////////////////////////////
				MySqlConnection con = new MySqlConnection(getConString());
				////////////////////////////////////////////////////////////////////////

				MySqlCommand cmd = new MySqlCommand(testQuery, con);
				MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
				DataTable table = new DataTable();
				adapter.Fill(table);
				return "Success!";
			}
			catch (Exception e)
			{
				return "Something went wrong, please check your credentials and db name and try again.  Error: "+e.Message;
			}
		}

        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public void AccountInfo(string bio, string city)
        {
            
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            //string sqlAddAcct = "INSERT INTO accounts(bio, city) values(@bioValue, @cityValue) (SELECT userName=@userValue");
            string sqlEditAcct = "UPDATE accounts SET bio=@bioValue,city=@cityValue WHERE accountID=@userIdValue";
            //string sqlEditAcct = "UPDATE accounts ('bio', 'city') VALUES (@bioValue, @cityValue) WHERE userId=@userValue";
            //string sqlEditAcct = "UPDATE accounts SET city='hi' WHERE user='Catlover99'";
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlEditAcct, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
			sqlCommand.Parameters.AddWithValue("@userIdValue", HttpUtility.UrlDecode(Session["accountID"].ToString()));
            sqlCommand.Parameters.AddWithValue("@bioValue", HttpUtility.UrlDecode(bio));
            sqlCommand.Parameters.AddWithValue("@cityValue", HttpUtility.UrlDecode(city));
            sqlConnection.Open();

            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (Exception)
            {
            }
            sqlConnection.Close();
            //return the result!
        }

        [WebMethod(EnableSession = true)]
        public void ProfileInfo(string petName, string breed, string gender, string age, string bio, string userName)
        {
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            //string sqlAddAcct = "INSERT INTO accounts(bio, city) values(@bioValue, @cityValue) (SELECT userName=@userValue");
            string sqlEditProfile = "UPDATE profiles SET petName=@petNameValue,breed=@breedValue, gender=@genderValue, age=@ageValue, bio=@bioValue WHERE accountID=@userIdValue";

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlEditProfile, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@userIdValue", Session["accountID"]);
            sqlCommand.Parameters.AddWithValue("@petNameValue", HttpUtility.UrlDecode(petName));
            sqlCommand.Parameters.AddWithValue("@breedValue", HttpUtility.UrlDecode(breed));
            sqlCommand.Parameters.AddWithValue("@genderValue", HttpUtility.UrlDecode(gender));
            sqlCommand.Parameters.AddWithValue("@ageValue", HttpUtility.UrlDecode(age));
            sqlCommand.Parameters.AddWithValue("@bioValue", HttpUtility.UrlDecode(bio));
            sqlConnection.Open();
        }
	}
}
