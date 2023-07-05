using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novels.Models;

namespace Novels.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookcategoriesController : ControllerBase
    {
        private readonly NovelStoreContext _context;

        public BookcategoriesController(NovelStoreContext context)
        {
            _context = context;
        }

        // GET: api/Bookcategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookcategory>>> GetBookcategories()
        {
            return await _context.Bookcategories.ToListAsync();
        }

        // GET: api/Bookcategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bookcategory>> GetBookcategory(int id)
        {
            var bookcategory = await _context.Bookcategories.FindAsync(id);

            if (bookcategory == null)
            {
                return NotFound();
            }

            return bookcategory;
        }

        // PUT: api/Bookcategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PutBookcategory(int id, Bookcategory bookcategory)
        {
            if (id != bookcategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(bookcategory).Property(p => p.CategoryId).IsModified = true;
            _context.Entry(bookcategory).Property(p => p.BooKId).IsModified = true;


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookcategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bookcategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bookcategory>> PostBookcategory(Bookcategory bookcategory)
        {
            _context.Bookcategories.Add(bookcategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookcategory", new { id = bookcategory.Id }, bookcategory);
        }

        // DELETE: api/Bookcategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookcategory(int id)
        {
            var bookcategory = await _context.Bookcategories.FindAsync(id);
            if (bookcategory == null)
            {
                return NotFound();
            }

            _context.Bookcategories.Remove(bookcategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookcategoryExists(int id)
        {
            return _context.Bookcategories.Any(e => e.Id == id);
        }
    }
}
