using MediatR;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Features.Books.Commands;
using BookCatalog.Application.Features.Books.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _mediator.Send(new GetAllBookQuery());
            return Ok(books);   
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid book ID.");

            var book = await _mediator.Send(new GetBookIdQuery(id));
            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] CreateBookDto bookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _mediator.Send(new AddBookCommand(bookDto));
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDto bookDto)
        {
            if (id <= 0)
                return BadRequest("Invalid book ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _mediator.Send(new UpdateBookAsyncCommand(id, bookDto));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid book ID.");

            await _mediator.Send(new DeleteBookCommand(id));
            return NoContent();
        }
    }
}