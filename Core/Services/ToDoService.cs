using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.DataAccess;
using CookingBot.Core.Entities;
using CookingBot.Core.Exceptions;
using Otus.ToDoList.ConsoleBot.Types;

namespace CookingBot.Core.Services
{
    internal class ToDoService : IToDoService
    {
        private List<ToDoItem> _listTasks = new List<ToDoItem>();
        private int _maxTasks = 0;
        private int _maxLengthTask = 0;
        private readonly IToDoRepository _toDoRepository;

        public ToDoService() { }

        public ToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = (IToDoRepository?)toDoRepository;
        }

        private void CheckCounthLimit()
        {
            if (_listTasks.Count() >= _maxTasks)
                throw new TaskCountLimitException(_maxTasks);
        }

        private void CheckLengthLimits(string name)
        {
            if (name.Length > _maxLengthTask)
            {
                throw new TaskLengthLimitException(name.Length, _maxLengthTask);
            }
        }

        private void CheckDuplicate(string name)
        {
            foreach (var curr in _listTasks)
            {
                if (curr.Name == name)
                {
                    throw new DuplicateTaskException(name);
                }
            }
        }

        public ToDoItem Add(ToDoUser user, string name)
        {
            CheckCounthLimit();
            if (name.Length > 0)
            {
                CheckLengthLimits(name);
                CheckDuplicate(name);
            }

            _listTasks.Add(new ToDoItem(user, name));
            return _listTasks.Last();
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

            if (!int.TryParse(str, out answ) || answ < min || answ > max)
                throw new ArgumentException($"{0} это значение не соответствует требованиям", str);

            return answ;
        }

        public void SetConfiguration(int maxTasks, int maxLengthTask)
        {
            _maxTasks = maxTasks;
            _maxLengthTask = maxLengthTask;
        }

        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            // очистка _toDoRepository
            List<ToDoItem> listTemp = new List<ToDoItem>();
            listTemp = _toDoRepository.GetAllByUserId(user.UserId).ToList();
            foreach (var curr in listTemp)
            {
                _toDoRepository.Delete(curr.Id);
            }

            // заполнение _toDoRepository
            foreach (var curr in _listTasks)
            {
                if (curr.User.UserId == user.UserId)
                {
                    _toDoRepository.Add(curr);
                }
            }

            return _toDoRepository.Find(user.UserId, x => x.Name.ToLower().StartsWith(namePrefix.ToLower())).ToList();
        }
    }
}
