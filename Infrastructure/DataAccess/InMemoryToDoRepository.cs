using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using CookingBot.Core.DataAccess;
using CookingBot.Core.Entities;

namespace CookingBot.Infrastructure.DataAccess
{
    internal class InMemoryToDoRepository : IToDoRepository
    {
        private List<ToDoItem> _toDoItems = new List<ToDoItem>();

        public void Add(ToDoItem item)
        {
            _toDoItems.Add(item);
        }

        public int CountActive(Guid userId)
        {
            List<ToDoItem> listTemp = new List<ToDoItem>();

            foreach (var curr in _toDoItems)
            {
                if (curr.User.UserId == userId && curr.State == ToDoItem.ToDoItemState.Active)
                    listTemp.Add(curr);
            }

            return listTemp.Count();
        }

        public void Delete(Guid id)
        {
            _toDoItems.Remove(_toDoItems.First(f => f.Id == id));
        }

        public bool ExistsByName(Guid userId, string name)
        {
            foreach (var curr in _toDoItems)
            {
                if (curr.User.UserId == userId && curr.Name == name)
                    return true;
            }
            return false;
        }

        public ToDoItem? Get(Guid id)
        {            
            return _toDoItems.FirstOrDefault(curr => curr.Id == id);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            List<ToDoItem> listTemp = new List<ToDoItem>();

            foreach (var curr in _toDoItems)
            {
                if (curr.User.UserId == userId && curr.State == ToDoItem.ToDoItemState.Active)
                    listTemp.Add(curr);
            }

            return listTemp;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            List<ToDoItem> listTemp = new List<ToDoItem>();

            foreach (var curr in _toDoItems)
            {
                if (curr.User.UserId == userId)
                    listTemp.Add(curr);
            }

            return listTemp;
        }

        private void CheckItemInMemory(ToDoItem item) 
        {
            if (_toDoItems.Find(f => f.Id == item.Id) == null)
            {
                throw new FindItemInMemoryException(item.Id.ToString());
            }
        }

        public void Update(ToDoItem item)
        {
            CheckItemInMemory(item);

            var index = _toDoItems.IndexOf(_toDoItems.FirstOrDefault(f => f.Id == item.Id));
            _toDoItems[index] = item;
        }
    }
}
