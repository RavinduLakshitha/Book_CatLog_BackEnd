using Dapper;
using BookCatalog.Application.Interfaces;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Infrastructure.Persistence
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BookRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Book>("SELECT * FROM Books");
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Book>(
                "SELECT * FROM Books WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> AddBookAsync(Book book)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "INSERT INTO Books (Name, Description, Author, Quantity, Price) VALUES (@Name, @Description, @Author, @Quantity, @Price);" +
                      "SELECT last_insert_rowid();";

            var id = await connection.ExecuteScalarAsync<int>(sql, book);
            return id;
        }

        public async Task UpdateBookAsync(Book book)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Books SET Name = @Name, Description = @Description, Author = @Author, Quantity = @Quantity, Price = @Price WHERE Id = @Id";
            await connection.ExecuteAsync(sql, book);
        }

        public async Task DeleteBookAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Books WHERE Id = @Id", new { Id = id });
        }
    }
}