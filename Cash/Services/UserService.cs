using Cash.DB;

namespace Services
{
    public class UserService
    {
        private readonly DatabaseManager _db;

        public UserService(DatabaseManager db)
        {
            _db = db;
        }

        public bool Register(string login, string password, string fullName)
        {
            if (_db.GetUserByLogin(login) != null) return false;
            string passwordHash = Utils.Hash(password);
            var user = new User { Login = login, PasswordHash = passwordHash, FullName = fullName, Role = "client" };
            _db.CreateUser(user);
            return true;
        }

        public User Login(string login, string password)
        {
            var user = _db.GetUserByLogin(login);
            if (user == null || user.PasswordHash != Utils.Hash(password))
            {
                _db.RecordLoginAttempt(login, false);
                return null;
            }
            _db.RecordLoginAttempt(login, true);
            return user;
        }

        public void BlockUser(int userId) => _db.SetUserBlockStatus(userId, true);
        public void UnblockUser(int userId) => _db.SetUserBlockStatus(userId, false);
        public List<User> GetAllUsers() => _db.GetAllUsers();
    }

    public static class Utils
    {
        public static string Hash(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}