using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot.Core.Services
{
    internal class ToDoReportService : IToDoReportService
    {
        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            int _total = 0;
            int _completed = 0;
            int _active = 0;
            DateTime _generateAt = DateTime.Now;

            return (total: _total, completed: _completed, active: _active, generatedAt: _generateAt);
        }
    }
}
