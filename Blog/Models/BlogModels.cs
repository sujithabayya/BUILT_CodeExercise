namespace Blog.Models
{


    public class PostModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Contents { get; set; }
        public DateTime TimeStamp { get; set; }
        public int CategoryId { get; set; }
    }
    public class CategoryModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

}
