namespace ASP_201.Models.Forum
{
    public class ForumIndexModel
    {
        public List<Forum.SectionModel4View> Sections { get; set; } = null!;
        public bool UserCanCreate { get; set; }

        // Дані від створення нової секції
        public string? CreateMessage { get; set; }
        public bool? IsMessagePositive { get; set; }
        public SectionCreateModel? formModel { get; set; }
    }
}
