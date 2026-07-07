using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot
{
    internal class ToDoService : IToDoService
    {
        public ToDoItem Add(ToDoUser user, string name)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public void MarkCompleted(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
