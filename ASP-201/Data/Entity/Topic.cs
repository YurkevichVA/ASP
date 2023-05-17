namespace ASP_201.Data.Entity
{
    public class Topic
    {
        public Guid         Id          { get; set; }
        public Guid         ThemeId     { get; set; }
        public Guid         AuthorId    { get; set; }
        public string       Title       { get; set; } = null!;
        public string       Description { get; set; } = null!;
        public DateTime     CreatedDt   { get; set; }
        public DateTime?    DeletedDt   { get; set; }

        // Navigational
        public User Author { get; set; } = null!;
    }
}
