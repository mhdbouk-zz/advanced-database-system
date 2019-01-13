using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using System.IO;

namespace Core.Repositories
{
    public class Address : IBase<Models.Address>
    {
        public List<Models.Address> GetAll()
        {
            List<Models.Address> addresses = new List<Models.Address>();
            var dataFromFile = File.ReadAllLines(Database.addressTablePath).Skip(1).ToArray();

            foreach (var item in dataFromFile)
            {
                var address = Models.Address.GetAddress(item);
                if (address != null) addresses.Add(address);
            }
            return addresses;
        }

        public Models.Address GetById(int Id)
        {
            var all = File.ReadAllLines(Database.addressTablePath).Skip(1).ToArray();

            for (int i = 0; i < all.Count(); i++)
            {
                var address = Models.Address.GetAddress(all[i]);
                if (address.AddressId == Id)
                {
                    return address;
                }
                continue;
            }
            return null;
        }

        public List<Models.Address> GetByUserId(Core.SelectType type, int UserId)
        {
            List<Models.Address> address = new List<Models.Address>();
            switch (type)
            {
                case SelectType.Index:
                    using (var streamReader = new StreamReader(Database.addressTablePath))
                    {
                        var indexFile = File.ReadAllLines(Database.addressIndexPath).Skip(1).ToList();
                        var index = indexFile.Where(a => Models.DenseIndex.GetIndex(a).IndexKey == UserId);
                        if (index == null) return address;
                        foreach (var item in index)
                        {
                            var offset = long.Parse(Models.DenseIndex.GetIndex(item).IndexValue);
                            streamReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                            address.Add(Models.Address.GetAddress(streamReader.ReadLine()));
                        }
                    }
                    return address;
                case SelectType.FileScan:
                    var all = File.ReadAllLines(Database.addressTablePath).Skip(1).ToArray();
                    for (int i = 0; i < all.Count(); i++)
                    {
                        var thisAddress = Models.Address.GetAddress(all[i]);
                        if (thisAddress != null && thisAddress.UserId == UserId)
                        {
                            address.Add(thisAddress);
                        }
                    }
                    return address;
                default:
                    return address;
            }
        }

        public Models.Address Insert(Models.Address entity)
        {
            //get lastest Id
            var allAddress = GetAll();
            int addressId = allAddress.OrderByDescending(a => a.AddressId).FirstOrDefault().AddressId + 1;
            entity.AddressId = addressId;
            //insert
            File.AppendAllText(Database.addressTablePath, $"\n{entity.ToRow()}");

            var indices = File.ReadAllLines(Database.addressIndexPath).Skip(1);
            List<Models.DenseIndex> AllDenseIndex = new List<DenseIndex>();
            bool found = false;
            foreach (var item in indices)
            {
                var index = Models.DenseIndex.GetIndex(item);
                if (index.IndexKey == entity.UserId)
                {
                    found = true;
                    index.IndexValue += ";" + (allAddress.Count + 1).ToString();
                }
                AllDenseIndex.Add(index);
            }
            if (!found)
            {
                AllDenseIndex.Add(new DenseIndex
                {
                    IndexKey = entity.UserId,
                    IndexValue = (allAddress.Count + 1).ToString()
                });
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(Models.DenseIndex.Header());
            AllDenseIndex.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );
            File.WriteAllText(Database.addressIndexPath, sb.ToString());
            //return with Id
            return entity;
        }

        public void Update(Models.Address entity)
        {
            var all = GetAll();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].AddressId == entity.AddressId)
                {
                    all[i] = entity;
                    break;
                }
                continue;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(Models.Address.Header());
            all.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );
            File.WriteAllText(Database.addressTablePath, sb.ToString());
        }

        internal void DeleteByUserId(int userId)
        {
            var all = GetAll().Where(a => a.UserId == userId);
            foreach (var item in all)
            {
                Delete(item);
            }

        }

        public void Delete(Models.Address entity)
        {
            var all = GetAll();
            var indices = File.ReadAllLines(Database.addressIndexPath).Skip(1).ToArray().Select(a => Models.DenseIndex.GetIndex(a)).ToList();

            List<Models.Address> addresses = new List<Models.Address>();
            bool found = false;

            for (int i = 0; i < all.Count; i++)
            {
                var item = all[i];
                if (item.AddressId == entity.AddressId)
                {
                    found = true;
                    continue;
                }
                if (found)
                {
                    indices = UpdateIndex(item.UserId, i, indices);
                }
                addresses.Add(item);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(Models.Address.Header());
            addresses.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );
            File.WriteAllText(Database.addressTablePath, sb.ToString());

            sb.Clear();
            sb.Append(Models.DenseIndex.Header());
            indices.ForEach(a =>
            {
                sb.AppendLine();
                sb.Append(a.ToRow());
            }
            );
            File.WriteAllText(Database.addressIndexPath, sb.ToString());
            //DeleteIndex(entity.UserId);
        }
        public void Delete(int Id)
        {
            Delete(GetById(Id));
        }
        private void UpdateIndex(int UserId, int AddressRowId)
        {
            //var indices = File.ReadAllLines(Database.addressIndexPath).Skip(1).ToArray();
            //List<Models.DenseIndex> allIndices = new List<Models.DenseIndex>();
            //allIndices = UpdateIndex(UserId, AddressRowId, indices);

            //StringBuilder sb = new StringBuilder();
            //sb.Append(Models.DenseIndex.Header());
            //allIndices.ForEach(a =>
            //{
            //    sb.AppendLine();
            //    sb.Append(a.ToRow());
            //}
            //);
            //File.WriteAllText(Database.addressIndexPath, sb.ToString());
        }

        private List<DenseIndex> UpdateIndex(int UserId, int AddressRowId, List<DenseIndex> indices)
        {
            //List<DenseIndex> allIndices = new List<DenseIndex>();
            var index = indices.FirstOrDefault(a => a.IndexKey == UserId);

            //foreach (var index in indices)
            //{
            //var index = Models.DenseIndex.GetIndex(item);

            if (index.IndexKey == UserId)
            {
                var values = index.IndexValue.Split(';').Select(a => a.ToInt()).Where(a => a != 0);
                List<int> newValues = new List<int>();
                foreach (var value in values)
                {
                    int val = value;
                    if (val == AddressRowId)
                        val = val - 1;
                    newValues.Add(val);
                }
                string newValuesStr = string.Empty;
                newValues.ForEach(a => newValuesStr += a + ";");
                index.IndexValue = newValuesStr;
            }
            //allIndices.Add(index);
            //indices.(a => a.IndexKey == UserId) = index;
            //}
            return indices;
        }

        private void DeleteIndex(int UserId)
        {
            var indices = File.ReadAllLines(Database.addressIndexPath).Skip(1).ToArray();
            List<Models.DenseIndex> allIndices = new List<Models.DenseIndex>();
            foreach (var item in indices)
            {
                var index = Models.DenseIndex.GetIndex(item);

                if (index.IndexKey == UserId)

                    //var values = index.IndexValue.Split(';').Select(a => a.ToInt());
                    //if (values.Contains(Id)) continue;
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
            File.WriteAllText(Database.addressIndexPath, sb.ToString());
        }
    }
}
