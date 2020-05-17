using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotesApi.Data;
using QuotesApi.Models;

namespace QuotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class QuotesController : ControllerBase
    {
        private QuotesDbContext _quotesDbContext;
        public QuotesController(QuotesDbContext quotesDbContext)
        {
            _quotesDbContext = quotesDbContext;
        }
        // GET: api/Quotes
        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        [AllowAnonymous]
        public IActionResult Get(string sort)
        {
            IQueryable<Quote> quotes;
            switch (sort)
            {
                case "desc":
                    quotes = _quotesDbContext.Quotes.OrderByDescending(q => q.CreatedAt);
                    break;
                case "asc":
                    quotes = _quotesDbContext.Quotes.OrderBy(q => q.CreatedAt);
                    break;
                default:
                    quotes = _quotesDbContext.Quotes;
                    break;
            }
            return StatusCode(StatusCodes.Status200OK, quotes);
        }

        // GET: api/Quotes/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            if (_quotesDbContext.Quotes.Find(id) == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "No record found against this id...");
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, _quotesDbContext.Quotes.Find(id));
            }

        }

        // POST: api/Quotes
        [HttpPost]
        public IActionResult Post([FromBody] Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            quote.UserId = userId;
            _quotesDbContext.Quotes.Add(quote);
            _quotesDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT: api/Quotes/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Quote q = _quotesDbContext.Quotes.Find(id);
            if (q == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "No record found against this id...");
            }
            if (userId != q.UserId)
            {
                return BadRequest("Sorry you can't update this record...");
            }
            else
            {
                q.Title = quote.Title;
                q.Author = quote.Author;
                q.Description = quote.Description;
                q.CreatedAt = quote.CreatedAt;
                _quotesDbContext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, "Record updated Succesfully...");
            }

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Quote q = _quotesDbContext.Quotes.Find(id);
            if (q == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "No record found against this id...");
            }
            if (userId != q.UserId)
            {
                return BadRequest("You can't delete this record...");
            }
            else
            {
                _quotesDbContext.Quotes.Remove(q);
                _quotesDbContext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK, "Record Deleted Successfully.");
            }

        }

        [HttpGet("[action]")]
        public IActionResult PagingQuote(int pageNumber = 1, int pageSize = 1)
        {
            var quotes = _quotesDbContext.Quotes;
            return Ok(quotes.Skip((pageNumber - 1) * pageSize).Take(pageSize));
        }

        [HttpGet("[action]")]
        public IActionResult SearchQuote(string title)
        {
            var quotes = _quotesDbContext.Quotes.Where(q => q.Title.StartsWith(title));
            return Ok(quotes);
        }

        [HttpGet("[action]")]
        public IActionResult MyQuote()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var quotes = _quotesDbContext.Quotes.Where(q => q.UserId == userId);
            return Ok(quotes);
        }
    }
}
