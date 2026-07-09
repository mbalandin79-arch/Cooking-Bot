using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace CookingBot
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _todoService;
        public List<ToDoItem> Tasks = new List<ToDoItem>();
        public ToDoUser someUser = null; // потом убрать
        public int maxTask = 0;
        public int maxLengthTask = 0;
        string userName = string.Empty;
        string str;

        public UpdateHandler()
        {

        }

        public UpdateHandler(IUserService userService, IToDoService todoService)
        {
            _userService = (IUserService?)userService;
            _todoService = (IToDoService?)todoService;
        }

        public void HandleUpdateAsync(ITelegramBotClient telegramBotClient, Update update)
        {
            try
            {
                ToDoUser _someUser = new ToDoUser(update.Message.From.Id, update.Message.From.Username!);
                Greeting(telegramBotClient, update);

                do
                {
                    try
                    {
                        Console.Clear();
                        telegramBotClient.SendMessage(update.Message.Chat, " Для начала введите максимально допустимое количество задач в диапазоне от 1 до 100: ");
                        str = Console.ReadLine();
                        ValidateString(str);
                        maxTask = ParseAndValidateInt(str, 1, 100);
                    }
                    catch (ArgumentException e)
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, $"{e.Message}");
                    }
                } while (maxTask <= 0);

                str = null;

                do
                {
                    try
                    {
                        Console.Clear();
                        telegramBotClient.SendMessage(update.Message.Chat, " А теперь введите максимально допустимую длину задачи в диапазоне от 1 до 100: ");
                        str = Console.ReadLine();
                        ValidateString(str);
                        maxLengthTask = ParseAndValidateInt(str, 1, 100);
                    }
                    catch (ArgumentException e)
                    {
                        telegramBotClient.SendMessage(update.Message.Chat, $"{e.Message}");
                    }
                } while (maxLengthTask <= 0);

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

            if (!int.TryParse(str, out answ) || (answ < min || answ > max))
                throw new ArgumentException($"{0} это значение не соответствует требованиям", str);

            return answ;
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

        private void Help(ITelegramBotClient telegramBotClient, Update update)
        {
            StringBuilder str = new StringBuilder("\n"); ;
            str.AppendLine(" Вам доступны следующие команды:");
            str.AppendLine(" \"/start\" - используется для начала работы");
            str.AppendLine(" \"/help\" - отображает краткую информацию как пользоваться Ботом, также выводит список доступных команд во время работы");
            str.AppendLine(" \"/info\" - предоставляет информацию о версии программы и дате её создания");
            if (_userService.GetUser(update.Message.From.Id) != null)
            {
                str.AppendLine(" \"/addtask Задача\" - позволяет добавить Задачу,\n между командой и Задачей обязательно должен быть пробел");
                str.AppendLine(" \"/showtasks\" - отображает все активные задачи");
                str.AppendLine(" \"/showalltasks\" - отображает все доступные задачи");
                str.AppendLine(" \"/removetask\" - позволяет удалить доступную задачу");
                str.AppendLine(" \"/completetask Идентификатор\" - позволяет изменить состояние задачи с \"активная\" на \"завершенная\",\n между командой и Идентификатором обязательно должен быть пробел");
            }
            str.AppendLine(" \"/exit\" - завершает работу Бота\n");
            telegramBotClient.SendMessage(update.Message.Chat, str.ToString());
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
            str.AppendLine(" \"/addtask Задача\" - позволяет добавить Задачу,\n между командой и Задачей обязательно должен быть пробел");
            str.AppendLine(" \"/showtasks\" - отображает все доступные задачи");
            str.AppendLine(" \"/showalltasks\" - отображает все доступные задачи");
            str.AppendLine(" \"/removetask\" - позволяет удалить доступную задачу");
            str.AppendLine(" \"/completetask Идентификатор\" - позволяет изменить состояние задачи с \"активная\" на \"завершенная\",\n между командой и Идентификатором обязательно должен быть пробел");
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

        private void MyNameIs(ITelegramBotClient telegramBotClient, Update update)
        {
            Console.Clear();
            if (_userService.GetUser(update.Message.From.Id) == null)
            {
                _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username);
                telegramBotClient.SendMessage(update.Message.Chat, " Зарегистрирован новый Пользователь");
            }
            else
            {
                telegramBotClient.SendMessage(update.Message.Chat, $" {_userService.GetUser(update.Message.From.Id).TelegramUserName} Добро пожаловать");
            }
        }

        private void Work(ITelegramBotClient telegramBotClient, Update update)
        {
            string command = string.Empty;
            string inputStr = string.Empty;

            do
            {
                if (string.IsNullOrWhiteSpace(command))
                {
                    telegramBotClient.SendMessage(update.Message.Chat, $"{update.Message.From.Username} Введите команду: ");
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
                        RemoveTask();

                        command = string.Empty;
                        break;
                    case "/completetask":
                        CompleteTask(inputStr);

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

        private void CompleteTask(string inputStr)
        {
            Console.Clear();

            if (Tasks.Count() > 0)
            {
                string selectedId = inputStr.Substring(14);

                if (!string.IsNullOrWhiteSpace(selectedId))
                {
                    if (Tasks.Where(w => w.Id.ToString() == selectedId).Count() == 0)
                    {
                        Console.Write(" Команда с Идентификатором ");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(selectedId);
                        Console.ResetColor();
                        Console.WriteLine(" отсутствует");
                    }
                    else
                    {
                        Tasks.Where(w => w.Id.ToString() == selectedId).FirstOrDefault().State = ToDoItem.ToDoItemState.Completed;
                        Tasks.Where(w => w.Id.ToString() == selectedId).FirstOrDefault().CreatedAt = DateTime.UtcNow;

                        Console.Write(" Команда с Именем ");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(Tasks.Where(w => w.Id.ToString() == selectedId).First().Name);
                        Console.ResetColor();
                        Console.WriteLine(" выполнена");
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(someUser.TelegramUserName);
                    Console.ResetColor();
                    Console.WriteLine(" Аргумент для команды \"/completetask\" отсутствует");
                }
            }
            else
            {
                Console.WriteLine("\n Список задач пуст\n");
            }
        }

        private void ShowAllTasks(ITelegramBotClient telegramBotClient, Update update)
        {
            List<ToDoItem> listAllTasks = _todoService.GetAllByUserId(_userService.GetUser(update.Message.From.Id).UserId).ToList();
            
            Console.Clear();

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

        private void RemoveTask()
        {
            Console.Clear();

            if (Tasks.Count() > 0)
            {
                string str;
                int num = 0;

                Console.WriteLine("\n Список доступных задач: ");
                for (int i = 0; i < Tasks.Count(); i++)
                {
                    Console.WriteLine("{0}. {1} - {2} - {3}", i + 1, Tasks[i].Name, Tasks[i].CreatedAt, Tasks[i].Id);
                }
                Console.WriteLine();

                Console.Write(" Введите номер задачи для удаления: ");
                do
                {
                    str = Console.ReadLine();
                    int.TryParse(str, out num);

                    if (num == 0)
                    {
                        Console.Write("\n Необходимо ввести именно номер задачи, попробуйте еще раз: ");
                    }

                    if (num > Tasks.Count())
                    {
                        Console.Write("\n Задачи с таким номером нет, попробуйте еще раз: ");
                    }
                }
                while (num < 1 || num > Tasks.Count());

                Console.WriteLine("\nЗадача \"{0}\" удалена.\n", Tasks[num - 1].Name);
                Tasks.RemoveAt(num - 1);
            }
            else
            {
                Console.WriteLine("\n Список задач пуст\n");
            }
        }

        private void ShowTasks(ITelegramBotClient telegramBotClient, Update update)
        {
            Console.Clear();

            if (Tasks.Count() > 0)
            {
                for (int i = 0; i < Tasks.Count(); i++)
                {
                    if (Tasks[i].State == ToDoItem.ToDoItemState.Active)
                        telegramBotClient.SendMessage(update.Message.Chat, $"{i + 1}. {Tasks[i].Name} - {Tasks[i].CreatedAt} - {Tasks[i].Id}");
                }                
            }
            else
            {
                telegramBotClient.SendMessage(update.Message.Chat, " Список задач пуст");
            }
        }

        private void AddTask(string inputStr, ITelegramBotClient telegramBotClient, Update update)
        {
            try
            {
                string newTask = inputStr.Substring(9);                

                Console.Clear();

                if (Tasks.Count() < maxTask)
                {
                    if (!string.IsNullOrWhiteSpace(newTask))
                    {
                        do
                        {                            
                            if (newTask.Count() > maxLengthTask)
                            {
                                throw new TaskLengthLimitException(newTask.Count(), maxLengthTask);
                            }

                            if (Tasks.Count() > 0)
                            {
                                foreach (var curr in Tasks)
                                {
                                    if (curr.Name == newTask)
                                    {
                                        throw new DuplicateTaskException(newTask);
                                    }
                                }
                            }
                        }
                        while (string.IsNullOrWhiteSpace(newTask));
                        Tasks.Add(new ToDoItem(someUser, newTask));
                        ToDoItem newToDoItem = _todoService.Add(someUser, newTask);

                        telegramBotClient.SendMessage(update.Message.Chat, $" Задача добавлена:\n Id: {newToDoItem.Id}\n User: {newToDoItem.User}\n Name: {newToDoItem.Name}\n CreatedAt: {newToDoItem.CreatedAt}\n State: {newToDoItem.State}\n StateCangedAt: {newToDoItem.StateCangedAt}");
                    }
                    else
                    {                        
                        telegramBotClient.SendMessage(update.Message.Chat, " Аргумент для команды \"/addtask\" отсутствует");
                    }
                }
                else
                {
                    throw new TaskCountLimitException(maxTask);
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
    }
}
