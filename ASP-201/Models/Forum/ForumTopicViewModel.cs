namespace ASP_201.Models.Forum
{
    public class ForumTopicViewModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CreatedDtString { get; set; } = null!;
        public string UrlIdString { get; set; } = null!;

        //Author data
        public string AuthorName { get; set; } = null!;
        public string AuthorLogin { get; set; } = null!;
        public string AuthorAvatarUrl { get; set; } = null!;
        public string AuthorRegistrationDtString { get; set; } = null!;
    }
}
