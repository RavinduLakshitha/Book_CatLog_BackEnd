using Xunit;
using Moq;
using BookCatalog.Application.Features.Books.Commands;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.DTOs;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Tests.Features.Books.Commands
{
    public class AddBookCommandHandlerTest
    {
        private readonly Mock<IBookRepository> _mockRepository;
        private readonly AddBookCommandHandler _handler;

        public AddBookCommandHandlerTest()
        {
            _mockRepository = new Mock<IBookRepository>();
            _handler = new AddBookCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_AddsBook_AndReturnsGeneratedId()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Name = "The Great Gatsby",
                Description = "A classic novel",
                Author = "F. Scott Fitzgerald",
                Quantity = 5,
                Price = 12.99m
            };

            int generatedId = 1;

            _mockRepository
                .Setup(repo => repo.AddBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(generatedId);

            var command = new AddBookCommand(createBookDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(generatedId, result);
            _mockRepository.Verify(repo => repo.AddBookAsync(It.IsAny<Book>()), Times.Once);
        }

        [Fact]
        public async Task Handle_MapsCreateBookDtoToBookEntity_Correctly()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Name = "1984",
                Description = "A dystopian novel",
                Author = "George Orwell",
                Quantity = 3,
                Price = 15.99m
            };

            int generatedId = 2;

            Book? capturedBook = null;
            _mockRepository
                .Setup(repo => repo.AddBookAsync(It.IsAny<Book>()))
                .Callback<Book>(book => capturedBook = book)
                .ReturnsAsync(generatedId);

            var command = new AddBookCommand(createBookDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedBook);
            Assert.Equal("1984", capturedBook.Name);
            Assert.Equal("A dystopian novel", capturedBook.Description);
            Assert.Equal("George Orwell", capturedBook.Author);
            Assert.Equal(3, capturedBook.Quantity);
            Assert.Equal(15.99m, capturedBook.Price);
            Assert.Equal(generatedId, result);
        }

        [Fact]
        public async Task Handle_CallsRepositoryOnce()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Name = "Test Book",
                Description = "Test Description",
                Author = "Test Author",
                Quantity = 1,
                Price = 9.99m
            };

            _mockRepository
                .Setup(repo => repo.AddBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(1);

            var command = new AddBookCommand(createBookDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.AddBookAsync(It.IsAny<Book>()), Times.Exactly(1));
        }

        [Theory]
        [InlineData("Book 1", "Author 1", 1, 10.00)]
        [InlineData("Book 2", "Author 2", 5, 20.50)]
        [InlineData("Book 3", "Author 3", 10, 25.99)]
        public async Task Handle_CorrectlyHandlesMultipleBooks(string name, string author, int quantity, decimal price)
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Name = name,
                Description = "Description",
                Author = author,
                Quantity = quantity,
                Price = (decimal)price
            };

            int generatedId = 1;

            _mockRepository
                .Setup(repo => repo.AddBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(generatedId);

            var command = new AddBookCommand(createBookDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(generatedId, result);
            _mockRepository.Verify(repo => repo.AddBookAsync(It.Is<Book>(b => 
                b.Name == name && 
                b.Author == author && 
                b.Quantity == quantity && 
                b.Price == (decimal)price)), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsAutoIncrementedId()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Name = "Book",
                Description = "Description",
                Author = "Author",
                Quantity = 1,
                Price = 10.00m
            };

            _mockRepository
                .Setup(repo => repo.AddBookAsync(It.IsAny<Book>()))
                .ReturnsAsync(5);

            var command = new AddBookCommand(createBookDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task Handle_PreservesAllBookProperties_DuringMapping()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Name = "The Catcher in the Rye",
                Description = "A story of adolescence",
                Author = "J.D. Salinger",
                Quantity = 7,
                Price = 14.50m
            };

            Book? capturedBook = null;
            _mockRepository
                .Setup(repo => repo.AddBookAsync(It.IsAny<Book>()))
                .Callback<Book>(book => capturedBook = book)
                .ReturnsAsync(3);

            var command = new AddBookCommand(createBookDto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedBook);
            Assert.Equal(createBookDto.Name, capturedBook.Name);
            Assert.Equal(createBookDto.Description, capturedBook.Description);
            Assert.Equal(createBookDto.Author, capturedBook.Author);
            Assert.Equal(createBookDto.Quantity, capturedBook.Quantity);
            Assert.Equal(createBookDto.Price, capturedBook.Price);
        }
    }
}
