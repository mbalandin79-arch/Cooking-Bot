using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.DataAccess;
using CookingBot.Core.Entities;

namespace CookingBot.Core.Services
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IToDoService _todoService;

        public ToDoReportService(IToDoRepository toDoRepository, IToDoService todoService)
        {
            _toDoRepository = toDoRepository;
            _todoService = todoService;
        }

        public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStatsAsync(Guid userId)
        {
            int _total;
            int _completed;
            int _active;
            DateTime _generateAt = DateTime.Now;

            _total = (await _todoService.GetAllByUserIdAsync(userId)).Count() > 0 ? (await _todoService.GetAllByUserIdAsync(userId)).Count() : 0;
            _active = (await _todoService.GetActiveByUserIdAsync(userId)).Count() > 0 ? (await _todoService.GetActiveByUserIdAsync(userId)).Count() : 0;
            _completed = _total > 0 ? _total - _active : 0;

            return (total: _total, completed: _completed, active: _active, generatedAt: _generateAt);
        }
    }
}
