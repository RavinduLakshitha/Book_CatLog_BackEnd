using MediatR;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;

namespace BookCatalog.Application.Features.Books.Queries
{
    public class GetBookIdQuery : IRequest<BookDto?>
    {
        public int Id { get; set; }

        public GetBookIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetBookIdQueryHandler : IRequestHandler<GetBookIdQuery, BookDto?>
    {
        private readonly IBookRepository _bookRepository;

        public GetBookIdQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookDto?> Handle(GetBookIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookByIdAsync(request.Id);
            return book != null ? new BookDto
            {
                Id = book.Id,
                Name = book.Name,
                Description = book.Description,
                Author = book.Author,
                Quantity = book.Quantity,
                Price = book.Price
            } : null;
        }
    }
}