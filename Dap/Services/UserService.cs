    using Dapper;
    using Dap.Models;
    using Npgsql;
    using System.Data;
    using System.Threading.Tasks;

    namespace Dap.Services
    {
        public interface IUserService
        {
            Task<bool> RegisterUser(AppUser newUser);
            Task<AppUser?> GetUser(string email, string password);
        }


        public class UserService : IUserService
        {
            private readonly string _connectionString;

            public UserService(IConfiguration config)
            {
                _connectionString = config.GetConnectionString("DefaultConnection");
            }

            public async Task<bool> RegisterUser(AppUser newUser)
            {
                using IDbConnection db = new NpgsqlConnection(_connectionString);

                var existingUser = await db.QueryFirstOrDefaultAsync<AppUser>(
                    "SELECT * FROM users WHERE username = @Username OR email = @Email",
                    new { newUser.Username, newUser.Email });

                if (existingUser != null) return false;

                var sql = "INSERT INTO users (username, email, passwordhash, role) VALUES (@Username, @Email, @PasswordHash, @Role)";
                var result = await db.ExecuteAsync(sql, newUser);

                return result > 0;
            }

            public async Task<AppUser?> GetUser(string email, string password)
            {
                using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM users WHERE email = @Email AND passwordhash = @Password";

            var user = await connection.QuerySingleOrDefaultAsync<AppUser>(query, new { Email = email, Password = password });
                return user;

            }

        }
    }
