using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System.Windows.Forms;



namespace DBConnect 
{
        class DBConnection
    {
        //Store the MySql connection to use later
        MySqlConnection connection = null;
        
        string connectionString;

        public MySqlConnection Connection { get => connection; }

        //Connection string information:
        //servername
        //database
        //userID
        //password
        
        public DBConnection(string serverName, string databaseName, string userID, string password)
        {
            //Example Format Connection String;
            //SERVERNAME=localhost;DATABASE=hire;UID=csharp;PASSWORD=password;

            this.connectionString = string.Format($"SERVER={serverName};DATABASE={databaseName};UID={userID};PASSWORD={password};");

        }

        public bool Connect()
        {
             bool succeeded = true;

            try
            {
                this.connection = new MySqlConnection(this.connectionString);

                this.connection.Open();
            }
            catch (MySqlException ex)
            {
                succeeded = false;

                switch(ex.Number)
                {
                    case 0:
                        MessageBox.Show("Authentication error - please check your login credentials.");
                        break;

                    case 1045:
                        MessageBox.Show("Cannot connect to server.");
                        break;

                    default:
                        MessageBox.Show("Exception found: " + ex.Message);
                        break;
                }
            }


            return succeeded;
        }

        public bool Close()
        {
            bool succeeded = true;

            try
            {
                this.connection.Close();
            }
            catch(MySqlException ex)
            {
                succeeded = false;
                MessageBox.Show("Exception found: " + ex.Message);
            }

            return succeeded;
        }
    }
}
