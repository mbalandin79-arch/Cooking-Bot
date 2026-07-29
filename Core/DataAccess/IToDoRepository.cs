using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.Entities;

namespace CookingBot.Core.DataAccess
{
    internal interface IToDoRepository
    {
        // Возвращает все задачи пользователя для UserId
        Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId);

        // Возвращает все задачи пользователя для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId);

        // Возвращает все задачи пользователя, которые удовлетворяют предикате
        Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate);

        Task<ToDoItem?> Get(Guid id);

        Task Add(ToDoItem item);

        Task Update(ToDoItem item);

        Task Delete(Guid id);

        // Проверяет есть ли задача с таким именем у пользователя
        Task<bool> ExistsByName(Guid userId, string name);

        // Возвращает количество активных задач у пользователя
        Task<int> CountActive(Guid userId);
    }
}
