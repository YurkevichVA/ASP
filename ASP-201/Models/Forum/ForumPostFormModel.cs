using Microsoft.AspNetCore.Mvc;

namespace ASP_201.Models.Forum
{
    public class ForumPostFormModel
    {
        [FromForm(Name = "post-content")]
        public string Content { get; set; } = null!;
        [FromForm(Name = "topic-id")]
        public string TopicId { get; set; } = null!;
        [FromForm(Name ="reply-id")]
        public string? ReplyId { get; set; }
    }
}
