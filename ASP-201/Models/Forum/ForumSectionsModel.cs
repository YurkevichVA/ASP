namespace ASP_201.Models.Forum
{
    public class ForumSectionsModel
    {
        public List<ThemeModel4View> Themes { get; set; } = null!;
        public bool UserCanCreate { get; set; }
        public string SectionId { get; set; } = null!;

        // Дані від створення нової секції
        public string? CreateMessage { get; set; }
        public bool? IsMessagePositive { get; set; }
        public ThemeCreateModel FormModel { get; set; } = null!;
    }
}
