namespace LiraryManagement.Models.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
