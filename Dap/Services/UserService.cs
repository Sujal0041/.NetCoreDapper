    using Dapper;
    using Dap.Models;
    using Npgsql;
    using System.Data;
    using System.Threading.Tasks;

    namespace Dap.Services
    {
    // Interface defining the contract for user-related operations
    public interface IUserService
    {
        Task<bool> RegisterUser(AppUser newUser); // Registers a new user
        Task<AppUser?> GetUser(string email, string password); // Retrieves a user by email and password
    }


    // Implementation of IUserService
    public class UserService : IUserService
    {
        private readonly string _connectionString; // Connection string for the database

        // Constructor to initialize the connection string from configuration
        public UserService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // Registers a new user in the database
        public async Task<bool> RegisterUser(AppUser newUser)
        {
            using IDbConnection db = new NpgsqlConnection(_connectionString); // Create a new database connection

            // Check if a user with the same username or email already exists
            var existingUser = await db.QueryFirstOrDefaultAsync<AppUser>(
                "SELECT * FROM users WHERE username = @Username OR email = @Email",
                new { newUser.Username, newUser.Email });

            if (existingUser != null) return false; // Return false if user already exists

            // SQL query to insert a new user into the database
            var sql = "INSERT INTO users (username, email, passwordhash, role) VALUES (@Username, @Email, @PasswordHash, @Role)";
            var result = await db.ExecuteAsync(sql, newUser); // Execute the query

            return result > 0; // Return true if the insertion was successful
        }

        // Retrieves a user by email and password
        public async Task<AppUser?> GetUser(string email, string password)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM users WHERE email = @Email AND passwordhash = @Password";
            return await connection.QuerySingleOrDefaultAsync<AppUser>(query, new { Email = email, Password = password });
        }
    }
    }
