﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace ProjectTemplate
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]

	public class ProjectServices : System.Web.Services.WebService
	{
		///replace the values of these variables with your database credentials
		private string dbID = "springc2023team1";
		private string dbPass = "springc2023team1";
		private string dbName = "springc2023team1";
		
		///call this method anywhere that you need the connection string!
		private string getConString() {
			return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName+"; UID=" + dbID + "; PASSWORD=" + dbPass;
		}

        [WebMethod(EnableSession = true)] //NOTICE: gotta enable session on each individual method
        public bool LogOn(string username, string password)
        {
            //we return this flag to tell them if they logged in or not
            bool success = false;

            //our connection string comes from our web.config file like we talked about earlier
/*            string sqlConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["myDB"].ConnectionString;
*/            //here's our query.  A basic select with nothing fancy.  Note the parameters that begin with @
            //NOTICE: we added admin to what we pull, so that we can store it along with the id in the session
            string sqlSelect = "SELECT username, email, is_photographer FROM users WHERE username=@usernameValue and password=@passwordValue";

            //set up our connection object to be ready to use our connection string
            MySqlConnection sqlConnection = new MySqlConnection(getConString());
            //set up our command object to use our connection, and our query
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            //tell our command to replace the @parameters with real values
            //we decode them because they came to us via the web so they were encoded
            //for transmission (funky characters escaped, mostly)
            sqlCommand.Parameters.AddWithValue("@usernameValue", HttpUtility.UrlDecode(username));
            sqlCommand.Parameters.AddWithValue("@passwordValue", HttpUtility.UrlDecode(password));

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
                // store photographer status
                Session["username"] = sqlDt.Rows[0]["username"];
                Session["is_photographer"] = sqlDt.Rows[0]["is_photographer"];
                success = true;
            }
            //return the result!
            return success;
        }

        [WebMethod(EnableSession = true)]
        public bool CreateAccount(string username, string password, string email, string first_name, string is_photographer, 
            string availability, string style, string type, string range, string experience)
        {
            string sqlSelect;

            if (is_photographer == "Photographer")
            {

                sqlSelect = "insert into users (username, password, email, first_name, is_photographer) " +
                "values(@usernameValue, @passwordValue, @emailValue, @firstNameValue, 1);" +
                "insert into photographers values(@usernameValue, @availabilityValue, @styleValue, @typeValue, @rangeValue, @experienceValue);";
            }
            else
            {
                sqlSelect = "insert into users (username, password, email, first_name, is_photographer) " +
                "values(@usernameValue, @passwordValue, @emailValue, @firstNameValue, 0);" +
                "insert into clients (username, availability, style, type, budget_range, experience)" +
                "values(@usernameValue, @availabilityValue, @styleValue, @typeValue, @rangeValue, @experienceValue);";
            }

            MySqlConnection sqlConnection = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@usernameValue", HttpUtility.UrlDecode(username));
            sqlCommand.Parameters.AddWithValue("@passwordValue", HttpUtility.UrlDecode(password));
            sqlCommand.Parameters.AddWithValue("@emailValue", HttpUtility.UrlDecode(email));
            sqlCommand.Parameters.AddWithValue("@firstNameValue", HttpUtility.UrlDecode(first_name));

            sqlCommand.Parameters.AddWithValue("@availabilityValue", HttpUtility.UrlDecode(availability));
            sqlCommand.Parameters.AddWithValue("@styleValue", HttpUtility.UrlDecode(style));
            sqlCommand.Parameters.AddWithValue("@typeValue", HttpUtility.UrlDecode(type));
            sqlCommand.Parameters.AddWithValue("@rangeValue", HttpUtility.UrlDecode(range));
            sqlCommand.Parameters.AddWithValue("@experienceValue", HttpUtility.UrlDecode(experience));
    
            sqlConnection.Open();
            //we're using a try/catch so that if the query errors out we can handle it gracefully
            //by closing the connection and moving on
            try{
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
                Session["username"] = username;
                Session["is_photographer"] = password;
                return true;

            }
            catch
            {
                sqlConnection.Close();
                return false;
            }

        }
        
        [WebMethod(EnableSession = true)]
        public string ReturnPotentialMatches()
        {
            string sqlSelect;

            if (Convert.ToInt32(Session["is_photographer"]) == 1)
            {
                sqlSelect = "SELECT username, availability, style, type, budget_range, experience FROM clients WHERE has_match = 0 AND username not in " +
                    "(SELECT client_username FROM rejects WHERE photographer_username = @photographerUsernameValue);";    
            }
            else
            {
                if (checkMatchedStatus() == 1){
                    return "Already Matched!";
                }
                else
                {
                    sqlSelect = "SELECT username, availability, style, type, budget_range, experience FROM photographers WHERE username in" +
                        "(SELECT photographer_username FROM pendings WHERE client_username = @clientUsernameValue);";
                }
            }
            
            MySqlConnection sqlConnection = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@clientUsernameValue", HttpUtility.UrlDecode(Convert.ToString(Session["username"])));
            sqlCommand.Parameters.AddWithValue("@photographerUsernameValue", HttpUtility.UrlDecode(Convert.ToString(Session["username"])));

            MySqlDataAdapter sqlDa = new MySqlDataAdapter(sqlCommand);

            DataTable sqlDt = new DataTable("clientAccounts");
            sqlDa.Fill(sqlDt);

            string output = "[";

            for (int i = 0; i < sqlDt.Rows.Count; i++)
            {
                output += "{" + "\"username\":\"" + sqlDt.Rows[i]["username"] + "\", \"availability\":\"" + sqlDt.Rows[i]["availability"] + "\",\"style\":\"" +
                    sqlDt.Rows[i]["style"] + "\", \"type\":\"" + sqlDt.Rows[i]["type"] + "\", \"budget_range\":\"" + sqlDt.Rows[i]["budget_range"] +
                    "\", \"experience\":\"" + sqlDt.Rows[i]["experience"] + "\"}";

                if (i != sqlDt.Rows.Count - 1)
                {
                    output += ",";
                }
            }
            output += "]";

            return output;
        }

        [WebMethod(EnableSession = true)]
        public int checkMatchedStatus()
        {
            string sqlSelect = "SELECT has_match FROM clients WHERE username = @clientUsernameValue;";
           
            MySqlConnection sqlConnection = new MySqlConnection(getConString());
            MySqlCommand sqlCommand = new MySqlCommand(sqlSelect, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@clientUsernameValue", HttpUtility.UrlDecode(Convert.ToString(Session["username"])));

            sqlConnection.Open();
            
            return Convert.ToInt32(sqlCommand.ExecuteScalar());

        }
    }
}
