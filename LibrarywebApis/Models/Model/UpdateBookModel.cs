namespace LibrarywebApis.Models.Model
{
    public class UpdateBookModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }

    }
}
