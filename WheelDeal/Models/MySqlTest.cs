using MySql.Data.MySqlClient;
using System.Data;

namespace WheelDeal.Models
{
    public class MySqlTest
    {
        private readonly string _connectionString;

        public MySqlTest(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        //public List<string> GetUsernames()
        //{
        //    List<string> usernames = new List<string>();

        //    using (MySqlConnection conn = new MySqlConnection(_connectionString))
        //    {
        //        conn.Open();
        //        string query = "SELECT username FROM users";
        //        MySqlCommand cmd = new MySqlCommand(query, conn);
        //        MySqlDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read())
        //        {
        //            usernames.Add(reader["username"].ToString());
        //        }

        //        conn.Close();
        //    }

        //    return usernames;
        //}
    }
}
