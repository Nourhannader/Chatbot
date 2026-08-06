using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Repositories
{
    public interface IBaseRepository<T,TKey> where T : class 
    {
        Task<T?> GetByIdAsync(TKey id);
        Task AddAsync(T entity);
        void Update(T entity);
    }
}
