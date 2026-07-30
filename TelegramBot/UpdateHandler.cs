using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using CookingBot.Core.Entities;
using CookingBot.Core.Exceptions;
using CookingBot.Core.Services;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace CookingBot.TelegramBot
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _todoService;
        private readonly IToDoReportService _toDoReportService;
        public string displayName = "Гость";
        public int maxTask = 0;
        public int maxLengthTask = 0;
        string? str;
        
        public UpdateHandler(IUserService userService, IToDoService todoService, IToDoReportService toDoReportService)
        {
            _userService = userService;
            _todoService = todoService;
            _toDoReportService = toDoReportService;
        }

        public Task HandleErrorAsync(ITelegramBotClient telegramBotClient, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"HandleError: {exception})");
            return Task.CompletedTask;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            await telegramBotClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'", ct);

            try
            {                
                ToDoUser _someUser = new ToDoUser(update.Message.From.Id, update.Message.From.Username!);
                await GreetingAsync(telegramBotClient, update, ct);

                //Console.Clear();

                do
                {
                    try
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, " Для начала введите максимально допустимое количество задач в диапазоне от 1 до 100: ", ct);
                        str = Console.ReadLine();
                        ValidateString(str);
                        maxTask = ParseAndValidateInt(str, 1, 100);
                    }
                    catch (ArgumentException e)
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, $"{e.Message}", ct);
                    }
                }
                while (maxTask <= 0);

                str = null;

                do
                {
                    try
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, " А теперь введите максимально допустимую длину задачи в диапазоне от 1 до 100: ", ct);
                        str = Console.ReadLine();
                        ValidateString(str);
                        maxLengthTask = ParseAndValidateInt(str, 1, 100);
                    }
                    catch (ArgumentException e)
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, $"{e.Message}", ct);
                    }
                }
                while (maxLengthTask <= 0);

                await _todoService.SetConfigurationAsync(maxTask, maxLengthTask);

                await WorkAsync(telegramBotClient, update, ct);

                Console.ReadLine();
                //return;
            }
            catch (Exception ex)
            {
                await telegramBotClient.SendMessage(update.Message.Chat, $" Произошла непредвиденная ошибка:\n Тип ошибки: {ex.GetType().Name}\n Сообщение: {ex.Message}", ct);
                var trace = new StackTrace(ex, true);
                foreach (var item in trace.GetFrames())
                {
                    await telegramBotClient.SendMessage(update.Message.Chat, $"Файл: {item.GetFileName()}, Строка: {item.GetFileLineNumber()}, Метод: {item.GetMethod()}", ct);
                }
                if (ex.InnerException != null)
                {
                    await telegramBotClient.SendMessage(update.Message.Chat, $" Внутреннее исключение:\n Тип: {ex.InnerException.GetType().Name}\n Сообщение: {ex.InnerException.Message}\n", ct);
                    var newtrace = new StackTrace(ex.InnerException, true);
                    foreach (var item in newtrace.GetFrames())
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, $"Файл: {item.GetFileName()}, Строка: {item.GetFileLineNumber()}, Метод: {item.GetMethod()}", ct);
                    }
                }
            }
        }

        private void ValidateString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException($"{0} это значение не соответствует требованиям", str);
        }

        private int ParseAndValidateInt(string str, int min, int max)
        {
            int answ = 0;

            if (!int.TryParse(str, out answ) || answ < min || answ > max)
                throw new ArgumentException($"{0} это значение не соответствует требованиям", str);

            return answ;
        }

        private async Task GreetingAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            //Console.Clear();
            StringBuilder str = new StringBuilder("\n");
            str.AppendLine(" Приветствую Вас в проекте \"Кулинарный бот\"\n");
            str.AppendLine(" Бот поддерживает следующие команды при старте:");
            str.AppendLine(" \"/start\" - используется для начала работы");
            str.AppendLine(" \"/help\" - отображает краткую информацию как пользоваться Ботом, также выводит список доступных команд во время работы");
            str.AppendLine(" \"/info\" - предоставляет информацию о версии программы и дате её создания");
            str.AppendLine(" \"/addtask Задача\" - позволяет добавить Задачу, между командой и Задачей обязательно должен быть пробел");
            str.AppendLine(" \"/showtasks\" - отображает все \"Активные\" задачи");
            str.AppendLine(" \"/showalltasks\" - отображает все задачи");
            str.AppendLine(" \"/removetask Идентификатор\" - позволяет удалить доступную задачу по ее Идентификатору, между командой и Идентификатором обязательно должен быть пробел");
            str.AppendLine(" \"/completetask Идентификатор\" - позволяет изменить состояние задачи с \"Активная\" на \"Завершенная\", между командой и Идентификатором обязательно должен быть пробел");
            str.AppendLine(" \"/report\" - отображает статистику по задачам текущего пользователя на данный момент времени");
            str.AppendLine(" \"/find Имя\" - отображает все задачи зарегистрированного пользователя с именем \"Имя\", между командой и Именем обязательно должен быть пробел");
            str.AppendLine(" \"/exit\" - завершение работы\n");
            str.AppendLine(" В процессе работы перечень доступных команд будет меняться");
            str.AppendLine(" Команды следует вводить с клавиатуры в Консоль");
            str.AppendLine(" Окончанием ввода команды считается нажатие клавиши Enter");
            str.AppendLine(" Некоторым командам потребуются дополнительные данные, об этом будет указано в описании команды\n");
            str.AppendLine(" Давайте попробуем?");
            str.Append(" Для продолжения нажмите Enter");

            await telegramBotClient.SendMessage(update.Message.Chat, str.ToString(), ct);
            Console.ReadLine();
        }

        private async Task WorkAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            string command = string.Empty;

            do
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    await telegramBotClient.SendMessage(update.Message.Chat, $"{displayName}, Введите команду: ", ct);
                    command = Console.ReadLine().ToLower();
                }

                switch (command)
                {
                    case "/start":
                        await MyNameIsAsync(telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/help":
                        await HelpAsync(telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/info":
                        await InfoAsync(telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    default:
                        await telegramBotClient.SendMessage(update.Message.Chat, " Бот не знает такой команды либо эта команда недоступна\n Для просмотра доступных команд введите \"/help\"", ct);
                        command = string.Empty;
                        break;
                }
            }
            while (string.IsNullOrWhiteSpace(command));
        }

        private async Task UserRegistrationAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            str = string.Empty;
            ToDoUser newUser;

            await telegramBotClient.SendMessage(update.Message.Chat, $" Ваше отображаемое Имя \"{update.Message.From.Username}\" ", ct);
            await telegramBotClient.SendMessage(update.Message.Chat, $" Если хотите изменить, введите новое Имя. Если нет, просто нажмите Enter ", ct);
            str = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(str))
            {
                newUser = await _userService.RegisterUserAsync(update.Message.From.Id, update.Message.From.Username);
            }
            else
            {
                newUser = await _userService.RegisterUserAsync(update.Message.From.Id, str);
            }
            await telegramBotClient.SendMessage(update.Message.Chat, " Зарегистрирован новый Пользователь", ct);
            await telegramBotClient.SendMessage(update.Message.Chat, $" UserId: {newUser.UserId}", ct);
            await telegramBotClient.SendMessage(update.Message.Chat, $" TelegramUserId: {newUser.TelegramUserId}", ct);
            await telegramBotClient.SendMessage(update.Message.Chat, $" TelegramUserName: {newUser.TelegramUserName}", ct);
            await telegramBotClient.SendMessage(update.Message.Chat, $" Registered Date: {newUser.RegisteredAt}", ct);
            displayName = newUser.TelegramUserName;
        }

        private async Task MyNameIsAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            //Console.Clear();
            ToDoUser? user = await _userService.GetUserAsync(update.Message.From.Id);

            if (user == null)
            {
                await telegramBotClient.SendMessage(update.Message.Chat, " Вы еще не зарегистрированы. Хотите принять участие в проекте \"Кулинарный Бот\"?", ct);
                await telegramBotClient.SendMessage(update.Message.Chat, " Для регистрации введите \"Y\" ", ct);
                str = Console.ReadLine().ToLower();
                if (str == "y")
                {
                    await UserRegistrationAsync(telegramBotClient, update, ct);
                }
                else
                {
                    return;
                }
            }
            else
            {
                await telegramBotClient.SendMessage(update.Message.Chat, $" {user.TelegramUserName} Добро пожаловать", ct);
            }

            string command = string.Empty;
            string inputStr = string.Empty;

            do
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    await telegramBotClient.SendMessage(update.Message.Chat, $"{displayName}, Введите команду: ", ct);
                    inputStr = Console.ReadLine().ToLower();
                    command = inputStr.Split(' ')[0];
                }

                switch (command)
                {
                    case "/help":
                        await HelpAsync(telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/info":
                        await InfoAsync(telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/addtask":
                        await AddTaskAsync(inputStr, telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/showtasks":
                        await ShowTasksAsync(telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/showalltasks":
                        await ShowAllTasksAsync(telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/removetask":
                        await RemoveTaskAsync(inputStr, telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/completetask":
                        await CompleteTaskAsync(inputStr, telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/report":
                        await ReportAsync(telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/find":
                        await FindAsync(inputStr, telegramBotClient, update, ct);
                        command = string.Empty;
                        break;
                    case "/exit":
                        Environment.Exit(0);
                        break;
                    default:
                        await telegramBotClient.SendMessage(update.Message.Chat, " Бот не знает такой команды либо эта команда недоступна\n Для просмотра доступных команд введите \"/help\"", ct);
                        command = string.Empty;
                        break;
                }
            }
            while (string.IsNullOrWhiteSpace(command));
        }

        private async Task HelpAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            StringBuilder str = new StringBuilder("\n"); ;
            str.AppendLine(" Вам доступны следующие команды:");
            if ((await _userService.GetUserAsync(update.Message.From.Id)) == null)
            {
                str.AppendLine(" \"/start\" - используется для начала работы");
            }
            str.AppendLine(" \"/help\" - отображает краткую информацию как пользоваться Ботом, также выводит список доступных команд во время работы");
            str.AppendLine(" \"/info\" - предоставляет информацию о версии программы и дате её создания");
            if ((await _userService.GetUserAsync(update.Message.From.Id)) != null)
            {
                str.AppendLine(" \"/addtask Задача\" - позволяет добавить Задачу, между командой и Задачей обязательно должен быть пробел");
                str.AppendLine(" \"/showtasks\" - отображает все \"Активные\" задачи");
                str.AppendLine(" \"/showalltasks\" - отображает все задачи");
                str.AppendLine(" \"/removetask Идентификатор\" - позволяет удалить доступную задачу по ее Идентификатору, между командой и Идентификатором обязательно должен быть пробел");
                str.AppendLine(" \"/completetask Идентификатор\" - позволяет изменить состояние задачи с \"Активная\" на \"Завершенная\", между командой и Идентификатором обязательно должен быть пробел");
                str.AppendLine(" \"/report\" - отображает статистику по задачам текущего пользователя на данный момент времени");
                str.AppendLine(" \"/find Имя\" - отображает все задачи зарегистрированного пользователя с именем \"Имя\", между командой и Именем обязательно должен быть пробел");
            }
            str.AppendLine(" \"/exit\" - завершает работу Бота\n");
            await telegramBotClient.SendMessage(update.Message.Chat, str.ToString(), ct);
        }

        private async Task InfoAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            string createDate = " Created 21.05.2026    ";

            Console.Clear();

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;

            await telegramBotClient.SendMessage(update.Message.Chat, $"{createDate} The Version used {version}", ct);
        }

        private async Task AddTaskAsync(string inputStr, ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            try
            {
                string newTask;
                if (inputStr.Length > 8)
                {
                    newTask = inputStr.Substring(9);
                }
                else
                {
                    newTask = string.Empty;
                }

                Console.Clear();

                if (!string.IsNullOrWhiteSpace(newTask))
                {
                    ToDoItem newToDoItem = await _todoService.AddAsync(await _userService.GetUserAsync(update.Message.From.Id), newTask);

                    StringBuilder str = new StringBuilder();
                    str.AppendLine($" Задача добавлена:\n");
                    str.AppendLine($" Id: {newToDoItem.Id}\n");
                    str.AppendLine($" User:\tUserId: {newToDoItem.User.UserId}\n");
                    str.AppendLine($" \t\tTelegramUserId: {newToDoItem.User.TelegramUserId}\n");
                    str.AppendLine($" \t\tTelegramUserName: {newToDoItem.User.TelegramUserName}\n");
                    str.AppendLine($" \t\tRegistered Date: {newToDoItem.User.RegisteredAt}\n");
                    str.AppendLine($" Name: {newToDoItem.Name}\n");
                    str.AppendLine($" CreatedAt: {newToDoItem.CreatedAt}\n");
                    str.AppendLine($" State: {newToDoItem.State}\n");
                    str.AppendLine($" StateCangedAt: {newToDoItem.StateCangedAt}\n");
                    await telegramBotClient.SendMessage(update.Message.Chat, str.ToString(), ct);
                }
                else
                {
                    await telegramBotClient.SendMessage(update.Message.Chat, " Аргумент для команды \"/addtask\" отсутствует", ct);
                }
            }
            catch (TaskCountLimitException e)
            {
                await telegramBotClient.SendMessage(update.Message.Chat, $"Превышено максимальное количество задач равное {e.TaskCountLimit}", ct);
            }
            catch (TaskLengthLimitException e)
            {
                await telegramBotClient.SendMessage(update.Message.Chat, $"Длина задачи ‘{e.TaskLength}’ превышает максимально допустимое значение {e.TaskLengthLimit}", ct);
            }
            catch (DuplicateTaskException e)
            {
                await telegramBotClient.SendMessage(update.Message.Chat, $"Задача ‘{e.Task}’ уже существует", ct);
            }            
        }

        private async Task ShowTasksAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            List<ToDoItem> listTasks = (await _todoService.GetAllByUserIdAsync((await _userService.GetUserAsync(update.Message.From.Id)).UserId)).ToList();

            if (listTasks.Count() > 0)
            {
                for (int i = 0; i < listTasks.Count(); i++)
                {
                    if (listTasks[i].State == ToDoItem.ToDoItemState.Active)
                        await telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. {listTasks[i].Name} - {listTasks[i].CreatedAt} - {listTasks[i].Id}", ct);
                }
            }
            else
            {
                await telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст", ct);
            }
        }

        private async Task ShowAllTasksAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            List<ToDoItem> listAllTasks = (await _todoService.GetAllByUserIdAsync((await _userService.GetUserAsync(update.Message.From.Id)).UserId)).ToList();

            if (listAllTasks.Count() > 0)
            {

                for (int i = 0; i < listAllTasks.Count(); i++)
                {
                    if (listAllTasks[i].State == ToDoItem.ToDoItemState.Active)
                        await telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. (Active) {listAllTasks[i].Name} - {listAllTasks[i].CreatedAt} - {listAllTasks[i].Id}", ct);
                    else if (listAllTasks[i].State == ToDoItem.ToDoItemState.Completed)
                        await telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. (Complete) {listAllTasks[i].Name} - {listAllTasks[i].CreatedAt} - {listAllTasks[i].Id}", ct);
                }
            }
            else
            {
                await telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст", ct);
            }
        }

        private async Task CompleteTaskAsync(string inputStr, ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            List<ToDoItem> listTasks = (await _todoService.GetActiveByUserIdAsync((await _userService.GetUserAsync(update.Message.From.Id)).UserId)).ToList();

            if (listTasks.Count() > 0)
            {
                string selectedId;
                if (inputStr.Length > 13)
                {
                    selectedId = inputStr.Substring(14);
                }
                else
                {
                    selectedId = string.Empty;
                }

                Guid num = default(Guid);

                if (Guid.TryParse(selectedId, out num))
                {
                    if (num == default(Guid))
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, " Необходимо ввести именно Идентификатор задачи, попробуйте еще раз: ", ct);
                    }
                    else if (listTasks.Where(w => w.Id == num).Count() == 0)
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, " Задачи с таким номером нет, попробуйте еще раз: ", ct);
                    }
                    else
                    {
                        await _todoService.MarkCompletedAsync(num);

                        await telegramBotClient.SendMessage(update.Message.Chat, $" Команда с Именем \"{listTasks.Where(w => w.Id == num).FirstOrDefault().Name}\" выполнена", ct);
                    }
                }
                else
                {
                    await telegramBotClient.SendMessage(update.Message.Chat, " Аргумент для команды \"/completetask\" отсутствует", ct);
                }
            }
            else
            {
                await telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст", ct);
            }
        }

        private async Task RemoveTaskAsync(string inputStr, ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            List<ToDoItem> listAllTasks = (await _todoService.GetAllByUserIdAsync((await _userService.GetUserAsync(update.Message.From.Id)).UserId)).ToList();

            if (listAllTasks.Count() > 0)
            {
                string selectedId;
                if (inputStr.Length > 11)
                {
                    selectedId = inputStr.Substring(12);
                }
                else
                {
                    selectedId = string.Empty;
                }

                Guid num = default(Guid);

                if (Guid.TryParse(selectedId, out num))
                {
                    if (num == default(Guid))
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, " Необходимо ввести именно Идентификатор задачи, попробуйте еще раз: ", ct);
                    }
                    else if (listAllTasks.Where(w => w.Id == num).Count() == 0)
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, " Задачи с таким номером нет, попробуйте еще раз: ", ct);
                    }
                    else
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, $@" Задача ""{listAllTasks.FirstOrDefault(w => w.Id == num).Name}"" удалена", ct);
                        await _todoService.DeleteAsync(num);
                    }
                }
                await ShowAllTasksAsync(telegramBotClient, update, ct);
            }
            else
            {
                await telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст", ct);
            }
        }

        private async Task ReportAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            var (total, completed, active, generatedAt) = await _toDoReportService.GetUserStatsAsync((await _userService.GetUserAsync(update.Message.From.Id)).UserId);
            string _generatedAt = generatedAt.ToShortDateString();
            int _total = total;
            int _active = active;
            int _completed = completed;

            await telegramBotClient.SendMessage(update.Message.Chat, $" Статистика по задачам на {_generatedAt}", ct);
            await telegramBotClient.SendMessage(update.Message.Chat, $" Всего: {_total}", ct);
            await telegramBotClient.SendMessage(update.Message.Chat, $" Завершенных: {_completed}", ct);
            await telegramBotClient.SendMessage(update.Message.Chat, $" Активных: {_active}", ct);
        }

        private async Task FindAsync(string inputStr, ITelegramBotClient telegramBotClient, Update update, CancellationToken ct)
        {
            string namePrefix;
            if (inputStr.Length > 5)
            {
                namePrefix = inputStr.Substring(6);
            }
            else
            {
                namePrefix = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(namePrefix))
            {
                ToDoUser user = await _userService.GetUserAsync(update.Message.From.Id);
                List<ToDoItem> listTasks = (await _todoService.FindAsync(user, namePrefix)).ToList();

                if (listTasks.Count() > 0)
                {
                    for (int i = 0; i < listTasks.Count(); i++)
                    {
                        await telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. {listTasks[i].Name} - {listTasks[i].CreatedAt} - {listTasks[i].Id}", ct);
                    }
                }
                else
                {
                    await telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст", ct);
                }
            }
            else
            {
                await telegramBotClient.SendMessage(update.Message.Chat, " Аргумент для команды \"/find\" отсутствует", ct);
            }
        }
    }
}
