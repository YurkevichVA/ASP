namespace ASP_201.Data.Entity
{
    public class Section
    {
        public Guid         Id          { get; set; }
        public string       Title       { get; set; } = null!;
        public string       Description { get; set; } = null!;
        public Guid         AuthorId    { get; set; }
        public DateTime     CreatedDt   { get; set; }
        public DateTime?    DeletedDt   { get; set; }
        public string?      Logo        { get; set; }
        public string?      UrlId       { get; set; } 

        public User         Author      { get; set; } = null!;
        public List<Rate>   RateList    { get; set; } = null!;

    }
}
