using System.Globalization;
using System.Text;
using BankingAppCSharpSimple.Models;

namespace BankingAppCSharpSimple
{
    class Program
    {
        private static DatabaseManager _dbManager = new DatabaseManager();
        private static User _currentUser = null;

        static string GetStringInput(string prompt, int minLength = 1, int maxLength = 255)
        {
            string value;
            while (true)
            {
                Console.Write(prompt);
                value = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                    Console.WriteLine("Ввод не может быть пустым. Попробуйте снова.");
                    continue;
                }
                if (value.Length < minLength || value.Length > maxLength)
                {
                    Console.WriteLine($"Длина ввода от {minLength} до {maxLength} символов. Попробуйте снова.");
                }
                else
                {
                    return value;
                }
            }
        }

        static string GetPasswordInput(string prompt = "Пароль: ")
        {
            Console.Write(prompt);
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    password.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password.ToString();
        }

        static decimal GetDecimalInput(string prompt, decimal minValue = 0.01m)
        {
            decimal value;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value) && value >= minValue)
                {
                    return value;
                }
                Console.WriteLine($"Некорректный ввод или значение меньше {minValue.ToString(CultureInfo.InvariantCulture)}. Используйте формат '123.45'.");
            }
        }

        static void HandleRegistration()
        {
            Console.WriteLine("\n--- Регистрация ---");
            string login = GetStringInput("Логин: ", 3, 50);
            string password = GetPasswordInput("Пароль (мин 6 символов): ");
            if (password.Length < 6) { Console.WriteLine("Пароль слишком короткий."); return; }
            string fullName = GetStringInput("ФИО: ", 3, 100);

            if (_dbManager.CreateUser(login, password, fullName))
            {
                Console.WriteLine($"Пользователь '{login}' успешно зарегистрирован!");
            }
        }

        static void HandleLogin()
        {
            Console.WriteLine("\n--- Вход ---");
            string login = GetStringInput("Логин: ");
            string password = GetPasswordInput("Пароль: ");

            User user = _dbManager.GetUserByLogin(login);
            if (user != null && user.PasswordHash == DatabaseManager.HashPassword(password))
            {
                if (user.IsBlocked)
                {
                    Console.WriteLine("Ваш аккаунт заблокирован.");
                }
                else
                {
                    _currentUser = user;
                    Console.WriteLine($"Добро пожаловать, {user.FullName}!");
                }
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль.");
            }
        }

        static void HandleLogout()
        {
            Console.WriteLine($"Пользователь {_currentUser.Login} вышел из системы.");
            _currentUser = null;
        }

        static void HandleViewMyAccounts()
        {
            if (_currentUser == null) return;
            Console.WriteLine("\n--- Ваши счета ---");
            var accounts = _dbManager.GetAccountsByUserId(_currentUser.UserId);
            if (accounts == null || !accounts.Any())
            {
                Console.WriteLine("У вас нет счетов.");
                return;
            }
            foreach (var acc in accounts)
            {
                Console.WriteLine($"ID: {acc.AccountId}, Номер: {acc.AccountNumber}, Баланс: {acc.Balance.ToString("N2", CultureInfo.InvariantCulture)} {acc.Currency}");
            }
        }

        static void HandleDepositFunds()
        {
            if (_currentUser == null) return;
            HandleViewMyAccounts();
            Console.Write("Введите ID счета для пополнения: ");
            if (!int.TryParse(Console.ReadLine(), out int accountId))
            {
                Console.WriteLine("Неверный ID счета.");
                return;
            }

            decimal amount = GetDecimalInput("Сумма пополнения: ");
            var accountToUpdate = _dbManager.GetAccountsByUserId(_currentUser.UserId).FirstOrDefault(a => a.AccountId == accountId);
            if (accountToUpdate == null)
            {
                Console.WriteLine("Счет не найден или не принадлежит вам.");
                return;
            }

            if (_dbManager.UpdateAccountBalance(accountId, accountToUpdate.Balance + amount))
            {
                _dbManager.CreateTransaction(accountId, amount, "deposit", "Консольное пополнение");
                Console.WriteLine("Счет успешно пополнен.");
            }
            else
            {
                Console.WriteLine("Ошибка пополнения счета.");
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Добро пожаловать!");

            try
            {
                var testUser = _dbManager.GetUserByLogin("admin");
                if (testUser != null) Console.WriteLine("Подключение к БД вроде бы работает (admin найден).");
                else Console.WriteLine("Подключение к БД работает, но admin не найден (проверьте данные).");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Критическая ошибка подключения к БД: {ex.Message}");
                Console.WriteLine("Проверьте строку подключения в DatabaseManager.cs и доступность сервера PostgreSQL.");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            bool running = true;
            while (running)
            {
                Console.WriteLine("\n----------------------------------");
                if (_currentUser == null)
                {
                    Console.WriteLine("Главное меню:");
                    Console.WriteLine("1. Регистрация");
                    Console.WriteLine("2. Вход");
                    Console.WriteLine("3. Выход");
                    Console.Write("Ваш выбор: ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1": HandleRegistration(); break;
                        case "2": HandleLogin(); break;
                        case "3": running = false; break;
                        default: Console.WriteLine("Неверный выбор."); break;
                    }
                }
                else
                {
                    Console.WriteLine($"Меню пользователя ({_currentUser.Login}, роль: {_currentUser.Role}):");
                    Console.WriteLine("1. Просмотр моих счетов");
                    Console.WriteLine("2. Внести средства (Депозит)");
                    if (_currentUser.Role == "admin")
                    {
                        Console.WriteLine("A1. Просмотр всех пользователей (Админ)");
                    }
                    Console.WriteLine("9. Выход из системы");
                    Console.Write("Ваш выбор: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": HandleViewMyAccounts(); break;
                        case "2": HandleDepositFunds(); break;
                        case "A1":
                            if (_currentUser.Role == "admin") { Console.WriteLine("Функция просмотра всех пользователей (TODO)"); }
                            else { Console.WriteLine("Доступ запрещен."); }
                            break;
                        case "9": HandleLogout(); break;
                        default: Console.WriteLine("Неверный выбор."); break;
                    }
                }
                if (running)
                {
                    Console.WriteLine("Нажмите Enter для продолжения...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            Console.WriteLine("Приложение завершено.");
        }
    }
}
