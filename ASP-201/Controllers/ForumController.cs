using ASP_201.Data;
using ASP_201.Models.Forum;
using ASP_201.Models.User;
using ASP_201.Services.Random;
using ASP_201.Services.Transliteration;
using ASP_201.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_201.Controllers
{
    public class ForumController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ForumController> _logger;
        private readonly IValidationService _validationService;
        private readonly ITransliterationService _transliterationService;
        private readonly IRandomService _randomService;

        public ForumController  (DataContext dataContext,
                                 ILogger<ForumController> logger,
                                 IValidationService validationService, 
                                 ITransliterationService transliterationService, 
                                 IRandomService randomService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _validationService = validationService;
            _transliterationService = transliterationService;
            _randomService = randomService;
        }
        private int _counter = 0;
        private int Counter { get => _counter++; set => _counter = value; }
        public IActionResult Index()
        {
            Counter = 0;
            string? userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            ForumIndexModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Sections = _dataContext
                    .Sections
                    .Include(s => s.Author)     // Включити навігаційну властивість Author
                    .Include(s => s.RateList)
                    .Where(s => s.DeletedDt == null)
                    .OrderBy(s => s.CreatedDt)
                    .AsEnumerable()             // IQuerible -> IEnumerable
                    .Select(s => new SectionModel4View()
                    {
                        Title = s.Title,
                        Description = s.Description,
                        LogoUrl = string.IsNullOrEmpty(s.Logo) ? $"section{Counter}.png" : s.Logo,
                        CreatedDtString = DateTime.Today == s.CreatedDt.Date
                            ? "Сьогодні "
                            : s.CreatedDt.Date.ToString("dd.MM.yyyy hh:mm"),
                        UrlIdString = s.UrlId ?? s.Id.ToString(),
                        IdString = s.Id.ToString(),
                        // AuthorName - RealName або Login в залежності від налагоджень
                        AuthorName = s.Author.IsRealNamePublic ? s.Author.RealName : s.Author.Login,
                        AuthorAvatarUrl = s.Author.Avatar == null ? "/avatars/no-avatar.png" : $"/avatars/{s.Author.Avatar}",
                        LikesCount = s.RateList.Count(r => r.Rating > 0),
                        DislikesCount = s.RateList.Count(r => r.Rating < 0),
                        GivenRating = userId == null? null : s.RateList.FirstOrDefault(r => r.UserID == Guid.Parse(userId))?.Rating
                    })
                    .ToList()
            };
            if (HttpContext.Session.GetString("CreateSectionMessage") is string message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive = HttpContext.Session.GetInt32("IsMessagePositive") == 1;
                if(model.IsMessagePositive == false)
                {
                    model.formModel = new()
                    {
                        Title = HttpContext.Session.GetString("SavedTitle")!,
                        Description = HttpContext.Session.GetString("SavedDescription")!
                    };
                    HttpContext.Session.Remove("SavedTitle");
                    HttpContext.Session.Remove("SavedDescription");
                }
                HttpContext.Session.Remove("CreateSectionMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }
            return View(model);
        }
        public ViewResult Section([FromRoute] string id)
        {
            Guid sectionId;
            try
            {
                sectionId = Guid.Parse(id);
            }
            catch
            {
                sectionId = _dataContext.Sections.First(s => s.UrlId == id).Id;
            }
            string? userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            ForumSectionsModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                SectionId = sectionId.ToString(),
                Themes = _dataContext
                    .Themes
                    .Include(t => t.Author)
                    .Include(t => t.RateList)
                    .Where(t => t.DeletedDt == null && t.SectionId == sectionId)
                    .AsEnumerable()
                    .Select(t => new ThemeModel4View()
                    {
                        Title = t.Title,
                        Description = t.Description,
                        //LogoUrl = $"/img/logos/section{Counter}.png",
                        CreatedDtString = DateTime.Today == t.CreatedDt.Date
                            ? "Сьогодні "
                            : t.CreatedDt.Date.ToString("dd.MM.yyyy hh:mm"),
                        UrlIdString = t.Id.ToString(),
                        IdString = t.Id.ToString(),
                        SectionId = t.SectionId.ToString(),
                        // AuthorName - RealName або Login в залежності від налагоджень
                        AuthorName = t.Author.IsRealNamePublic ? t.Author.RealName : t.Author.Login,
                        AuthorLogin = t.Author.Login,
                        AuthorAvatarUrl = t.Author.Avatar == null ? "/avatars/no-avatar.png" : $"/avatars/{t.Author.Avatar}",
                        AuthorRegistrationDtString = t.Author.RegisterDt.ToString("dd.MM.yyyy"),
                        LikesCount = t.RateList.Count(r => r.Rating > 0),
                        DislikesCount = t.RateList.Count(r => r.Rating < 0),
                        GivenRating = userId == null ? null : t.RateList.FirstOrDefault(r => r.UserID == Guid.Parse(userId))?.Rating
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is string message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive = HttpContext.Session.GetInt32("IsMessagePositive") == 1;
                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Title = HttpContext.Session.GetString("SavedTitle")!,
                        Description = HttpContext.Session.GetString("SavedDescription")!
                    };
                    HttpContext.Session.Remove("SavedTitle");
                    HttpContext.Session.Remove("SavedDescription");
                }
                HttpContext.Session.Remove("CreateSectionMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }
        public IActionResult Theme([FromRoute] string id)
        {
            Guid themeId;
            try
            {
                themeId = Guid.Parse(id);
            }
            catch
            {
                themeId = Guid.Empty; // _dataContext.Themes.First(s => s.Id == id).Id;
            }
            var theme = _dataContext.Themes.Find(themeId);
            if (theme == null)
            {
                return NotFound();
            }
            ForumThemeModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Title = theme.Title,
                ThemeId = id,
                Topics = _dataContext
                    .Topics
                    .Include(t => t.Author)
                    .Where(t => t.DeletedDt == null && t.ThemeId == themeId)
                    .AsEnumerable()
                    .Select(t => new ForumTopicViewModel()
                    {
                        Title = t.Title,
                        Description =  t.Description.Length > 100? t.Description[..100] + "..." : t.Description,
                        UrlIdString = t.Id.ToString(),
                        CreatedDtString = DateTime.Today == t.CreatedDt.Date
                            ? "Сьогодні "
                            : t.CreatedDt.Date.ToString("dd.MM.yyyy hh:mm"),
                        AuthorName = t.Author.IsRealNamePublic ? t.Author.RealName : t.Author.Login,
                        AuthorLogin = t.Author.Login,
                        AuthorAvatarUrl = t.Author.Avatar == null ? "/avatars/no-avatar.png" : $"/avatars/{t.Author.Avatar}",
                        AuthorRegistrationDtString = t.Author.RegisterDt.ToString("dd.MM.yyyy")
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is string message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive = HttpContext.Session.GetInt32("IsMessagePositive") == 1;
                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Title = HttpContext.Session.GetString("SavedTitle")!,
                        Description = HttpContext.Session.GetString("SavedDescription")!
                    };
                    HttpContext.Session.Remove("SavedTitle");
                    HttpContext.Session.Remove("SavedDescription");
                }
                HttpContext.Session.Remove("CreateSectionMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }
        public IActionResult Topics([FromRoute] string id)
        {
            Guid topicId;
            try
            {
                topicId = Guid.Parse(id);
            }
            catch
            {
                topicId = Guid.Empty; // _dataContext.Themes.First(s => s.Id == id).Id;
            }
            var topic = _dataContext.Topics.Find(topicId);
            if (topic == null)
            {
                return NotFound();
            }
            ForumTopicsModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Title = topic.Title,
                Description = topic.Description,
                TopicId = id,
                Posts =
                    _dataContext
                    .Posts
                    .Include(p => p.Author)
                    .Include(p => p.Reply)
                    .Where(p => p.DeletedDt == null && p.TopicId == topicId)
                    .Select(p => new ForumPostViewModel
                    {
                        Content = p.Content,
                        AuthorName = p.Author.IsRealNamePublic ? p.Author.RealName : p.Author.Login,
                        //AuthorLogin = t.Author.Login,
                        AuthorAvatarUrl = p.Author.Avatar == null ? "/avatars/no-avatar.png" : $"/avatars/{p.Author.Avatar}",
                        //AuthorRegistrationDtString = t.Author.RegisterDt.ToString("dd.MM.yyyy")
                        UserCanReply = HttpContext.User.Identity.IsAuthenticated == true,
                        PostIdString = p.Id.ToString(),
                        ReplyContent = p.Reply.Content
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is string message)
            {
                model.CreateMessage = message;
                model.IsMessagePositive = HttpContext.Session.GetInt32("IsMessagePositive") == 1;
                if (model.IsMessagePositive == false)
                {
                    model.FormModel = new()
                    {
                        Content = HttpContext.Session.GetString("SavedContent")!,
                        ReplyId = HttpContext.Session.GetString("SavedReplyId")!
                    };
                    HttpContext.Session.Remove("SavedContent");
                    HttpContext.Session.Remove("SavedReplyId");
                }
                HttpContext.Session.Remove("CreateSectionMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }

            return View(model);
        }
        [HttpPost]
        public RedirectToActionResult CreateSection(SectionCreateModel formModel)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);

                string trans = _transliterationService.Transliterate(formModel.Title);
                string urlId = trans;
                int n = 2;
                while (_dataContext.Sections.Where(s => s.UrlId == urlId).Count() > 0)
                {
                    urlId = $"{trans}{n++}";
                }
                
                if (!_validationService.Validate(formModel.Title, ValidationTerms.NotEmpty)) throw new Exception("Назва не може бути порожнею");
                if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty)) throw new Exception("Опис не може бути порожним");

                #region Logo
                string savedName = null!;
                if (formModel.Logo is not null)
                {
                    // Генеруємо для файла нове ім'я, зберігаючи розширення
                    string ext = Path.GetExtension(formModel.Logo.FileName);
                    // TODO: перевірити розширення на перелік дозволених

                    string path = "";
                    do
                    {
                        savedName = _randomService.RandomFileName() + ext;
                        path = "wwwroot/img/logos/" + savedName;
                    } while (System.IO.File.Exists(path));

                    using FileStream fs = new(path, FileMode.Create);
                    formModel.Logo.CopyTo(fs);
                    ViewData["savedName"] = savedName;
                }
                #endregion

                _dataContext.Sections.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Title = formModel.Title,
                    Description = formModel.Description,
                    CreatedDt = DateTime.Now,
                    AuthorId = userId,
                    UrlId = _transliterationService.Transliterate(formModel.Title),
                    Logo = savedName
                });
                _dataContext.SaveChanges();
                HttpContext.Session.SetString("CreateSectionMessage", "Розділ успішно створено");
                HttpContext.Session.SetInt32("IsMessagePositive", 1);
            }
            catch(Exception ex)
            {
                if(ex is ArgumentNullException) HttpContext.Session.SetString("CreateSectionMessage", "Відмовлено в авторизації");
                else HttpContext.Session.SetString("CreateSectionMessage", ex.Message);

                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public RedirectToActionResult CreateTheme(ThemeCreateModel formModel)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);

                if (!_validationService.Validate(formModel.Title, ValidationTerms.NotEmpty)) throw new Exception("Назва не може бути порожнею");
                if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty)) throw new Exception("Опис не може бути порожним");

                _dataContext.Themes.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Title = formModel.Title,
                    Description = formModel.Description,
                    CreatedDt = DateTime.Now,
                    AuthorId = userId,
                    SectionId = Guid.Parse(formModel.SectionId)
                });
                _dataContext.SaveChanges();
                HttpContext.Session.SetString("CreateSectionMessage", "Розділ успішно створено");
                HttpContext.Session.SetInt32("IsMessagePositive", 1);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException) HttpContext.Session.SetString("CreateSectionMessage", "Відмовлено в авторизації");
                else HttpContext.Session.SetString("CreateSectionMessage", ex.Message);

                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            return RedirectToAction(nameof(Section), new { id = formModel.SectionId });
        }
        [HttpPost]
        public RedirectToActionResult CreateTopic(ForumTopicFormModel formModel)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);

                string trans = _transliterationService.Transliterate(formModel.Title);
                string urlId = trans;
                int n = 2;
                while (_dataContext.Sections.Where(s => s.UrlId == urlId).Count() > 0)
                {
                    urlId = $"{trans}{n++}";
                }

                if (!_validationService.Validate(formModel.Title, ValidationTerms.NotEmpty)) throw new Exception("Назва не може бути порожнею");
                if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty)) throw new Exception("Опис не може бути порожним");

                _dataContext.Topics.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Title = formModel.Title,
                    Description = formModel.Description,
                    CreatedDt = DateTime.Now,
                    AuthorId = userId,
                    ThemeId = Guid.Parse(formModel.ThemeId)
                });
                _dataContext.SaveChanges();
                HttpContext.Session.SetString("CreateSectionMessage", "Розділ успішно створено");
                HttpContext.Session.SetInt32("IsMessagePositive", 1);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException) HttpContext.Session.SetString("CreateSectionMessage", "Відмовлено в авторизації");
                else HttpContext.Session.SetString("CreateSectionMessage", ex.Message);

                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            return RedirectToAction(nameof(Theme), new { id = formModel.ThemeId });
        }
        [HttpPost]
        public RedirectToActionResult CreatePost(ForumPostFormModel formModel)
        {
            Guid userId;
            try
            {
                userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);

                //string trans = _transliterationService.Transliterate(formModel.Title);
                //string urlId = trans;
                //int n = 2;
                //while (_dataContext.Sections.Where(s => s.UrlId == urlId).Count() > 0)
                //{
                //    urlId = $"{trans}{n++}";
                //}

                if (!_validationService.Validate(formModel.Content, ValidationTerms.NotEmpty)) throw new Exception("Відповідь не може бути порожнею");

                _dataContext.Posts.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Content = formModel.Content,
                    ReplyId = String.IsNullOrEmpty(formModel.ReplyId) ? null : Guid.Parse(formModel.ReplyId),
                    CreatedDt = DateTime.Now,
                    AuthorId = userId,
                    TopicId = Guid.Parse(formModel.TopicId)
                });
                _dataContext.SaveChanges();
                HttpContext.Session.SetString("CreateSectionMessage", "Відповідь опубліковано");
                HttpContext.Session.SetInt32("IsMessagePositive", 1);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException) HttpContext.Session.SetString("CreateSectionMessage", "Відмовлено в авторизації");
                else HttpContext.Session.SetString("CreateSectionMessage", ex.Message);

                HttpContext.Session.SetInt32("IsMessagePositive", 0);
                HttpContext.Session.SetString("SavedContent", formModel.Content ?? String.Empty);
                HttpContext.Session.SetString("SavedReplyId", formModel.ReplyId ?? String.Empty);
            }
            return RedirectToAction(nameof(Topics), new {id = formModel.TopicId});
        }
    }
}
