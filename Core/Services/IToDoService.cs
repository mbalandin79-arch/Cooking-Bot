using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.Entities;

namespace CookingBot.Core.Services
{
    internal interface IToDoService
    {
        // Возвращает все ToDoItem для UserId
        Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId);

        // Возвращает ToDoItem для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId);

        // Добавляет ToDoItem в общий Список и возвращает добавленный ToDoItem
        Task<ToDoItem> Add(ToDoUser user, string name);

        // Изменяет ToDoItem в общем Списоке по Guid сотояние с Active на Completed 
        Task MarkCompleted(Guid id);

        // Удаляет ToDoItem из общего Списока по Guid
        Task Delete(Guid id);

        // Задает ограничения для ToDoItem
        Task SetConfiguration(int maxTasks, int maxLengthTask);

        // Возвращает все задачи пользователя, которые начинаются на namePrefix, использует метод IToDoRepository.Find
        Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix);
    }
}
