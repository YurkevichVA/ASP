namespace ASP_201.Data.Entity
{
    public class Rate
    {
        public Guid ItemId { get; set; }
        public Guid UserID { get; set; }
        public int  Rating { get; set; }
    }
}
