using System.ComponentModel.DataAnnotations;

namespace LiraryManagement.Models.Domain
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsActive { get; set; }

        // Foreign key properties
        public int? CategoriesId { get; set; }  // Foreign key to Category
        public int? AuthorsId { get; set; }      // Foreign key to Author

        // Navigation properties
        public Category? Categories { get; set; }
        public Author? Authors { get; set; }
    }
}
