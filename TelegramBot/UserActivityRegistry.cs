using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookingBot.TelegramBot
{
    /// <summary>
    /// Состояние пользователя вне сценариев (простой конечный автомат):
    /// чего бот ждёт от пользователя в следующем текстовом сообщении.
    /// </summary>
    public enum HandlerState
    {
        /// <summary>Ожидает команды /start.</summary>
        AwaitingStart,
        /// <summary>Ожидает «Y» для регистрации.</summary>
        AwaitingRegistration,
        /// <summary>Ожидает имя.</summary>
        AwaitingRegistrationName,
        /// <summary>Ожидает новое имя.</summary>
        AwaitingChangeName,
        /// <summary>Ожидает новое значение лимита.</summary>
        AwaitingConfigLimit,
        /// <summary>Готов к обычной работе.</summary>
        Ready,
        /// <summary>Ожидает имя для /find.</summary>
        AwaitingFindName,
        /// <summary>Ожидает имя для /findall.</summary>
        AwaitingFindAllName
    }

    /// <summary>
    /// Реестр активности пользователей в памяти: блокировки обновлений,
    /// состояния вне сценариев, цели смены имени и цели изменения лимитов.
    /// Создаётся один экземпляр в Program.cs и передаётся в UpdateHandler
    /// (основная работа с записями) и в CleanupBackgroundTask (авточистка
    /// записей пользователей, отсутствующих в базе данных).
    /// </summary>
    public sealed class UserActivityRegistry
    {
        private readonly ConcurrentDictionary<long, SemaphoreSlim> _userLocks = new();
        private readonly ConcurrentDictionary<long, HandlerState> _states = new();
        private readonly ConcurrentDictionary<long, Guid> _changeNameTargets = new();
        private readonly ConcurrentDictionary<long, string> _configLimitTargets = new();

        /// <summary>Возвращает персональную блокировку обновлений пользователя, создавая её при первом обращении.</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        /// <returns>Блокировка (семафор) пользователя.</returns>
        public SemaphoreSlim GetOrAddLock(long userId)
        {
            return _userLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
        }

        /// <summary>Устанавливает состояние пользователя (вне сценариев).</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        /// <param name="newState">Новое состояние.</param>
        public void SetState(long userId, HandlerState newState)
        {
            _states[userId] = newState;
        }

        /// <summary>Возвращает состояние пользователя; по умолчанию — AwaitingStart.</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        /// <returns>Текущее состояние пользователя.</returns>
        public HandlerState GetState(long userId)
        {
            return _states.TryGetValue(userId, out HandlerState state) ? state : HandlerState.AwaitingStart;
        }

        /// <summary>Запоминает пользователя, чьё имя будет меняться.</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram (кто меняет).</param>
        /// <param name="targetUserId">Идентификатор пользователя в БД (кого меняют).</param>
        public void SetChangeNameTarget(long userId, Guid targetUserId)
        {
            _changeNameTargets[userId] = targetUserId;
        }

        /// <summary>Возвращает цель смены имени, если пользователь её задавал.</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        /// <param name="targetUserId">Найденная цель смены имени.</param>
        /// <returns>true, если цель задана.</returns>
        public bool TryGetChangeNameTarget(long userId, out Guid targetUserId)
        {
            return _changeNameTargets.TryGetValue(userId, out targetUserId);
        }

        /// <summary>Удаляет цель смены имени пользователя.</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        public void RemoveChangeNameTarget(long userId)
        {
            _changeNameTargets.TryRemove(userId, out _);
        }

        /// <summary>Запоминает название лимита, который будет менять пользователь.</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        /// <param name="targetName">Название лимита (ключ appsettings.json).</param>
        public void SetConfigLimitTarget(long userId, string targetName)
        {
            _configLimitTargets[userId] = targetName;
        }

        /// <summary>Возвращает название лимита, который меняет пользователь; null, если цель не задана.</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        /// <returns>Название лимита или null.</returns>
        public string? GetConfigLimitTarget(long userId)
        {
            return _configLimitTargets.TryGetValue(userId, out string? targetName) ? targetName : null;
        }

        /// <summary>Удаляет цель изменения лимита пользователя.</summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        public void RemoveConfigLimitTarget(long userId)
        {
            _configLimitTargets.TryRemove(userId, out _);
        }

        /// <summary>
        /// Возвращает идентификаторы всех пользователей, у которых есть хотя бы
        /// одна запись в реестре (объединение ключей всех словарей).
        /// </summary>
        /// <returns>Список идентификаторов пользователей.</returns>
        public IReadOnlyList<long> GetUserIds()
        {
            return _userLocks.Keys
                .Union(_states.Keys)
                .Union(_changeNameTargets.Keys)
                .Union(_configLimitTargets.Keys)
                .ToList();
        }

        /// <summary>
        /// Удаляет все записи пользователя из реестра (блокировку, состояние,
        /// цели смены имени и лимита).
        /// </summary>
        /// <param name="userId">Идентификатор пользователя в Telegram.</param>
        /// <returns>Количество удалённых записей.</returns>
        public int RemoveUser(long userId)
        {
            int removed = 0;

            if (_userLocks.TryRemove(userId, out _)) removed++;
            if (_states.TryRemove(userId, out _)) removed++;
            if (_changeNameTargets.TryRemove(userId, out _)) removed++;
            if (_configLimitTargets.TryRemove(userId, out _)) removed++;

            return removed;
        }
    }
}
