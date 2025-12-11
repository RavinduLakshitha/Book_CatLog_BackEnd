using Xunit;
using Moq;
using BookCatalog.Application.Features.Books.Commands;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.DTOs;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Tests.Features.Books.Commands
{
    public class UpdateBookCommandHandlerTest
    {
        private readonly Mock<IBookRepository> _mockRepository;
        private readonly UpdateBookAsyncCommandHandler _handler;

        public UpdateBookCommandHandlerTest()
        {
            _mockRepository = new Mock<IBookRepository>();
            _handler = new UpdateBookAsyncCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_UpdatesBook_WithValidData()
        {
            // Arrange
            int bookId = 1;
            var bookDto = new BookDto
            {
                Id = bookId,
                Name = "Updated Great Gatsby",
                Description = "Updated description",
                Author = "F. Scott Fitzgerald",
                Quantity = 10,
                Price = 14.99m
            };

            _mockRepository
                .Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateBookAsyncCommand(bookId, bookDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateBookAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task Handle_MapsBookDtoToBookEntity_Correctly()
        {
            // Arrange
            int bookId = 2;
            var bookDto = new BookDto
            {
                Id = bookId,
                Name = "1984 Updated",
                Description = "Updated dystopian novel",
                Author = "George Orwell",
                Quantity = 5,
                Price = 16.99m
            };

            Book? capturedBook = null;
            _mockRepository
                .Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Callback<Book>(book => capturedBook = book)
                .Returns(Task.CompletedTask);

            var command = new UpdateBookAsyncCommand(bookId, bookDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedBook);
            Assert.Equal(bookId, capturedBook.Id);
            Assert.Equal("1984 Updated", capturedBook.Name);
            Assert.Equal("Updated dystopian novel", capturedBook.Description);
            Assert.Equal("George Orwell", capturedBook.Author);
            Assert.Equal(5, capturedBook.Quantity);
            Assert.Equal(16.99m, capturedBook.Price);
        }

        [Fact]
        public async Task Handle_UsesCommandIdNotDtoId()
        {
            // Arrange
            int commandId = 10;
            var bookDto = new BookDto
            {
                Id = 999,  // Different from command ID
                Name = "Book",
                Description = "Description",
                Author = "Author",
                Quantity = 1,
                Price = 10.00m
            };

            Book? capturedBook = null;
            _mockRepository
                .Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Callback<Book>(book => capturedBook = book)
                .Returns(Task.CompletedTask);

            var command = new UpdateBookAsyncCommand(commandId, bookDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedBook);
            Assert.Equal(commandId, capturedBook.Id);  // Should use command ID, not DTO ID
            Assert.NotEqual(999, capturedBook.Id);
        }

        [Fact]
        public async Task Handle_CallsRepositoryOnce()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Id = 1,
                Name = "Test Book",
                Description = "Test",
                Author = "Test Author",
                Quantity = 1,
                Price = 9.99m
            };

            _mockRepository
                .Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateBookAsyncCommand(1, bookDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateBookAsync(It.IsAny<Book>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData(1, "Book 1", "Author 1", 5, 10.00)]
        [InlineData(2, "Book 2", "Author 2", 10, 20.50)]
        [InlineData(3, "Book 3", "Author 3", 15, 25.99)]
        public async Task Handle_CorrectlyUpdatesMultipleBooks(int id, string name, string author, int quantity, decimal price)
        {
            // Arrange
            var bookDto = new BookDto
            {
                Id = id,
                Name = name,
                Description = "Description",
                Author = author,
                Quantity = quantity,
                Price = (decimal)price
            };

            _mockRepository
                .Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateBookAsyncCommand(id, bookDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateBookAsync(It.Is<Book>(b =>
                b.Id == id &&
                b.Name == name &&
                b.Author == author &&
                b.Quantity == quantity &&
                b.Price == (decimal)price)), Times.Once);
        }

        [Fact]
        public async Task Handle_PreservesAllProperties_DuringUpdate()
        {
            // Arrange
            int bookId = 5;
            var bookDto = new BookDto
            {
                Id = bookId,
                Name = "The Catcher in the Rye",
                Description = "A story of adolescence and alienation",
                Author = "J.D. Salinger",
                Quantity = 8,
                Price = 14.50m
            };

            Book? capturedBook = null;
            _mockRepository
                .Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Callback<Book>(book => capturedBook = book)
                .Returns(Task.CompletedTask);

            var command = new UpdateBookAsyncCommand(bookId, bookDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedBook);
            Assert.Equal(bookDto.Name, capturedBook.Name);
            Assert.Equal(bookDto.Description, capturedBook.Description);
            Assert.Equal(bookDto.Author, capturedBook.Author);
            Assert.Equal(bookDto.Quantity, capturedBook.Quantity);
            Assert.Equal(bookDto.Price, capturedBook.Price);
        }

        [Fact]
        public async Task Handle_CompletesSuccessfully_WithoutException()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Id = 1,
                Name = "Book",
                Description = "Description",
                Author = "Author",
                Quantity = 1,
                Price = 10.00m
            };

            _mockRepository
                .Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateBookAsyncCommand(1, bookDto);

            // Act & Assert (no exception thrown)
            await _handler.Handle(command, CancellationToken.None);
        }

        [Fact]
        public async Task Handle_DoesNotThrowException_WhenRepositorySucceeds()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Id = 1,
                Name = "Book",
                Description = "Description",
                Author = "Author",
                Quantity = 1,
                Price = 10.00m
            };

            _mockRepository
                .Setup(repo => repo.UpdateBookAsync(It.IsAny<Book>()))
                .Returns(Task.CompletedTask);

            var command = new UpdateBookAsyncCommand(1, bookDto);

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            Assert.Null(exception);
        }
    }
}
