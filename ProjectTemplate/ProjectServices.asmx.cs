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

        public string tmpId = "";
		////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////
		///call this method anywhere that you need the connection string!
		////////////////////////////////////////////////////////////////////////
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
		}

        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public string LogOn(string uName, string pass)
        {
            //we return this flag to tell them if they logged in or not
            string account = ""; 
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session

            string sqlSelect = "SELECT accountID, userName, firstName, lastName, email, bio, isAdmin, city, imageFileName FROM accounts WHERE userName=@userValue and password=@passValue";


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
                Session["accountID"] = sqlDt.Rows[0]["accountID"];
                Session["firstName"] = sqlDt.Rows[0]["firstName"];
                Session["lastName"] = sqlDt.Rows[0]["lastName"];
                Session["email"] = sqlDt.Rows[0]["email"];
                Session["bio"] = sqlDt.Rows[0]["bio"];
                Session["isAdmin"] = sqlDt.Rows[0]["isAdmin"];
                Session["city"] = sqlDt.Rows[0]["city"];
                Session["imageFileName"] = sqlDt.Rows[0]["imageFileName"];
                account = "{" + "\"userName\"" + ":" + "\"" + Session["userName"].ToString() + "\"" + "," 
                    + "\"accountID\"" + ":" + "\"" + Session["accountID"] + "\"" + "," 
                    + "\"firstName\"" + ":" + "\"" + Session["firstName"].ToString() + "\"" + "," 
                    + "\"lastName\"" + ":" + "\"" + Session["lastName"].ToString() + "\"" + "," 
                    + "\"email\"" + ":" + "\"" + Session["email"].ToString() + "\"" + ","
                    + "\"bio\"" + ":" + "\"" + Session["bio"].ToString() + "\"" + ","
                    + "\"isAdmin\"" + ":" + "\"" + Session["isAdmin"] + "\"" + ","
                    + "\"city\"" + ":" + "\"" + Session["city"].ToString() + "\"" + ","
                    +"\"imageFileName\"" + ":" + "\"" + Session["imageFileName"].ToString() + "\"" + "}";
                
            }
            //return the result!
            return account;
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
        public void ProfileInfo(string petName, string breed, string gender, string age, string bio)
        {
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            //string sqlAddAcct = "INSERT INTO accounts(bio, city) values(@bioValue, @cityValue) (SELECT userName=@userValue");
            string sqlEditProfile = "UPDATE profiles SET petName=@petNameValue,breed=@breedValue, gender=@genderValue, age=@ageValue, bio=@bioValue WHERE accountId=@userIdValue";


            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlEditProfile, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)

            sqlCommand.Parameters.AddWithValue("@userIdValue", HttpUtility.UrlDecode(Session["accountID"].ToString()));
            sqlCommand.Parameters.AddWithValue("@petNameValue", HttpUtility.UrlDecode(petName));
            sqlCommand.Parameters.AddWithValue("@breedValue", HttpUtility.UrlDecode(breed));
            sqlCommand.Parameters.AddWithValue("@genderValue", HttpUtility.UrlDecode(gender));
            sqlCommand.Parameters.AddWithValue("@ageValue", HttpUtility.UrlDecode(age));
            sqlCommand.Parameters.AddWithValue("@bioValue", HttpUtility.UrlDecode(bio));
            sqlConnection.Open();
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (Exception)
            {
                
            }
            sqlConnection.Close();
        }

        [WebMethod(EnableSession = true)]
        public string UploadPhoto(string filename, bool pet)
        {
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            string sqlEditProfile;
            string sqlGetProfileFirstName;
            string name = "";

            if (pet)
            {
                sqlEditProfile = "UPDATE profiles SET imageFileName=@filename WHERE accountID=@userIdValue";
                sqlGetProfileFirstName = "SELECT petName FROM profiles WHERE accountID=@userIdValue";
            }

            else
            {
                sqlEditProfile = "UPDATE accounts SET imageFileName=@filename WHERE accountID=@userIdValue";
                sqlGetProfileFirstName = "SELECT firstName FROM accounts WHERE accountID=@userIdValue";
            }

            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand1 = new MySqlCommand(sqlEditProfile, sqlConnection);
            MySqlCommand sqlCommand2 = new MySqlCommand(sqlGetProfileFirstName, sqlConnection);
            sqlCommand1.Parameters.AddWithValue("@userIdValue", HttpUtility.UrlDecode(Session["accountID"].ToString()));
            sqlCommand1.Parameters.AddWithValue("@filename", HttpUtility.UrlDecode(filename));
            sqlCommand2.Parameters.AddWithValue("@userIdValue", HttpUtility.UrlDecode(Session["accountID"].ToString()));
            sqlConnection.Open();
            try
            {
                int accountID = Convert.ToInt32(sqlCommand1.ExecuteScalar());
                MySqlDataReader dr = sqlCommand2.ExecuteReader();
                if(dr.HasRows)
                {
                    dr.Read();
                    name = dr.GetString(0);
                }
                dr.Close();
            }
            catch (Exception)
            {
            }
            sqlConnection.Close();

            return name;
            
        }

        public void AddPet(string petName, string breed, string gender, string age, string bio)
        {
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            //string sqlAddAcct = "INSERT INTO accounts(bio, city) values(@bioValue, @cityValue) (SELECT userName=@userValue");
            string sqlAddPet = "INSERT INTO profiles (petName, breed, gender, age, bio, accountId) VALUES (@petNameValue, @breedValue, @genderValue, @ageValue, @bioValue, @accountIdValue)";


            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            MySqlCommand sqlCommand = new MySqlCommand(sqlAddPet, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            /*
            sqlCommand.Parameters.AddWithValue("@petNameValue", HttpUtility.UrlDecode(petName));
            sqlCommand.Parameters.AddWithValue("@breedValue", HttpUtility.UrlDecode(breed));
            sqlCommand.Parameters.AddWithValue("@genderValue", HttpUtility.UrlDecode(gender));
            sqlCommand.Parameters.AddWithValue("@ageValue", HttpUtility.UrlDecode(age));
            sqlCommand.Parameters.AddWithValue("@bioValue", HttpUtility.UrlDecode(bio));
            sqlCommand.Parameters.AddWithValue("@accountIdValue", 1);
            */

            sqlCommand.Parameters.AddWithValue("@petNameValue", HttpUtility.UrlDecode(petName));
            sqlCommand.Parameters.AddWithValue("@breedValue", HttpUtility.UrlDecode(breed));
            sqlCommand.Parameters.AddWithValue("@genderValue", HttpUtility.UrlDecode(gender));
            sqlCommand.Parameters.AddWithValue("@ageValue", HttpUtility.UrlDecode(age));
            sqlCommand.Parameters.AddWithValue("@bioValue", HttpUtility.UrlDecode(bio));
            sqlCommand.Parameters.AddWithValue("@accountIdValue", HttpUtility.UrlDecode(Session["accountID"].ToString()));
            sqlConnection.Open();
            try
            {
                int accountID = Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (Exception e)
            {
                throw e;
            }
            sqlConnection.Close();
        }

            public string RandomId() {

            Random rnd = new Random();
            int rndNum = rnd.Next(1, 11);  // chooses a number between 1 and 10
            tmpId = rndNum.ToString();

            return tmpId;
        }

        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public string Randompet(string accountId)
        {
            //we return this flag to tell them if they logged in or not
            string petinfo = "";
            
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session

            accountId = RandomId();

            string sqlSelect = "SELECT profileid, petName, accountid, breed, gender, age, bio, imageFileName FROM profiles WHERE accountid=@userValue";


            //set up our connection object to be ready to use our connection string
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            //set up our command object to use our connection, and our query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@userValue", HttpUtility.UrlDecode(accountId));

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
                Session["profileid"] = sqlDt.Rows[0]["profileid"];
                Session["petName"] = sqlDt.Rows[0]["petName"];
                Session["breed"] = sqlDt.Rows[0]["breed"];
                Session["gender"] = sqlDt.Rows[0]["gender"];
                Session["age"] = sqlDt.Rows[0]["age"];
                Session["petbio"] = sqlDt.Rows[0]["bio"];
                Session["imageFileName"] = sqlDt.Rows[0]["imageFileName"];
                petinfo = "{" + "\"profileid\"" + ":" + "\"" + Session["profileid"].ToString() + "\"" + ","
                    + "\"petName\"" + ":" + "\"" + Session["petName"].ToString() + "\"" + ","
                    + "\"breed\"" + ":" + "\"" + Session["breed"].ToString() + "\"" + ","
                    + "\"gender\"" + ":" + "\"" + Session["gender"].ToString() + "\"" + ","
                    + "\"age\"" + ":" + "\"" + Session["age"].ToString() + "\"" + ","
                    + "\"petbio\"" + ":" + "\"" + Session["petbio"].ToString() + "\"" + ","
                    + "\"imageFileName\"" + ":" + "\"" + Session["imageFileName"].ToString() + "\"" + "}";

            }
            //return the result!

            return petinfo;
        }

        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public string Loadpet(string accID)
        {
            //we return this flag to tell them if they logged in or not
            string petinfo = "";
            //our connection string comes from our web.config file like we talked about earlier
            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session

            string sqlSelect = "SELECT profileid, petName, accountid, breed, gender, age, bio, imageFileName FROM profiles WHERE accountid=@userValue";


            //set up our connection object to be ready to use our connection string
            MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
            //set up our command object to use our connection, and our query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@userValue", HttpUtility.UrlDecode(accID));


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
                Session["profileid"] = sqlDt.Rows[0]["profileid"];
                Session["petName"] = sqlDt.Rows[0]["petName"];
                Session["breed"] = sqlDt.Rows[0]["breed"];
                Session["gender"] = sqlDt.Rows[0]["gender"];
                Session["age"] = sqlDt.Rows[0]["age"];
                Session["petbio"] = sqlDt.Rows[0]["bio"];
                Session["imageFileName"] = sqlDt.Rows[0]["imageFileName"];
                petinfo = "{" + "\"profileid\"" + ":" + "\"" + Session["profileid"].ToString() + "\"" + ","
                    + "\"petName\"" + ":" + "\"" + Session["petName"].ToString() + "\"" + ","
                    + "\"breed\"" + ":" + "\"" + Session["breed"].ToString() + "\"" + ","
                    + "\"gender\"" + ":" + "\"" + Session["gender"].ToString() + "\"" + ","
                    + "\"age\"" + ":" + "\"" + Session["age"].ToString() + "\"" + ","
                    + "\"petbio\"" + ":" + "\"" + Session["petbio"].ToString() + "\"" + ","
                    + "\"imageFileName\"" + ":" + "\"" + Session["imageFileName"].ToString() + "\"" + "}";

            }
            //return the result!
            return petinfo;
        }

        [WebMethod(EnableSession = true)]
        public Models.Account[] GetAccounts()
        {
            //check out the return type.  It's an array of Account objects.  You can look at our custom Account class in this solution to see that it's 
            //just a container for public class-level variables.  It's a simple container that asp.net will have no trouble converting into json.  When we return
            //sets of information, it's a good idea to create a custom container class to represent instances (or rows) of that information, and then return an array of those objects.  
            //Keeps everything simple.

            //WE ONLY SHARE ACCOUNTS WITH LOGGED IN USERS!
            if (Session["accountID"] != null)
            {
                DataTable sqlDt = new DataTable("accounts");

                string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
                string sqlSelect = "select accountID, userName, firstName, lastName, email, password from accounts order by lastName";

                MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

                //gonna use this to fill a data table
                MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);
                //filling the data table
                sqlDa.Fill(sqlDt);

                //loop through each row in the dataset, creating instances
                //of our container class Account.  Fill each acciount with
                //data from the rows, then dump them in a list.
                List<Models.Account> accounts = new List<Models.Account>();
                for (int i = 0; i < sqlDt.Rows.Count; i++)
                {
                    //only share user id and pass info with admins!
                        accounts.Add(new Models.Account
                        {
                            accountID = Convert.ToInt32(sqlDt.Rows[i]["accountID"]),
                            userName = sqlDt.Rows[i]["userName"].ToString(),
                            password = sqlDt.Rows[i]["password"].ToString(),
                            firstName = sqlDt.Rows[i]["firstName"].ToString(),
                            lastName = sqlDt.Rows[i]["lastName"].ToString(),
                            email = sqlDt.Rows[i]["email"].ToString()
                        });
                }
                //convert the list of accounts to an array and return!
                return accounts.ToArray();
            }
            else
            {
                //if they're not logged in, return an empty array
                return new Models.Account[0];
            }
        }

        [WebMethod(EnableSession = true)]
        public void DeleteAccount(string id)
        {
            if (Session["isAdmin"].ToString() == "true")
            {
                string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
                string sqlSelect = "delete from accounts where accountID=@idValue";

                MySqlConnection sqlConnection = new MySqlConnection(sqlConnectString);
                MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

                sqlCommand.Parameters.AddWithValue("@idValue", HttpUtility.UrlDecode(id));

                sqlConnection.Open();
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                }
                sqlConnection.Close();
            }
        }

    }
}
