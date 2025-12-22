using System.ComponentModel.DataAnnotations;

namespace PortfolioBackend.Models
{
    public class Page
    {
        [Key]
        public int Id { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string ContentJson { get; set; } // String JSON
        public DateTime LastUpdated { get; set; }
    }
}