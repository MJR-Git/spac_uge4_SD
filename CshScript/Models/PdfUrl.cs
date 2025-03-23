namespace CshScript.Models
{
    public class PdfUrl
    {
        public string Brnummer { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? AlternativeUrl { get; set; }
        public bool Downloaded { get; set; } = false;
    }
}