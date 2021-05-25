namespace API.DTOs
{
    public class DocumentDto
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string User { get; set; }
        public string Affair { get; set; }
        public string Title { get; set; }
        public string Hash { get; set; }

        public UserDto UserDto { get; set; }
    }
}