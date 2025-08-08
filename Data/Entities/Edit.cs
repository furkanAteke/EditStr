using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class Edit
    {
        public int Id { get; set; }
        public string EditName { get; set; }
        public string EditFull { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
        public string? Description { get; set; }
    }
}
