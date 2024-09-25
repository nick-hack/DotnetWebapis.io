namespace LibrarywebApis.Models.Model
{
    public class AddBookModel
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }
        public int? CategoriesId { get; set; }  // Foreign key to Category
        public int? AuthorsId { get; set; }      // Foreign key to Author
    }
}
