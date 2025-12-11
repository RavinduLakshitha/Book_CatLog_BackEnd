using MediatR;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;

namespace BookCatalog.Application.Features.Books.Commands
{
    public class DeleteBookCommand : IRequest
    {
        public int Id { get; set; }

        public DeleteBookCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
    {
        private readonly IBookRepository _bookRepository;

        public DeleteBookCommandHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            await _bookRepository.DeleteBookAsync(request.Id);
        }
    }
}