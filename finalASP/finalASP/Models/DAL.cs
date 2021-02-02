using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using MySql.Data.MySqlClient;

namespace finalASP.Models
{
    public class DAL
    {
        MySqlConnection conn;

        string myConnectionString = "server=localhost;uid=root;pwd=;database=asp;";

        public DAL()
        {
            conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

        }

        public List<User> GetUsers()
        {
            List<User> ulist = new List<User>();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from users";

                MySqlDataReader myreader = cmd.ExecuteReader();

                while (myreader.Read())
                {
                    User user = new User();
                    user.Id = myreader.GetInt32("id");
                    user.Name = myreader.GetString("name");
                    user.Password = myreader.GetString("password");
                    ulist.Add(user);
                }
                myreader.Close();
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
            }
            return ulist;
        }

        internal void addAsset(Asset asset)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "insert into asset(name, description, value, userid ) values ('" + asset.Name + "','" + asset.Description +  "'," + asset.Value + "," + asset.UserId + ")";
                cmd.ExecuteNonQuery();     
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
    
            }

        }

        internal List<Asset> getUserAssets(int id)
        {
            List<Asset> ulist = new List<Asset>();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from asset where userid = " + id;

                MySqlDataReader myreader = cmd.ExecuteReader();

                while (myreader.Read())
                {
                    Asset asset = new Asset();
                    asset.Id = myreader.GetInt32("id");
                    asset.UserId = myreader.GetInt32("userid");
                    asset.Value = myreader.GetInt32("value");
                    asset.Name = myreader.GetString("name");
                    asset.Description = myreader.GetString("description");
                    ulist.Add(asset);
                }
                myreader.Close();
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
            }
            return ulist;
        }

        internal User authenticate(string username, string password)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select id from users where name = '"+username+"' and password = '"+password+"'";

                MySqlDataReader myreader = cmd.ExecuteReader();

                if (myreader.Read())
                {
                    int id = myreader.GetInt32("id");
                    User user = new User(id, username, password);
                    

                    myreader.Close();
                    return user;
                    
                }
                myreader.Close();
                return null;
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        internal bool isUnique(string username)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select name from users";

                MySqlDataReader myreader = cmd.ExecuteReader();

                while (myreader.Read())
                {
                    if (myreader.GetString("name").Equals(username)) {

                        myreader.Close();
                        return false; }
                }
                myreader.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }

        public User insertUser(User user)
        {   try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "insert into users(name, password) values ('"+user.Name+"','"+user.Password+"')" ;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select * from users where name ='"+user.Name+"' AND password = '"+user.Password+"'";
                MySqlDataReader myreader = cmd.ExecuteReader();

                if (myreader.Read())
                {
                    user.Id = myreader.GetInt32("id");
                    myreader.Close();
                    return user;
                }
                myreader.Close();
                return null;
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
                return null;
            }

        }

    }
}