namespace ASP_201.Models.Forum
{
    public class SectionModel4View
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public string CreatedDtString { get; set; } = null!;
        public string UrlIdString { get; set; } = null!;
        public string IdString { get; set; } = null!;
        //public string TransliterationTitle { get; set; } = null!;

        // Author
        public string AuthorName { get; set; } = null!;
        public string AuthorAvatarUrl { get; set; } = null!;
        // Rating
        public int    LikesCount { get; set; }
        public int    DislikesCount { get; set; }
        public int? GivenRating { get; set; }
    }
}
