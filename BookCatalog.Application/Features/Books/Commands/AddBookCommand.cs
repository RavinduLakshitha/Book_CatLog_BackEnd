using MediatR;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;

namespace BookCatalog.Application.Features.Books.Commands
{
    public class AddBookCommand : IRequest<int>
    {
        public CreateBookDto Book { get; set; }

        public AddBookCommand(CreateBookDto book)
        {
            Book = book;
        }
    }

    public class AddBookCommandHandler : IRequestHandler<AddBookCommand, int>
    {
        private readonly IBookRepository _bookRepository;

        public AddBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<int> Handle(AddBookCommand request, CancellationToken cancellationToken)
        {
            var book = new Domain.Entities.Book
            {
                Name = request.Book.Name,
                Description = request.Book.Description,
                Author = request.Book.Author,
                Quantity = request.Book.Quantity,
                Price = request.Book.Price
            };
            return await _bookRepository.AddBookAsync(book);
        }
    }
}
