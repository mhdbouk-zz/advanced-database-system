using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public class User : IBase<Models.User>
    {
        public List<Models.User> GetAll()
        {
            DateTime t1 = DateTime.Now;
            List<Models.User> users = new List<Models.User>();
            var dataFromFile = File.ReadAllLines(Database.userTablePath).Skip(1).ToArray();

            foreach (var item in dataFromFile)
            {
                var user = Models.User.GetUser(item);
                if (user != null) users.Add(user);
            }
            DateTime t2 = DateTime.Now;
            TimeSpan ts = t2.Subtract(t1);
            Console.Write("get all time: ");
            Console.WriteLine(ts.ToString());
            return users;
        }
        public Models.User GetById(int Id)
        {
            var all = File.ReadAllLines(Database.userTablePath).Skip(1).ToArray();

            for (int i = 0; i < all.Count(); i++)
            {
                var user = Models.User.GetUser(all[i]);
                if (user != null && user.UserId == Id)
                {
                    return user;
                }
                continue;
            }
            return null;
        }

        public Models.User GetById(Core.SelectType type, int Id)
        {
            switch (type)
            {
                case SelectType.Index:
                    var indexFile = File.ReadAllLines(Database.userIndexPath).Skip(1).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
                    var index = indexFile.FirstOrDefault(a => Models.DenseIndex.GetIndex(a).IndexKey == Id);
                    if (index == null) return null;
                    var offset = Models.DenseIndex.GetIndex(index).IndexValue.ToInt();

                    using (var streamReader = new StreamReader(Database.userTablePath))
                    {
                        streamReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                        return Models.User.GetUser(streamReader.ReadLine());
                    }
                case SelectType.FileScan:
                    var all = File.ReadAllLines(Database.userTablePath).Skip(1).ToArray().Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                    for (int i = 0; i < all.Count(); i++)
                    {
                        var user = Models.User.GetUser(all[i]);
                        if (user != null && user.UserId == Id)
                        {
                            return user;
                        }
                    }
                    return null;
                default:
                    return null;
            }
        }

        public Models.User Insert(Models.User entity)
        {
            //get lastest Id
            var all = GetAll();
            int userId = all.OrderByDescending(a => a.UserId).FirstOrDefault().UserId + 1;
            entity.UserId = userId;
            //insert
            File.AppendAllText(Database.userTablePath, $"\n{entity.ToRow()}");

            var index = new Models.DenseIndex()
            {
                IndexKey = entity.UserId,
                IndexValue = (all.Count + 1).ToString()
            };
            File.AppendAllText(Database.userIndexPath, $"\n{index.ToRow()}");
            //return with Id
            return entity;
        }
        public void Update(Models.User entity)
        {
            var all = GetAll();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].UserId == entity.UserId)
                {
                    all[i] = entity;
                    break;
                }
                continue;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(Models.User.Header());
            all.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );
            File.WriteAllText(Database.userTablePath, sb.ToString());
        }
        public void Delete(Models.User entity)
        {
            var all = GetAll();
            List<Models.User> users = new List<Models.User>();
            foreach (var item in all)
            {
                if (item.UserId == entity.UserId)
                    continue;
                users.Add(item);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(Models.User.Header());
            users.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );
            File.WriteAllText(Database.userTablePath, sb.ToString());

            Address addressRepo = new Address();
            addressRepo.DeleteByUserId(entity.UserId);
        }
        public void Delete(int Id)
        {
            Delete(GetById(Id));
            DeleteIndex(Id);
        }
        private void DeleteIndex(int Id)
        {
            var indices = File.ReadAllLines(Database.userIndexPath).Skip(1).ToArray();
            List<Models.DenseIndex> allIndices = new List<Models.DenseIndex>();
            bool found = false;
            foreach (var item in indices)
            {
                var index = Models.DenseIndex.GetIndex(item);
                if (index.IndexKey == Id)
                {
                    found = true;
                    continue;
                }
                if (found)
                {
                    int value = index.IndexValue.ToInt();
                    index.IndexValue = (value-1).ToString();
                }
                allIndices.Add(index);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(Models.DenseIndex.Header());
            allIndices.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );
            File.WriteAllText(Database.userIndexPath, sb.ToString());

            indices = File.ReadAllLines(Database.addressIndexPath).Skip(1).ToArray();
            allIndices = new List<Models.DenseIndex>();
            //bool found;
            foreach (var item in indices)
            {
                var index = Models.DenseIndex.GetIndex(item);
                if (index.IndexKey == Id)
                    continue;
                allIndices.Add(index);
            }

            sb.Clear();
            sb.Append(Models.DenseIndex.Header());
            allIndices.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );
            File.WriteAllText(Database.addressIndexPath, sb.ToString());
        }
    }
}
