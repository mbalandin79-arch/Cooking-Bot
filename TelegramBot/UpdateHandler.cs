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
        string str;

        public UpdateHandler() { }

        public UpdateHandler(IUserService userService, IToDoService todoService)
        {
            _userService = (IUserService?)userService;
            _todoService = (IToDoService?)todoService;
        }

        public UpdateHandler(IUserService userService, IToDoService todoService, IToDoReportService toDoReportService)
        {
            _userService = (IUserService?)userService;
            _todoService = (IToDoService?)todoService;
            _toDoReportService = (IToDoReportService)toDoReportService;
        }

        public void HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update)
        {
            try
            {
                ToDoUser _someUser = new ToDoUser(update.Message.From.Id, update.Message.From.Username!);
                Greeting(telegramBotClient, update);

                Console.Clear();

                do
                {
                    try
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, " Для начала введите максимально допустимое количество задач в диапазоне от 1 до 100: ");
                        str = Console.ReadLine();
                        ValidateString(str);
                        maxTask = ParseAndValidateInt(str, 1, 100);
                    }
                    catch (ArgumentException e)
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, $"{e.Message}");
                    }
                }
                while (maxTask <= 0);

                str = null;

                do
                {
                    try
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, " А теперь введите максимально допустимую длину задачи в диапазоне от 1 до 100: ");
                        str = Console.ReadLine();
                        ValidateString(str);
                        maxLengthTask = ParseAndValidateInt(str, 1, 100);
                    }
                    catch (ArgumentException e)
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, $"{e.Message}");
                    }
                }
                while (maxLengthTask <= 0);

                _todoService.SetConfiguration(maxTask, maxLengthTask);

                Work(telegramBotClient, update);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                telegramBotClient.SendMessage(update.Message.Chat, $" Произошла непредвиденная ошибка:\n Тип ошибки: {ex.GetType().Name}\n Сообщение: {ex.Message}");
                var trace = new StackTrace(ex, true);
                foreach (var item in trace.GetFrames())
                {
                    telegramBotClient.SendMessage(update.Message.Chat, $"Файл: {item.GetFileName()}, Строка: {item.GetFileLineNumber()}, Метод: {item.GetMethod()}");
                }
                if (ex.InnerException != null)
                {
                    telegramBotClient.SendMessage(update.Message.Chat, $" Внутреннее исключение:\n Тип: {ex.InnerException.GetType().Name}\n Сообщение: {ex.InnerException.Message}\n");
                    var newtrace = new StackTrace(ex.InnerException, true);
                    foreach (var item in newtrace.GetFrames())
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, $"Файл: {item.GetFileName()}, Строка: {item.GetFileLineNumber()}, Метод: {item.GetMethod()}");
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

        private void Greeting(ITelegramBotClient telegramBotClient, Update update)
        {
            Console.Clear();
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
            telegramBotClient.SendMessage(update.Message.Chat, str.ToString());
            Console.ReadLine();
        }

        private void Work(ITelegramBotClient telegramBotClient, Update update)
        {
            string command = string.Empty;
            string inputStr = string.Empty;

            do
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    telegramBotClient.SendMessage(update.Message.Chat, $"{displayName}, Введите команду: ");
                    inputStr = Console.ReadLine().ToLower();
                    command = inputStr.Split(' ')[0];
                }

                switch (command)
                {
                    case "/start":
                        MyNameIs(telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/help":
                        Help(telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/info":
                        Info(telegramBotClient, update);
                        command = string.Empty;
                        break;
                    default:
                        telegramBotClient.SendMessage(update.Message.Chat, " Бот не знает такой команды либо эта команда недоступна\n Для просмотра доступных команд введите \"/help\"");
                        command = string.Empty;
                        break;
                }
            }
            while (string.IsNullOrWhiteSpace(command));
        }

        private void UserRegistration(ITelegramBotClient telegramBotClient, Update update)
        {
            str = string.Empty;

            telegramBotClient.SendMessage(update.Message.Chat, $" Ваше отображаемое Имя \"{update.Message.From.Username}\" ");
            telegramBotClient.SendMessage(update.Message.Chat, $" Если хотите изменить, введите новое Имя. Если нет, просто нажмите Enter ");
            str = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(str))
            {
                _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username);
            }
            else
            {
                _userService.RegisterUser(update.Message.From.Id, str);
            }
            telegramBotClient.SendMessage(update.Message.Chat, " Зарегистрирован новый Пользователь");
            telegramBotClient.SendMessage(update.Message.Chat, $" UserId: {_userService.GetUser(update.Message.From.Id).UserId}");
            telegramBotClient.SendMessage(update.Message.Chat, $" TelegramUserId: {_userService.GetUser(update.Message.From.Id).TelegramUserId}");
            telegramBotClient.SendMessage(update.Message.Chat, $" TelegramUserName: {_userService.GetUser(update.Message.From.Id).TelegramUserName}");
            telegramBotClient.SendMessage(update.Message.Chat, $" Registered Date: {_userService.GetUser(update.Message.From.Id).RegisteredAt}");
            displayName = _userService.GetUser(update.Message.From.Id).TelegramUserName;
        }

        private void MyNameIs(ITelegramBotClient telegramBotClient, Update update)
        {
            Console.Clear();

            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                telegramBotClient.SendMessage(update.Message.Chat, " Вы еще не зарегистрированы. Хотите принять участие в проекте \"Кулинарный Бот\"?");
                telegramBotClient.SendMessage(update.Message.Chat, " Для регистрации введите \"Y\" ");
                str = Console.ReadLine().ToLower();
                if (str == "y")
                {
                    UserRegistration(telegramBotClient, update);
                }
                else
                {
                    return;
                }
            }
            else
            {
                telegramBotClient.SendMessage(update.Message.Chat, $" {_userService.GetUser(update.Message.From.Id).TelegramUserName} Добро пожаловать");
            }

            string command = string.Empty;
            string inputStr = string.Empty;

            do
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    telegramBotClient.SendMessage(update.Message.Chat, $"{displayName}, Введите команду: ");
                    inputStr = Console.ReadLine().ToLower();
                    command = inputStr.Split(' ')[0];
                }

                switch (command)
                {
                    case "/help":
                        Help(telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/info":
                        Info(telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/addtask":
                        AddTask(inputStr, telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/showtasks":
                        ShowTasks(telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/showalltasks":
                        ShowAllTasks(telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/removetask":
                        RemoveTask(inputStr, telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/completetask":
                        CompleteTask(inputStr, telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/report":
                        Report(telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/find":
                        Find(inputStr, telegramBotClient, update);
                        command = string.Empty;
                        break;
                    case "/exit":
                        Environment.Exit(0);
                        break;
                    default:
                        telegramBotClient.SendMessage(update.Message.Chat, " Бот не знает такой команды либо эта команда недоступна\n Для просмотра доступных команд введите \"/help\"");
                        command = string.Empty;
                        break;
                }
            }
            while (string.IsNullOrWhiteSpace(command));
        }

        private void Help(ITelegramBotClient telegramBotClient, Update update)
        {
            StringBuilder str = new StringBuilder("\n"); ;
            str.AppendLine(" Вам доступны следующие команды:");
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                str.AppendLine(" \"/start\" - используется для начала работы");
            }
            str.AppendLine(" \"/help\" - отображает краткую информацию как пользоваться Ботом, также выводит список доступных команд во время работы");
            str.AppendLine(" \"/info\" - предоставляет информацию о версии программы и дате её создания");
            if (_userService.GetUser(update.Message.From.Id) != null)
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
            telegramBotClient.SendMessage(update.Message.Chat, str.ToString());
        }

        private void Info(ITelegramBotClient telegramBotClient, Update update)
        {
            string createDate = " Created 21.05.2026    ";

            Console.Clear();

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;

            telegramBotClient.SendMessage(update.Message.Chat, $"{createDate} The Version used {version}");
        }

        private void AddTask(string inputStr, ITelegramBotClient telegramBotClient, Update update)
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
                    ToDoItem newToDoItem = _todoService.Add(_userService.GetUser(update.Message.From.Id), newTask);

                    telegramBotClient.SendMessage(update.Message.Chat, $" Задача добавлена:");
                    telegramBotClient.SendMessage(update.Message.Chat, $" Id: {newToDoItem.Id}");
                    telegramBotClient.SendMessage(update.Message.Chat, $" User:\tUserId: {newToDoItem.User.UserId}");
                    telegramBotClient.SendMessage(update.Message.Chat, $" \t\tTelegramUserId: {newToDoItem.User.TelegramUserId}");
                    telegramBotClient.SendMessage(update.Message.Chat, $" \t\tTelegramUserName: {newToDoItem.User.TelegramUserName}");
                    telegramBotClient.SendMessage(update.Message.Chat, $" \t\tRegistered Date: {newToDoItem.User.RegisteredAt}");
                    telegramBotClient.SendMessage(update.Message.Chat, $" Name: {newToDoItem.Name}");
                    telegramBotClient.SendMessage(update.Message.Chat, $" CreatedAt: {newToDoItem.CreatedAt}");
                    telegramBotClient.SendMessage(update.Message.Chat, $" State: {newToDoItem.State}");
                    telegramBotClient.SendMessage(update.Message.Chat, $" StateCangedAt: {newToDoItem.StateCangedAt}");
                }
                else
                {
                    telegramBotClient.SendMessage(update.Message.Chat, " Аргумент для команды \"/addtask\" отсутствует");
                }
            }
            catch (TaskCountLimitException e)
            {
                telegramBotClient.SendMessage(update.Message.Chat, $"Превышено максимальное количество задач равное {e.TaskCountLimit}");
            }
            catch (TaskLengthLimitException e)
            {
                telegramBotClient.SendMessage(update.Message.Chat, $"Длина задачи ‘{e.TaskLength}’ превышает максимально допустимое значение {e.TaskLengthLimit}");
            }
            catch (DuplicateTaskException e)
            {
                telegramBotClient.SendMessage(update.Message.Chat, $"Задача ‘{e.Task}’ уже существует");
            }
        }

        private void ShowTasks(ITelegramBotClient telegramBotClient, Update update)
        {
            List<ToDoItem> listTasks = _todoService.GetAllByUserId(_userService.GetUser(update.Message.From.Id).UserId).ToList();

            if (listTasks.Count() > 0)
            {
                for (int i = 0; i < listTasks.Count(); i++)
                {
                    if (listTasks[i].State == ToDoItem.ToDoItemState.Active)
                        telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. {listTasks[i].Name} - {listTasks[i].CreatedAt} - {listTasks[i].Id}");
                }
            }
            else
            {
                telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст");
            }
        }

        private void ShowAllTasks(ITelegramBotClient telegramBotClient, Update update)
        {
            List<ToDoItem> listAllTasks = _todoService.GetAllByUserId(_userService.GetUser(update.Message.From.Id).UserId).ToList();

            if (listAllTasks.Count() > 0)
            {

                for (int i = 0; i < listAllTasks.Count(); i++)
                {
                    if (listAllTasks[i].State == ToDoItem.ToDoItemState.Active)
                        telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. (Active) {listAllTasks[i].Name} - {listAllTasks[i].CreatedAt} - {listAllTasks[i].Id}");
                    else if (listAllTasks[i].State == ToDoItem.ToDoItemState.Completed)
                        telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. (Complete) {listAllTasks[i].Name} - {listAllTasks[i].CreatedAt} - {listAllTasks[i].Id}");
                }
            }
            else
            {
                telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст");
            }
        }

        private void CompleteTask(string inputStr, ITelegramBotClient telegramBotClient, Update update)
        {
            List<ToDoItem> listTasks = _todoService.GetActiveByUserId(_userService.GetUser(update.Message.From.Id).UserId).ToList();

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
                        telegramBotClient.SendMessage(update.Message.Chat, " Необходимо ввести именно Идентификатор задачи, попробуйте еще раз: ");
                    }
                    else if (listTasks.Where(w => w.Id == num).Count() == 0)
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, " Задачи с таким номером нет, попробуйте еще раз: ");
                    }
                    else
                    {
                        _todoService.MarkCompleted(num);

                        telegramBotClient.SendMessage(update.Message.Chat, $" Команда с Именем \"{listTasks.Where(w => w.Id == num).FirstOrDefault().Name}\" выполнена");
                    }
                }
                else
                {
                    telegramBotClient.SendMessage(update.Message.Chat, " Аргумент для команды \"/completetask\" отсутствует");
                }
            }
            else
            {
                telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст");
            }
        }

        private void RemoveTask(string inputStr, ITelegramBotClient telegramBotClient, Update update)
        {
            List<ToDoItem> listAllTasks = _todoService.GetAllByUserId(_userService.GetUser(update.Message.From.Id).UserId).ToList();

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
                        telegramBotClient.SendMessage(update.Message.Chat, " Необходимо ввести именно Идентификатор задачи, попробуйте еще раз: ");
                    }
                    else if (listAllTasks.Where(w => w.Id == num).Count() == 0)
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, " Задачи с таким номером нет, попробуйте еще раз: ");
                    }
                    else
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, $" Задача \"{listAllTasks.Where(w => w.Id == num).FirstOrDefault().Name}\" удалена");
                        _todoService.Delete(num);
                    }
                }
                ShowAllTasks(telegramBotClient, update);
            }
            else
            {
                telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст");
            }
        }

        private void Report(ITelegramBotClient telegramBotClient, Update update)
        {
            var tempReportService = _toDoReportService.GetUserStats(_userService.GetUser(update.Message.From.Id).UserId);
            string _generatedAt = tempReportService.generatedAt.ToShortDateString();
            int _total = tempReportService.total;
            int _active = tempReportService.active;
            int _completed = tempReportService.completed;

            telegramBotClient.SendMessage(update.Message.Chat, $" Статистика по задачам на {_generatedAt}");
            telegramBotClient.SendMessage(update.Message.Chat, $" Всего: {_total}");
            telegramBotClient.SendMessage(update.Message.Chat, $" Завершенных: {_completed}");
            telegramBotClient.SendMessage(update.Message.Chat, $" Активных: {_active}");
        }

        private void Find(string inputStr, ITelegramBotClient telegramBotClient, Update update)
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
                List<ToDoItem> listTasks = _todoService.Find(_userService.GetUser(update.Message.From.Id), namePrefix).ToList();

                if (listTasks.Count() > 0)
                {
                    for (int i = 0; i < listTasks.Count(); i++)
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. {listTasks[i].Name} - {listTasks[i].CreatedAt} - {listTasks[i].Id}");
                    }
                }
                else
                {
                    telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст");
                }
            }
            else
            {
                telegramBotClient.SendMessage(update.Message.Chat, " Аргумент для команды \"/find\" отсутствует");
            }
        }
    }
}
