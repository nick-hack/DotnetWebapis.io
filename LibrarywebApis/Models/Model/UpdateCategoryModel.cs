namespace LibrarywebApis.Models.Model
{
    public class UpdateCategoryModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
