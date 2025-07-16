namespace DEMO_CRUD.Models.Entity
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get => title; set => title = value; }

        private string title;

        public Book(string title)
        {
            this.title = title;
        }
    }
}
