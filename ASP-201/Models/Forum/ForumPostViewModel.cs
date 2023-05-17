namespace ASP_201.Models.Forum
{
    public class ForumPostViewModel
    {
        public string Content { get; set; } = null!;

        //Author data
        public string AuthorName { get; set; } = null!;
        //public string AuthorLogin { get; set; } = null!;
        public string AuthorAvatarUrl { get; set; } = null!;
        //public string AuthorRegistrationDtString { get; set; } = null!;
        public bool UserCanReply { get; set; }
        public string PostIdString { get; set; } = null!;
        public string? ReplyContent { get; set; }
    }
}
