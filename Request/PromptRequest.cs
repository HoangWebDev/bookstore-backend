namespace BookStore.Request
{
    public class PromptRequest
    {       
        public string Title { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public int DesiredLength { get; set; }
        public bool HasImage { get; set; }
    }
}
