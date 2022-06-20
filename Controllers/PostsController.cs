using csharp_blog_backend.Models;
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
        public async Task<ActionResult<IEnumerable<Post>>> GetListaPost(string? stringa)
        {
            if (_context.ListaPost == null)
            {
                return NotFound();
            }
            if (stringa != null)
            {

                return await _context.ListaPost.Where(m => m.Title.Contains(stringa) || m.Description.Contains(stringa)).ToListAsync();
            }
            else
            {
                return await _context.ListaPost.ToListAsync();
            }
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Post>>> GetListaPost(string stringa)
        //{
        //    if (_context.ListaPost == null)
        //    {
        //        return NotFound();
        //    }
        //    if (stringa != null)
        //    {

        //        return await _context.ListaPost.Where(m => m.Title.Contains(stringa) || m.Description.Contains(stringa)).ToListAsync();
        //    }
        //    else
        //    {
        //        return await _context.ListaPost.ToListAsync();
        //    }
        //}

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
            //string fileName = "immagine-" + post.Id + "." + post.Image.Substring("FileLocal;".Length);
            //string Image = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files");
            ////string fileNameWithPath = Path.Combine(Image, fileName);
            //if (!Directory.Exists(Image))
            //    return NotFound();



            //using (var stream = new FileStream(fileNameWithPath, FileMode.Open)
            //using (var stream = System.IO.F FileMode.Open)
            //{
            //    post.File = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
            //    //post.File.OpenReadStream();
            //}


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
        public async Task<ActionResult<Post>> PostPost([FromForm] Post post)
        {
            if (_context.ListaPost == null)
            {
                return Problem("Entity set 'BlogContext.ListaPost'  is null.");
            }

            FileInfo fileInfo = new FileInfo(post.File.FileName);

            string Image = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files");
            if (!Directory.Exists(Image))
                Directory.CreateDirectory(Image);
            //FileInfo fileInfo = new FileInfo(post.File.FileName);
            Guid g = Guid.NewGuid();

            string fileName = g.ToString() + fileInfo.Extension;
            string fileNameWithPath = Path.Combine(Image, fileName);
            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                post.File.CopyTo(stream);
            }


            // post.Image = $"FileLocal;{fileInfo.Extension}";


            post.Image = "https://localhost:500/Files/" + fileName;
            //_context.ListaPost.Add(post);
            //await _context.SaveChangesAsync();

            // salviamo anche il file come  varBinaryMAx nel DB
            //in questa parte c'è il salvataggio a db per un file blog

            byte[] b;

            //per leggerlo in html basta usare <img src="data:image/png;base64,iVBORw0KGgoAAAANSU ...">

            using (BinaryReader br = new BinaryReader(post.File.OpenReadStream()))

            {
                post.ImageBytes = br.ReadBytes((int)post.File.OpenReadStream().Length);
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
