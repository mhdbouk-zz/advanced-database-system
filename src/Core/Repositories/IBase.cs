using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IBase<T> where T : class
    {
        List<T> GetAll();
        T GetById(int Id);
        T Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int Id);
    }
}
