namespace Library.Objects.Helpers.Response
{
    public class MessageResponse
    {
        public string Message { get; }

        public MessageResponse(string message)
        {
            Message = message;
        }
    }
}
