namespace ASP_201.Models.Forum
{
    public class ForumThemeModel
    {
        public bool UserCanCreate { get; set; }
        public string ThemeId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public List<ForumTopicViewModel> Topics { get; set; } = null!;

        // Дані від створення нової теми
        public string? CreateMessage { get; set; }
        public bool? IsMessagePositive { get; set; }
        public ForumTopicFormModel FormModel { get; set; } = null!;

    }
}
