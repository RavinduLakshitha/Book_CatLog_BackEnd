using Xunit;
using Moq;
using BookCatalog.Application.Features.Books.Queries;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.DTOs;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Tests.Features.Books.Queries
{
    public class GetAllBooksQueryHandlerTests
    {
        private readonly Mock<IBookRepository> _mockRepository;
        private readonly GetAllBookQueryHandler _handler;

        public GetAllBooksQueryHandlerTests()
        {
            _mockRepository = new Mock<IBookRepository>();
            _handler = new GetAllBookQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_ReturnsAllBooks_WhenBooksExist()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Name = "The Great Gatsby",
                    Description = "A classic novel",
                    Author = "F. Scott Fitzgerald",
                    Quantity = 5,
                    Price = 12.99m
                },
                new Book
                {
                    Id = 2,
                    Name = "1984",
                    Description = "A dystopian novel",
                    Author = "George Orwell",
                    Quantity = 3,
                    Price = 15.99m
                }
            };

            _mockRepository
                .Setup(repo => repo.GetAllBooksAsync())
                .ReturnsAsync(books);

            var query = new GetAllBookQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            
            var resultList = result.ToList();
            Assert.Equal("The Great Gatsby", resultList[0].Name);
            Assert.Equal("1984", resultList[1].Name);
            Assert.Equal("F. Scott Fitzgerald", resultList[0].Author);
            Assert.Equal("George Orwell", resultList[1].Author);

            _mockRepository.Verify(repo => repo.GetAllBooksAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenNoBooksExist()
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.GetAllBooksAsync())
                .ReturnsAsync(new List<Book>());

            var query = new GetAllBookQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _mockRepository.Verify(repo => repo.GetAllBooksAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsMappedDtos_WithCorrectProperties()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Name = "The Great Gatsby",
                    Description = "A classic novel",
                    Author = "F. Scott Fitzgerald",
                    Quantity = 5,
                    Price = 12.99m
                }
            };

            _mockRepository
                .Setup(repo => repo.GetAllBooksAsync())
                .ReturnsAsync(books);

            var query = new GetAllBookQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var resultList = result.ToList();
            var firstBook = resultList[0];

            Assert.IsType<BookDto>(firstBook);
            Assert.Equal(1, firstBook.Id);
            Assert.Equal("The Great Gatsby", firstBook.Name);
            Assert.Equal("A classic novel", firstBook.Description);
            Assert.Equal("F. Scott Fitzgerald", firstBook.Author);
            Assert.Equal(5, firstBook.Quantity);
            Assert.Equal(12.99m, firstBook.Price);
        }

        [Fact]
        public async Task Handle_CallsRepositoryOnce()
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.GetAllBooksAsync())
                .ReturnsAsync(new List<Book>());

            var query = new GetAllBookQuery();

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.GetAllBooksAsync(), Times.Exactly(1));
        }
    }
}
