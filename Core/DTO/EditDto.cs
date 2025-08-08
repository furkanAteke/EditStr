using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.DTO
{
    public class EditDto
    {
        public int Id { get; set; }
        public string EditName { get; set; }
        public string? EditFull { get; set; }
        public IFormFile EditPath { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public int CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public string? Description { get; set; }
    }
}
