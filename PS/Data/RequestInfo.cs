using System.ComponentModel.DataAnnotations.Schema;

namespace PS.Data
{
    public class RequestInfo
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        
        public string? IP { get; set; }
        public string? Mac { get; set; }
        public string? Url { get; set; }
        public string? Method { get; set; }
    }
}
