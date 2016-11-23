using BookService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestTestWebService.Controllers
{
    public class BooksController : ApiController
    {
        private static List<Book> _books = null;

        // GET: api/Books
        public IQueryable<BookDTO> GetBooks()
        {
            init();
            var books = from b in _books
                        select new BookDTO()
                        {
                            Id = b.Id,
                            Title = b.Title,
                            AuthorName = b.Author.Name
                        };
            return books.AsQueryable();
        }

        // GET: api/Books/5
        [ResponseType(typeof(BookDetailDTO))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            init();
            if (_books.Any(b => b.Id == id))
            {
                var book = _books.FirstOrDefault(b => b.Id == id);
                var detail = new BookDetailDTO
                {
                    Id = book.Id,
                    Title = book.Title,
                    Year = book.Year,
                    Price = book.Price,
                    AuthorName = book.Author.Name,
                    Genre = book.Genre
                };
                return Ok(book);
            }
            return NotFound();
        }

        // POST: api/Books
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> PostBook(Book book)
        {
            init();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _books.Add(book);

            var dto = new BookDTO()
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Author.Name
            };

            return CreatedAtRoute("DefaultApi", new { id = book.Id }, dto);
        }
        // PUT: api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> Put(int id, Book book)
        {
            init();
            if (_books.Any(b => b.Id == id))
            {
                var oldbook = _books.FirstOrDefault(b => b.Id == id);
                oldbook.Genre = book.Genre;
                oldbook.Author = book.Author;
                oldbook.Price = book.Price;
                oldbook.Title = book.Title;
                oldbook.Year = book.Year;
                var dto = new BookDTO()
                {
                    Id = book.Id,
                    Title = book.Title,
                    AuthorName = book.Author.Name
                };
                return CreatedAtRoute("DefaultApi", new { id = book.Id }, dto);
            }
            return NotFound();
        }

        // DELETE: api/Books/5
        public void Delete(int id)
        {
            init();
            if (_books.Any(b => b.Id == id))
            {
                var book = _books.FirstOrDefault(b => b.Id == id);
                _books.Remove(book);
            }
        }

        private void init()
        {
            if (_books == null)
            {
                _books = new List<Book>
                {
                    new Book {
                        Author = new Author { Name = "Koen" },
                        Id = 1,
                        Title = "Some Book",
                        Genre = "IT",
                        Price = 20,
                        Year = 1986
                    }
                };
            }
        }
    }
}
