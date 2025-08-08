using Microsoft.AspNetCore.Mvc.Rendering;
namespace EditStr.Models
{
    public class GenericViewModel<T>
    {
        public T Response { get; set; }
        public List<SelectListItem> ParentList { get; set; }
        public List<SelectListItem> ChildList { get; set; }
    }
}
