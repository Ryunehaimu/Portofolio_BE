using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioBackend.Data;
using PortfolioBackend.Models;
using System.Text.Json;

namespace PortfolioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PagesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: Ambil Data Page (Dipakai saat Frontend Load)
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetPage(string slug)
        {
            var page = await _context.Pages.FirstOrDefaultAsync(p => p.Slug == slug);

            if (page == null)
            {
                // Kalau page belum ada, return object kosong biar Frontend tidak error
                return Ok(new
                {
                    slug = slug,
                    title = "New Page",
                    blocks = new List<object>()
                });
            }

            // Convert JSON String dari DB menjadi Object Asli
            var blocksObj = string.IsNullOrEmpty(page.ContentJson)
                            ? new List<object>()
                            : JsonSerializer.Deserialize<object>(page.ContentJson);

            return Ok(new
            {
                slug = page.Slug,
                title = page.Title,
                blocks = blocksObj
            });
        }

        // 2. POST: Simpan Data Page (Dipakai saat tombol Save diklik)
        [HttpPost("{slug}")]
        public async Task<IActionResult> SavePage(string slug, [FromBody] PageDto dto)
        {
            var page = await _context.Pages.FirstOrDefaultAsync(p => p.Slug == slug);

            // Convert Object dari React menjadi JSON String untuk SQL
            string jsonString = JsonSerializer.Serialize(dto.Blocks);

            if (page == null)
            {
                // Create Baru
                page = new Page
                {
                    Slug = slug,
                    Title = dto.Title ?? slug,
                    ContentJson = jsonString,
                    LastUpdated = DateTime.Now
                };
                _context.Pages.Add(page);
            }
            else
            {
                // Update Existing
                page.Title = dto.Title ?? page.Title;
                page.ContentJson = jsonString;
                page.LastUpdated = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Saved successfully" });
        }

        // 3. GET ALL: Untuk Dashboard List
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pages = await _context.Pages
                .Select(p => new { p.Id, p.Title, p.Slug, p.LastUpdated })
                .ToListAsync();
            return Ok(pages);
        }

        [HttpDelete("{slug}")]
        public async Task<IActionResult> DeletePage(string slug)
        {
            var page = await _context.Pages.FirstOrDefaultAsync(p => p.Slug == slug);
            if (page == null)
            {
                return NotFound(new { message = "Page not found" });
            }

            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Page deleted successfully" });
        }
    }

    // DTO Helper untuk menangkap data dari React
    public class PageDto
    {
        public string Title { get; set; }
        public object Blocks { get; set; }
    }
}