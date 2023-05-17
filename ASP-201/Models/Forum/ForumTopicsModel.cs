namespace ASP_201.Models.Forum
{
    public class ForumTopicsModel
    {
        public bool UserCanCreate { get; set; }
        public string TopicId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<ForumPostViewModel> Posts { get; set; } = null!;

        // Дані від створення нової теми
        public string? CreateMessage { get; set; }
        public bool? IsMessagePositive { get; set; }
        public ForumPostFormModel FormModel { get; set; } = null!;
    }
}
