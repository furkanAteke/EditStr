using Core.DTO;

namespace EditStr.Models
{
    public class CategoryEditViewModel
    {
        public IReadOnlyList<CategoryDto>? Category { get; set; }
        public IReadOnlyList<EditDto>? Edits { get; set; }
        public EditDto? Edit { get; set; }
    }
}
