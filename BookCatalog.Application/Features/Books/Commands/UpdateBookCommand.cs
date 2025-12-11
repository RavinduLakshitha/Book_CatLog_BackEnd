using MediatR;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;

namespace BookCatalog.Application.Features.Books.Commands
{
    public class UpdateBookAsyncCommand : IRequest
    {
        public int Id { get; set; }
        public BookDto Book { get; set; }

        public UpdateBookAsyncCommand(int id, BookDto book)
        {
            Id = id;
            Book = book;
        }
    }
    public class UpdateBookAsyncCommandHandler : IRequestHandler<UpdateBookAsyncCommand>
    {
        private readonly IBookRepository _bookRepository;

        public UpdateBookAsyncCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task Handle(UpdateBookAsyncCommand request, CancellationToken cancellationToken)
        {
            var book = new BookCatalog.Domain.Entities.Book
            {
                Id = request.Id,
                Name = request.Book.Name,
                Description = request.Book.Description,
                Author = request.Book.Author,
                Quantity = request.Book.Quantity,
                Price = request.Book.Price
            };
            await _bookRepository.UpdateBookAsync(book);
        }
    }

}