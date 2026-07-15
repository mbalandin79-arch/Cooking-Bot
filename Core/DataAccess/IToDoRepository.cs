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
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);

        // Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);

        // Возвращает все задачи пользователя, которые удовлетворяют предикате
        IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate);

        ToDoItem? Get(Guid id);

        void Add(ToDoItem item);

        void Update(ToDoItem item);

        void Delete(Guid id);

        // Проверяет есть ли задача с таким именем у пользователя
        bool ExistsByName(Guid userId, string name);

        // Возвращает количество активных задач у пользователя
        int CountActive(Guid userId);
    }
}
