using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedDatabaseSystems
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database(init: true);

            operation:
            Console.WriteLine("Choose Operation");
            Console.WriteLine("1. Select");

            int operation = Console.ReadLine().ToInt();
            if (operation <= 0 || operation > 2) goto operation;
            DateTime t1 = DateTime.Now;
            DateTime t2 = DateTime.Now;
            TimeSpan ts = new TimeSpan();
            if (operation == 1)
            {
                usingIndex:
                Console.WriteLine("Choose select method");
                Console.WriteLine("1. Index");
                Console.WriteLine("2. File Scan");
                int usingIndex = Console.ReadLine().ToInt();
                if (usingIndex <= 0 || usingIndex > 2) goto usingIndex;
                Console.Write("Id: ");
                int Id = Console.ReadLine().ToInt();
                if (Id == 0) goto operation;
                t1 = DateTime.Now;
                var user = db.User.GetById(usingIndex == 1 ? SelectType.Index : SelectType.FileScan, Id);
                if (user == null)
                {
                    Console.WriteLine("Not Found");
                }
                else
                {
                    Console.WriteLine();
                    Helper.PrintLine();
                    Helper.PrintRow("UserId", "First Name", "Last Name", "Email", "Display Name", "UserName", "Password");
                    Helper.PrintLine();
                    Helper.PrintRow(user.UserId.ToString(), user.FirstName, user.LastName, user.Email, user.DisplayName, user.UserName, user.Password);
                    Console.WriteLine();
                    var addresses = db.Address.GetByUserId(usingIndex == 1 ? SelectType.Index : SelectType.FileScan, user.UserId);
                    Helper.PrintLine();
                    Helper.PrintRow("AddressId", "UserId", "Country", "Address Line");
                    Helper.PrintLine();
                    foreach (var address in addresses)
                    {
                        Helper.PrintRow(address.AddressId.ToString(), address.UserId.ToString(), address.Country, address.AddressLine);
                    }

                }
            }
            //else if (operation == 2)
            //{
            //    Console.Write("First Name:\t");
            //    string firstName = Console.ReadLine();

            //    Console.Write("Last Name:\t");
            //    string lastName = Console.ReadLine();

            //    Console.Write("Email:\t\t");
            //    string email = Console.ReadLine();

            //    Console.Write("Password:\t");
            //    string password = Console.ReadLine();
            //    t1 = DateTime.Now;
            //    var user = new Core.Models.User()
            //    {
            //        FirstName = firstName,
            //        LastName = lastName,
            //        DisplayName = $"{firstName} {lastName}",
            //        Email = email,
            //        UserName = $"{firstName[0].ToString().ToLower()}{lastName.ToLower()}",
            //        Password = password
            //    };
            //    user = db.User.Insert(user);
            //    Helper.PrintLine();
            //    Helper.PrintRow("UserId", "First Name", "Last Name", "Email", "Display Name", "UserName", "Password");
            //    Helper.PrintLine();
            //    Helper.PrintRow(user.UserId.ToString(), user.FirstName, user.LastName, user.Email, user.DisplayName, user.UserName, user.Password);
            //    Console.WriteLine();
            //    Console.Write("Number of addresses: ");
            //    int addressCount = Console.ReadLine().ToInt();
            //    List<Core.Models.Address> addresses = new List<Core.Models.Address>();

            //    for (int i = 0; i < addressCount; i++)
            //    {
            //        Console.Write("Country:\t");
            //        string country = Console.ReadLine();

            //        Console.Write("Address:\t");
            //        string AddressLine = Console.ReadLine();

            //        addresses.Add(db.Address.Insert(new Core.Models.Address
            //        {
            //            AddressLine = AddressLine,
            //            Country = country,
            //            UserId = user.UserId
            //        }));
            //    }
            //    if (addressCount > 0)
            //    {
            //        Helper.PrintLine();
            //        Helper.PrintRow("AddressId", "UserId", "Country", "Address Line");
            //    }
            //    foreach (var address in addresses)
            //    {
            //        Helper.PrintRow(address.AddressId.ToString(), address.UserId.ToString(), address.Country, address.AddressLine);
            //    }

            //}

            t2 = DateTime.Now;
            ts = t2.Subtract(t1);
            Console.WriteLine(ts.ToString());

            goto operation;
        }
    }
}
