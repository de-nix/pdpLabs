using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using MySql.Data.MySqlClient;

namespace finalASP.Models
{
    public class DAL
    {
        MySqlConnection conn;

        string myConnectionString = "server=localhost;uid=root;pwd=;database=asp2;";

        public DAL()
        {
            conn = new MySqlConnection();
            conn.ConnectionString = myConnectionString;
            conn.Open();

        }

        public String getParticipants(string name)
        {
            String participants = "";
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select participants from courses where name = '" + name + "'";

                MySqlDataReader myreader = cmd.ExecuteReader();

                if (myreader.Read())
                {
                    participants = myreader.GetString("participants");
                }
                myreader.Close();
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
            }
            return participants;
        }
        public String getCourses(string name)
        {
            String courses = "";
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select name from courses where participants like '%" + name + "%'";

                MySqlDataReader myreader = cmd.ExecuteReader();

                while (myreader.Read())
                {
                    courses += myreader.GetString("name") + ",";
                }
                myreader.Close();
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
            }
            return courses;
        }
        public void insertGrade(String student,String course, String grade, int idProfessor) {


            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from courses where name = '" + course + "'";

                MySqlDataReader myreader = cmd.ExecuteReader();

                if (myreader.Read())
                {
                    int newID = myreader.GetInt32("idprofesor");
                    String grades = myreader.GetString("grades");
                    String participants = myreader.GetString("participants");
                    if (idProfessor != newID) return;
                    String newString = "";
                    myreader.Close();
                    if (grades.Contains(student + ":"))
                    {

                        newString = Regex.Replace(grades, student + ":..", student + ":" + grade);
                        Console.WriteLine(newString);
                        cmd.CommandText = "update courses set grades = '," + newString + "' where name = '"+course+"'";
                        cmd.ExecuteNonQuery();
                    }
                    else {

                        newString = grades + "," + student + ":" + grade;
                        participants = participants + "," + student;
                        cmd.CommandText = "update courses set grades = '" + newString + "' , participants ='" + participants + "' where name = '" + course + "'";
                        cmd.ExecuteNonQuery();
                    }
                    

                }
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
            }



        }



        internal void addAsset(Course asset)
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

        internal List<Course> getUserAssets(int id)
        {
            List<Course> ulist = new List<Course>();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from asset where userid = " + id;

                MySqlDataReader myreader = cmd.ExecuteReader();

                while (myreader.Read())
                {
                    Course asset = new Course();
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

        internal Person authenticate(string username)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from person where name = '"+username+"'";

                MySqlDataReader myreader = cmd.ExecuteReader();

                if (myreader.Read())
                {
                    int id = myreader.GetInt32("id");
                    String name = myreader.GetString("name");
                    String role = myreader.GetString("role");
                    Person user = new Person(id, name, role);
                    

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

        public Person insertUser(Person user)
        {   try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "insert into users(name, password) values ('"+user.Name+"','"+user.Role+"')" ;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "select * from users where name ='"+user.Name+"' AND password = '"+user.Role+"'";
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