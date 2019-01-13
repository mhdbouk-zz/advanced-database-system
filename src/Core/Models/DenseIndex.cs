using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DenseIndex
    {
        public int IndexKey { get; set; }
        public string IndexValue { get; set; }
        
        /// <summary>
        /// Used to return a string to save it into the csv file
        /// </summary>
        /// <returns></returns>
        public string ToRow()
        {
            return $"{IndexKey},{IndexValue}";
        }

        /// <summary>
        /// static method that return the header of the index csv file
        /// </summary>
        public static string Header()
        {
            return "Key,Value";
        }

        /// <summary>
        /// static method to return new Models.DenseIndex object from a string found from the csv file
        /// </summary>
        public static DenseIndex GetIndex(string row)
        {
            if (string.IsNullOrWhiteSpace(row))
                return null;
            var data = row.Split(',');
            return new DenseIndex
            {
                IndexKey = data[0].ToInt(),
                IndexValue = data[1]
            };
        }
    }
}
