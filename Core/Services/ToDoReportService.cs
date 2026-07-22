using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.DataAccess;
using CookingBot.Infrastructure.DataAccess;

namespace CookingBot.Core.Services
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly IToDoRepository _toDoRepository;

        public ToDoReportService() { }

        public ToDoReportService(IToDoRepository toDoRepository) 
        {
            _toDoRepository = toDoRepository;
        }

        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            int _total;
            int _completed;
            int _active;
            DateTime _generateAt = DateTime.Now;

            _total = _toDoRepository.GetAllByUserId(userId).Count() > 0 ? _toDoRepository.GetAllByUserId(userId).Count() : 0;
            _active = _toDoRepository.GetActiveByUserId(userId).Count() > 0 ? _toDoRepository.GetActiveByUserId(userId).Count() : 0;
            _completed = _total > 0 ? _total - _active : 0;

            return (total: _total, completed: _completed, active: _active, generatedAt: _generateAt);
        }
    }
}
