using System.Threading.Tasks;
using Library.Attributes;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [Route("book")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookModel _model;

        public BookController(IBookModel model)
        {
            _model = model;
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(BookProxy book)
        {
            var response = await _model.CreateBookAsync(book);

            return StatusCode(HttpStatusCode.CREATED, response);
        }

        [HttpPut]
        [IdRoute]
        public async Task<ActionResult> UpdateAsync(int id, BookProxy book)
        {
            await _model.UpdateBookAsync(id, book);

            return Ok();
        }

        [HttpGet]
        [IdRoute]
        public async Task<BookProxy> GetByIdAsync(int id)
        {
            return await _model.GetBookByIdAsync(id);
        }

        [HttpDelete]
        [IdRoute]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await _model.DeleteBookAsync(id);

            return Ok();
        }
    }
}
