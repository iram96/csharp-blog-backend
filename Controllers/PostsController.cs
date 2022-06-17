﻿using csharp_blog_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_blog_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly BlogContext _context;

        public PostsController(BlogContext context)
        {
            _context = context;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetListaPost()
        {
            if (_context.ListaPost == null)
            {
                return NotFound();
            }
            return await _context.ListaPost.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            if (_context.ListaPost == null)
            {
                return NotFound();
            }
            var post = await _context.ListaPost.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            if (_context.ListaPost == null)
            {
                return Problem("Entity set 'BlogContext.ListaPost'  is null.");
            }

            string Image = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files");
            if (!Directory.Exists(Image))
                Directory.CreateDirectory(Image);
            FileInfo fileInfo = new FileInfo(post.File.FileName);
            string fileName = post.Title + fileInfo.Extension;
            string fileNameWithPath = Path.Combine(Image, fileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                post.File.CopyTo(stream);
            }

            _context.ListaPost.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (_context.ListaPost == null)
            {
                return NotFound();
            }
            var post = await _context.ListaPost.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.ListaPost.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return (_context.ListaPost?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
