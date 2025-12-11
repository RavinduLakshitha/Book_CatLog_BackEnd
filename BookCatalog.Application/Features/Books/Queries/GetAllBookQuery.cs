using MediatR;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.DTOs;


namespace BookCatalog.Application.Features.Books.Queries
{
    public class GetAllBookQuery : IRequest<IEnumerable<BookDto>>
    {
    }

    public class GetAllBookQueryHandler : IRequestHandler<GetAllBookQuery, IEnumerable<BookDto>>
    {
        private readonly IBookRepository _bookRepository;

        public GetAllBookQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BookDto>> Handle(GetAllBookQuery request, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetAllBooksAsync();
            return books.Select(b => new BookDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                Author = b.Author,
                Quantity = b.Quantity,
                Price = b.Price
            });
        }
    }
}

