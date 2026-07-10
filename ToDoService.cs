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
        private int _maxTasks = 0;
        private int _maxLengthTask = 0;

        public ToDoItem Add(ToDoUser user, string name)
        {
            if (_listTasks.Count() < _maxTasks)
            {
                if (name.Length > _maxLengthTask)
                {
                    throw new TaskLengthLimitException(name.Length, _maxLengthTask);
                }
                if (name.Length > 0)
                {
                    foreach (var curr in _listTasks)
                    {
                        if (curr.Name == name)
                        {
                            throw new DuplicateTaskException(name);
                        }
                    }
                }

                _listTasks.Add(new ToDoItem(user, name));
                return _listTasks.Last();
            }
            else
            {
                throw new TaskCountLimitException(_maxTasks);
            }
        }

        public void Delete(Guid id)
        {
            _listTasks.Remove(_listTasks.First(f => f.Id == id));
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            List<ToDoItem> listTemp = new List<ToDoItem>();

            foreach (var curr in _listTasks)
            {
                if (curr.User.UserId == userId && curr.State == ToDoItem.ToDoItemState.Active)
                    listTemp.Add(curr);
            }

            return listTemp;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            List<ToDoItem> listTemp = new List<ToDoItem>();

            foreach (var curr in _listTasks)
            {
                if (curr.User.UserId == userId)
                    listTemp.Add(curr);
            }

            return listTemp;
        }

        public void MarkCompleted(Guid id)
        {
            if (id != default(Guid))
            {
                _listTasks.Where(w => w.Id == id).FirstOrDefault().State = ToDoItem.ToDoItemState.Completed;
                _listTasks.Where(w => w.Id == id).FirstOrDefault().CreatedAt = DateTime.Now;
            }
        }

        public void ValidateString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException($"{0} это значение не соответствует требованиям", str);
        }

        public int ParseAndValidateInt(string str, int min, int max)
        {
            int answ = 0;

            if (!int.TryParse(str, out answ) || (answ < min || answ > max))
                throw new ArgumentException($"{0} это значение не соответствует требованиям", str);

            return answ;
        }

        public void SetConfiguration(int maxTasks, int maxLengthTask)
        {
            _maxTasks = maxTasks;
            _maxLengthTask = maxLengthTask;
        }
    }
}
