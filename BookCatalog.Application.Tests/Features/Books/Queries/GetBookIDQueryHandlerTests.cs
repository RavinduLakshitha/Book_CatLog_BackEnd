using Xunit;
using Moq;
using BookCatalog.Application.Features.Books.Queries;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.DTOs;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Tests.Features.Books.Queries
{
    public class GetBookIdQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _mockRepository;
        private readonly GetBookIdQueryHandler _handler;

        public GetBookIdQueryHandlerTests()
        {
            _mockRepository = new Mock<IBookRepository>();
            _handler = new GetBookIdQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ReturnsBook_WhenBookExists()
        {
            // Arrange
            int bookId = 1;
            var book = new Book
            {
                Id = bookId,
                Name = "The Great Gatsby",
                Description = "A classic novel",
                Author = "F. Scott Fitzgerald",
                Quantity = 5,
                Price = 12.99m
            };

            _mockRepository
                .Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync(book);

            var query = new GetBookIdQuery(bookId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
            Assert.Equal("The Great Gatsby", result.Name);
            Assert.Equal("A classic novel", result.Description);
            Assert.Equal("F. Scott Fitzgerald", result.Author);
            Assert.Equal(5, result.Quantity);
            Assert.Equal(12.99m, result.Price);

            _mockRepository.Verify(repo => repo.GetBookByIdAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenBookNotFound()
        {
            // Arrange
            int bookId = 999;
            _mockRepository
                .Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            var query = new GetBookIdQuery(bookId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);

            _mockRepository.Verify(repo => repo.GetBookByIdAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsMappedDto_WithCorrectProperties()
        {
            // Arrange
            int bookId = 5;
            var book = new Book
            {
                Id = bookId,
                Name = "1984",
                Description = "A dystopian novel",
                Author = "George Orwell",
                Quantity = 3,
                Price = 15.99m
            };

            _mockRepository
                .Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync(book);

            var query = new GetBookIdQuery(bookId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsType<BookDto>(result);
            Assert.Equal(5, result.Id);
            Assert.Equal("1984", result.Name);
            Assert.Equal("A dystopian novel", result.Description);
            Assert.Equal("George Orwell", result.Author);
            Assert.Equal(3, result.Quantity);
            Assert.Equal(15.99m, result.Price);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async Task Handle_CallsRepositoryWithCorrectId(int bookId)
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.GetBookByIdAsync(bookId))
                .ReturnsAsync((Book?)null);

            var query = new GetBookIdQuery(bookId);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.GetBookByIdAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsCorrectBook_WithMultipleCalls()
        {
            // Arrange
            var book1 = new Book
            {
                Id = 1,
                Name = "Book 1",
                Description = "Description 1",
                Author = "Author 1",
                Quantity = 1,
                Price = 10.00m
            };

            var book2 = new Book
            {
                Id = 2,
                Name = "Book 2",
                Description = "Description 2",
                Author = "Author 2",
                Quantity = 2,
                Price = 20.00m
            };

            _mockRepository
                .Setup(repo => repo.GetBookByIdAsync(1))
                .ReturnsAsync(book1);

            _mockRepository
                .Setup(repo => repo.GetBookByIdAsync(2))
                .ReturnsAsync(book2);

            var query1 = new GetBookIdQuery(1);
            var query2 = new GetBookIdQuery(2);

            // Act
            var result1 = await _handler.Handle(query1, CancellationToken.None);
            var result2 = await _handler.Handle(query2, CancellationToken.None);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal("Book 1", result1.Name);
            Assert.Equal("Book 2", result2.Name);
            Assert.Equal(1, result1.Id);
            Assert.Equal(2, result2.Id);

            _mockRepository.Verify(repo => repo.GetBookByIdAsync(1), Times.Once);
            _mockRepository.Verify(repo => repo.GetBookByIdAsync(2), Times.Once);
        }
    }
}
