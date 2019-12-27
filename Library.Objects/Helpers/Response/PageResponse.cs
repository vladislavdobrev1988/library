namespace Library.Objects.Helpers.Response
{
    public class PageResponse<T>
    {
        public T[] Page { get; }
        public int Total { get; }

        public PageResponse(T[] page, int total)
        {
            Page = page;
            Total = total;
        }
    }
}
