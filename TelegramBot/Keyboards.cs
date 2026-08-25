using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using CookingBot.Core.Entities;

namespace CookingBot.TelegramBot
{
    public static class Keyboards
    {
        public static InlineKeyboardMarkup BuildKeyboardForUser(ToDoUser? user)
        {
            var rows = new List<List<InlineKeyboardButton>>();

            if (user == null)
            {
                rows.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData("Старт", "/start"),
                    InlineKeyboardButton.WithCallbackData("Помощь", "/help")
                });
                rows.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData("Все задачи (общие)", "/alltasks"),
                    InlineKeyboardButton.WithCallbackData("Поиск (общие)", "/findall")
                });
                rows.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData("Информация", "/info")
                });

                return new InlineKeyboardMarkup(rows);
            }

            rows.Add(new()
            {
                InlineKeyboardButton.WithCallbackData("Добавить задачу", "/addtask"),
                InlineKeyboardButton.WithCallbackData("Активные задачи", "/showtasks")
            });
            rows.Add(new()
            {
                InlineKeyboardButton.WithCallbackData("Все задачи", "/showalltasks"),
                InlineKeyboardButton.WithCallbackData("Инфо о задаче", "/infotask")
            });
            rows.Add(new()
            {
                InlineKeyboardButton.WithCallbackData("Все задачи (общие)", "/alltasks"),
                InlineKeyboardButton.WithCallbackData("Поиск (общие)", "/findall")
            });
            rows.Add(new()
            {
                InlineKeyboardButton.WithCallbackData("Удалить задачу", "/removetask"),
                InlineKeyboardButton.WithCallbackData("Завершить задачу", "/completetask")
            });
            rows.Add(new()
            {
                InlineKeyboardButton.WithCallbackData("Поиск", "/find"),
                InlineKeyboardButton.WithCallbackData("Отчёт", "/report")
            });
            rows.Add(new()
            {
                InlineKeyboardButton.WithCallbackData("Мой профиль", "/myinfo"),
                InlineKeyboardButton.WithCallbackData("Помощь", "/help")
            });
            rows.Add(new()
            {
                InlineKeyboardButton.WithCallbackData("Информация", "/info"),
                InlineKeyboardButton.WithCallbackData("Выход", "/exit")
            });

            if (user.State == ToDoUser.ToDoUserState.Moderator || user.State == ToDoUser.ToDoUserState.Admin)
            {
                rows.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData("Список пользователей", "mod_listusers")
                });
                rows.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData("Повысить до Member", "mod_promote_member"),
                    InlineKeyboardButton.WithCallbackData("Понизить до Guest", "mod_demote_guest")
                });
            }

            if (user.State == ToDoUser.ToDoUserState.Admin)
            {
                rows.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData("Повысить до Moderator", "admin_promote_mod"),
                    InlineKeyboardButton.WithCallbackData("Повысить до Admin", "admin_promote_admin")
                });
                rows.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData("Понизить до Advanced", "admin_demote_advanced"),
                    InlineKeyboardButton.WithCallbackData("Понизить до Moderator", "admin_demote_mod")
                });
            }

            return new InlineKeyboardMarkup(rows);
        }

        public static InlineKeyboardMarkup BuildProfileKeyboard(Guid userId)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Смена имени", $"changename_{userId}") },
                new[] { InlineKeyboardButton.WithCallbackData("Удалить аккаунт", $"deleteaccount_{userId}") },
                new[] { InlineKeyboardButton.WithCallbackData("Назад", "mainmenu") }
            });
        }

        public static ReplyKeyboardMarkup BuildCancelKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { new KeyboardButton("Отмена") }
            })
            { ResizeKeyboard = true };
        }
    }
}
