using Microsoft.AspNetCore.Mvc;

namespace ASP_201.Models.Forum
{
    public class SectionCreateModel
    {
        [FromForm(Name = "section-title")]
        public string Title       { get; set; } = null!;
        [FromForm(Name = "section-description")]
        public string Description { get; set; } = null!;
        [FromForm(Name = "logo-file")]
        public IFormFile Logo { get; set; } = null!;
    }
}
