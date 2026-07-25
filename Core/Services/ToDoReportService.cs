using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CookingBot.Core.DataAccess;

namespace CookingBot.Core.Services
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IToDoService _todoService;

        public ToDoReportService() { }

        public ToDoReportService(IToDoRepository toDoRepository) 
        {
            _toDoRepository = (IToDoRepository?)toDoRepository;
        }

        public ToDoReportService(IToDoRepository toDoRepository, IToDoService todoService)
        {
            _toDoRepository = (IToDoRepository?)toDoRepository;
            _todoService = (IToDoService?)todoService;
        }

        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            int _total;
            int _completed;
            int _active;
            DateTime _generateAt = DateTime.Now;
                        
            _total = _todoService.GetAllByUserId(userId).Count() > 0 ? _todoService.GetAllByUserId(userId).Count() : 0;
            _active = _todoService.GetActiveByUserId(userId).Count() > 0 ? _todoService.GetActiveByUserId(userId).Count() : 0;
            _completed = _total > 0 ? _total - _active : 0;

            return (total: _total, completed: _completed, active: _active, generatedAt: _generateAt);
        }
    }
}
