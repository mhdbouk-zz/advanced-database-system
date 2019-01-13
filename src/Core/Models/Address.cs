using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
        public int UserId { get; set; }

        /// <summary>
        /// Used to return a string to save it into the csv file
        /// </summary>
        /// <returns></returns>
        public string ToRow()
        {
            return $"{AddressId},{UserId},{Country},{AddressLine}";
        }

        /// <summary>
        /// static method that return the header of the address csv file
        /// </summary>
        public static string Header()
        {
            return "AddressId,UserId,Country,AddressLine";
        }

        /// <summary>
        /// static method to return new Models.Address object from a string found from the csv file
        /// </summary>
        public static Address GetAddress(string row)
        {
            if (string.IsNullOrWhiteSpace(row))
                return null;
            var data = row.Split(',');
            return new Address
            {
                AddressId = data[0].ToInt(),
                UserId = data[1].ToInt(),
                Country = data[2],
                AddressLine = data[3]
            };
        }
    }
}
