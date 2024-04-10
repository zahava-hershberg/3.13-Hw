using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3._13_Hw.Data
{
    public class AdManager
    {
        private string _connectionString;
        public AdManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser(Users u, string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT Into Users(Name,Email,Password) VALUES(@name, @email, @password) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", u.Name);
            cmd.Parameters.AddWithValue("@email", u.Email);
            cmd.Parameters.AddWithValue("@password", hash);
            connection.Open();
            u.Id = (int)(decimal)cmd.ExecuteScalar();

        }
        public void AddAd(Ads a, int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads(Number, Description, Date, UserId) VALUES(@num, @description, @date, @userId)";
            cmd.Parameters.AddWithValue("@num", a.Number);
            cmd.Parameters.AddWithValue("@description", a.Description);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@userId", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public Users Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            var isMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isMatch)
            {
                return null;
            }

            return user;
        }
        public Users GetByEmail(string email)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT TOP 1 * FROM Users WHERE email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new Users
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                Password= (string)reader["Password"]
            };
        }
        public List<Ads> GetAds()
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name FROM Ads a JOIN Users u ON u.Id=a.UserId";
            connection.Open();
            var ads = new List<Ads>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ads
                {
                    Id = (int)reader["Id"],
                    Number = (string)reader["Number"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"],
                    UserName = (string)reader["Name"]
                });

            }
            return ads;
        }
        public void Delete(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Ads WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Ads> Account(int id)
        {
            var connection = new SqlConnection(_connectionString);
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Ads a JOIN Users u ON u.Id = a.UserId WHERE a.UserId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var ads = new List<Ads>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ads
                {
                    Id = (int)reader["Id"],
                    Number = (string)reader["Number"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"]
                });
            }
            return ads;

        }

    }
}
