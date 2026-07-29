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
        private int _maxTasks = 0;
        private int _maxLengthTask = 0;
        private readonly IToDoRepository _toDoRepository;

        public ToDoService() { }

        public ToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = (IToDoRepository?)toDoRepository;
        }

        private void CheckCounthLimit(Guid userId)
        {
            if (_toDoRepository.CountActive(userId).Result >= _maxTasks)
                throw new TaskCountLimitException(_maxTasks);
        }

        private void CheckLengthLimits(string name)
        {
            if (name.Length > _maxLengthTask)
            {
                throw new TaskLengthLimitException(name.Length, _maxLengthTask);
            }
        }

        private void CheckDuplicate(Guid userId, string name)
        {
            if (_toDoRepository.ExistsByName(userId, name).Result)
            {
                throw new DuplicateTaskException(name);
            }
        }

        public async Task<ToDoItem> Add(ToDoUser user, string name)
        {
            CheckCounthLimit(user.UserId);
            if (name.Length > 0)
            {
                CheckLengthLimits(name);
                CheckDuplicate(user.UserId, name);
            }

            await _toDoRepository.Add(new ToDoItem(user, name));

            return _toDoRepository.Find(user.UserId, x => x.Name == name).Result.First();
        }

        public async Task Delete(Guid id)
        {
            _toDoRepository.Delete(id);

            return;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId)
        {
            return _toDoRepository.GetActiveByUserId(userId).Result;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId)
        {
            return _toDoRepository.GetAllByUserId(userId).Result;
        }

        public async Task MarkCompleted(Guid id)
        {
            if (id != default(Guid))
            {
                ToDoItem updateItem = _toDoRepository.Get(id).Result;
                updateItem.State = ToDoItem.ToDoItemState.Completed;
                _toDoRepository.Update(updateItem);
            }

            return;
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

        public async Task SetConfiguration(int maxTasks, int maxLengthTask)
        {
            _maxTasks = maxTasks;
            _maxLengthTask = maxLengthTask;

            return;
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix)
        {
            return _toDoRepository.Find(user.UserId, x => x.Name.ToLower().StartsWith(namePrefix.ToLower())).Result.ToList();
        }
    }
}
