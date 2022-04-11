namespace Commons.Models.Database
{
    public class Visit
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Visitors { get; set; }
        public string Country { get; set; }
        public DateTime Date { get; set; }
    }
}
