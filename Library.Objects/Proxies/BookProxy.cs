using Library.Objects.Proxies.Base;

namespace Library.Objects.Proxies
{
    public class BookProxy : BaseProxy
    {
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public int AuthorId { get; set; }
    }
}
