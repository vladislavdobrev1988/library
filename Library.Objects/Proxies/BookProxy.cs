using Library.Objects.Proxies.Base;

namespace Library.Objects.Proxies
{
    public class BookProxy : BaseProxy
    {
        public string Title { get; set; }
        public string PublishDate { get; set; }
        public int AuthorId { get; set; }
    }
}
