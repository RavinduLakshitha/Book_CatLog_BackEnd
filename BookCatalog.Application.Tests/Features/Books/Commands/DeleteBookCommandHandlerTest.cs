using Xunit;
using Moq;
using BookCatalog.Application.Features.Books.Commands;
using BookCatalog.Application.Interfaces;

namespace BookCatalog.Application.Tests.Features.Books.Commands
{
    public class DeleteBookCommandHandlerTest
    {
        private readonly Mock<IBookRepository> _mockRepository;
        private readonly DeleteBookCommandHandler _handler;

        public DeleteBookCommandHandlerTest()
        {
            _mockRepository = new Mock<IBookRepository>();
            _handler = new DeleteBookCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_DeletesBook_WhenValidIdProvided()
        {
            // Arrange
            int bookId = 1;
            _mockRepository
                .Setup(repo => repo.DeleteBookAsync(bookId))
                .Returns(Task.CompletedTask);

            var command = new DeleteBookCommand(bookId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteBookAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task Handle_CallsRepositoryWithCorrectId()
        {
            // Arrange
            int bookId = 5;
            _mockRepository
                .Setup(repo => repo.DeleteBookAsync(bookId))
                .Returns(Task.CompletedTask);

            var command = new DeleteBookCommand(bookId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteBookAsync(bookId), Times.Exactly(1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task Handle_CorrectlyDeletesBookWithVariousIds(int bookId)
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.DeleteBookAsync(bookId))
                .Returns(Task.CompletedTask);

            var command = new DeleteBookCommand(bookId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteBookAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task Handle_CompletesSuccessfully_WithoutException()
        {
            // Arrange
            int bookId = 1;
            _mockRepository
                .Setup(repo => repo.DeleteBookAsync(bookId))
                .Returns(Task.CompletedTask);

            var command = new DeleteBookCommand(bookId);

            // Act & Assert (no exception thrown)
            await _handler.Handle(command, CancellationToken.None);
        }

        [Fact]
        public async Task Handle_DoesNotThrowException_WhenRepositorySucceeds()
        {
            // Arrange
            int bookId = 7;
            _mockRepository
                .Setup(repo => repo.DeleteBookAsync(bookId))
                .Returns(Task.CompletedTask);

            var command = new DeleteBookCommand(bookId);

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(command, CancellationToken.None));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Handle_DeletesMultipleBooks_InSequence()
        {
            // Arrange
            _mockRepository
                .Setup(repo => repo.DeleteBookAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var command1 = new DeleteBookCommand(1);
            var command2 = new DeleteBookCommand(2);
            var command3 = new DeleteBookCommand(3);

            // Act
            await _handler.Handle(command1, CancellationToken.None);
            await _handler.Handle(command2, CancellationToken.None);
            await _handler.Handle(command3, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteBookAsync(1), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteBookAsync(2), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteBookAsync(3), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteBookAsync(It.IsAny<int>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Handle_RepositoryCalledOnce_PerCommand()
        {
            // Arrange
            int bookId = 50;
            _mockRepository
                .Setup(repo => repo.DeleteBookAsync(bookId))
                .Returns(Task.CompletedTask);

            var command = new DeleteBookCommand(bookId);

            // Act
            await _handler.Handle(command, CancellationToken.None);
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockRepository.Verify(repo => repo.DeleteBookAsync(bookId), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_PassesCorrectIdToRepository()
        {
            // Arrange
            int expectedId = 42;
            int capturedId = 0;

            _mockRepository
                .Setup(repo => repo.DeleteBookAsync(It.IsAny<int>()))
                .Callback<int>(id => capturedId = id)
                .Returns(Task.CompletedTask);

            var command = new DeleteBookCommand(expectedId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expectedId, capturedId);
        }
    }
}
