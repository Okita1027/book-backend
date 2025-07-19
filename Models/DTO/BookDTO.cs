namespace DEMO_CRUD.Models.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string PublishedDate { get; set; }
        public int Stock { get; set; }
        public int Available { get; set; }

        // 作者的名字
        public string AuthorName { get; set; }

        // 出版社的名字
        public string PublisherName { get; set; }
    }
}
