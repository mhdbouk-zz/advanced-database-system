using Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Database
    {
        private const string firstNamePath = "data\\firstName.txt";
        private const string lastNamePath = "data\\lastName.txt";
        public static string userTablePath = "data\\Users.csv";
        public static string addressTablePath = "data\\Addresses.csv";
        public static string addressIndexPath = "data\\AddressIndex.csv";
        public static string userIndexPath = "data\\UserIndex.csv";

        public Repositories.User User;
        public Repositories.Address Address;
        public Database(bool init = false)
        {
            Console.WriteLine("Database attached!");
            User = new Repositories.User();
            Address = new Repositories.Address();
            if (init)
                Init();
        }

        private void Init()
        {
            Console.WriteLine("Init Database..");
            var firstNames = File.ReadAllLines(firstNamePath);
            var lastNames = File.ReadAllLines(lastNamePath);
            try { File.Delete(userTablePath); } catch { }
            try { File.Delete(addressTablePath); } catch { }
            try { File.Delete(addressIndexPath); } catch { }
            try { File.Delete(userIndexPath); } catch { }
            List<DenseIndex> UserIndex = new List<DenseIndex>();
            List<DenseIndex> AddressIndex = new List<DenseIndex>();
            List<User> users = new List<User>();
            List<Address> addresses = new List<Address>();
            int userId = 1;
            int addressId = 1;
            var random = new Random();
            ConsoleSpiner spin = new ConsoleSpiner();
            foreach (var first in firstNames)
            {
                foreach (var last in lastNames)
                {
                    var user = new User
                    {
                        FirstName = first,
                        LastName = last,
                        UserId = userId,
                        DisplayName = $"{first} {last}",
                        UserName = $"{first[0].ToString().ToLower()}{last.ToLower()}",
                        Email = $"{first[0].ToString().ToLower()}{last.ToLower()}@gmail.com",
                        Password = $"{first}@{last}@{Math.Pow(userId, 2)}!"
                    };

                    users.Add(user);
                    int randomAddresses = random.Next(5);
                    for (int i = 0; i < randomAddresses; i++)
                    {
                        var address = new Address
                        {
                            AddressId = addressId,
                            UserId = user.UserId,
                            Country = Helper.RandomCountry(),
                            AddressLine = Helper.RandomString(45)
                        };
                        addresses.Add(address);
                        addressId++;
                    }

                    userId++;
                    spin.Turn();
                }
            }
            Console.WriteLine();
            StringBuilder sb = new StringBuilder();
            sb.Append(Models.User.Header());
            users.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );

            File.AppendAllText(userTablePath, sb.ToString());

            using (var streamReader = new StreamReader(userTablePath))
            {
                OffsetStreamReader reader = new OffsetStreamReader(streamReader.BaseStream, true);
                for (int i = 0; i < users.Count + 1; i++)
                {
                    long offset = reader.Offset;
                    string val = reader.ReadLine();
                    if (offset == 0) continue;

                    if (string.IsNullOrWhiteSpace(val))
                        break;

                    UserIndex.Add(new DenseIndex { IndexKey = val.Split(',')[0].ToInt(), IndexValue = offset.ToString() });
                }
            }

            sb.Clear();
            sb.Append(Models.DenseIndex.Header());
            UserIndex.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );

            File.AppendAllText(userIndexPath, sb.ToString());

            sb.Clear();
            sb.Append(Models.Address.Header());
            addresses.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );

            File.AppendAllText(addressTablePath, sb.ToString());

            using (var streamReader = new StreamReader(addressTablePath))
            {
                OffsetStreamReader reader = new OffsetStreamReader(streamReader.BaseStream, true);
                for (int i = 0; i < addresses.Count + 1; i++)
                {
                    long offset = reader.Offset;
                    string val = reader.ReadLine();
                    if (offset == 0) continue;

                    if (string.IsNullOrWhiteSpace(val))
                        break;
                    int currentUserId = val.Split(',')[1].ToInt();
                    AddressIndex.Add(new DenseIndex { IndexKey = currentUserId, IndexValue = offset.ToString() });
                }
            }

            sb.Clear();
            sb.Append(Models.DenseIndex.Header());
            AddressIndex.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );

            File.AppendAllText(addressIndexPath, sb.ToString());

            Console.WriteLine("Database Filled with data");
        }
    }
}
