using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot
{
    internal interface IToDoService
    {
        // Возвращает все ToDoItem для UserId
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);

        // Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);

        // Добавляет ToDoItem в общий Список и возвращает добавленный ToDoItem
        ToDoItem Add(ToDoUser user, string name);

        // Изменяет ToDoItem в общем Списоке по Guid сотояние с Active на Completed 
        void MarkCompleted(Guid id);

        // Удаляет ToDoItem из общего Списока по Guid
        void Delete(Guid id);

        // Задает ограничения для ToDoItem
        void SetConfiguration(int maxTasks, int maxLengthTask);
    }
}
