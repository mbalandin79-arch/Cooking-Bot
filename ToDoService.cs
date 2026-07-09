using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot
{
    internal class ToDoService : IToDoService
    {
        private List<ToDoItem> _listTasks = new List<ToDoItem>();

        public ToDoItem Add(ToDoUser user, string name)
        {
            _listTasks.Add(new ToDoItem(user, name));
            return _listTasks.Last();
        }

        public void Delete(Guid id)
        {
            _listTasks.RemoveAll(r => r.Id == id);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            List<ToDoItem> listTemp = new List<ToDoItem>();
            foreach (ToDoItem item in _listTasks) 
            {
                if (item.Id == userId && item.State == ToDoItem.ToDoItemState.Active)
                    listTemp.Add(item);
            } 
            return listTemp;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            List<ToDoItem> listTemp = new List<ToDoItem>();
            foreach (ToDoItem item in _listTasks)
            {
                if (item.Id == userId)
                    listTemp.Add(item);
            }
            return listTemp;
        }

        public void MarkCompleted(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
