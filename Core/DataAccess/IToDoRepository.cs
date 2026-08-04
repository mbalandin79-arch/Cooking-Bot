using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.Entities;

namespace CookingBot.Core.DataAccess
{
    public interface IToDoRepository
    {
        // Возвращает все задачи пользователя для UserId
        Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId);

        // Возвращает все задачи пользователя для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId);

        // Возвращает все задачи пользователя, которые удовлетворяют предикате
        Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate);

        // Возвращает задачу по id
        Task<ToDoItem?> GetAsync(Guid id);

        // Добавляет задачу
        Task AddAsync(ToDoItem item);

        // Изменяет существующую задачу
        Task UpdateAsync(ToDoItem item);

        // Удаляет задачу по id
        Task DeleteAsync(Guid id);

        // Проверяет есть ли задача с таким именем у пользователя
        Task<bool> ExistsByNameAsync(Guid userId, string name);

        // Возвращает количество активных задач у пользователя
        Task<int> CountActiveAsync(Guid userId);
    }
}
