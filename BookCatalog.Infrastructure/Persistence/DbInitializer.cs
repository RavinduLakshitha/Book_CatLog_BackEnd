using BookCatalog.Infrastructure.Persistence;

namespace BookCatalog.Infrastructure.Persistence
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DbInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            var tableCommand = @"CREATE TABLE IF NOT EXISTS Books (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Name TEXT NOT NULL,
                                    Description TEXT,
                                    Author TEXT,
                                    Quantity INTEGER,
                                    Price Decimal(18,2)
                                );";

            using var command = connection.CreateCommand();
            command.CommandText = tableCommand;
            command.ExecuteNonQuery();
        }
    }
}