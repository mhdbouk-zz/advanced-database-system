using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Used to return a string to save it into the csv file
        /// </summary>
        /// <returns></returns>
        public string ToRow()
        {
            return $"{UserId},{FirstName},{LastName},{DisplayName},{UserName},{Email},{Password}";
        }

        /// <summary>
        /// static method that return the header of the user csv file
        /// </summary>
        public static string Header()
        {
            return "UserId,FirstName,LastName,DisplayName,UserName,Email,Password";
        }

        /// <summary>
        /// static method to return new Models.User object from a string found from the csv file
        /// </summary>
        public static User GetUser(string row)
        {
            if (string.IsNullOrWhiteSpace(row))
                return null;
            var data = row.Split(',');
            return new User
            {
                UserId = data[0].ToInt(),
                FirstName = data[1],
                LastName = data[2],
                DisplayName = data[3],
                UserName = data[4],
                Email = data[5],
                Password = data[6]
            };
        }
    }
}