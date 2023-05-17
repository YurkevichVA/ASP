using ASP_201.Data;
using ASP_201.Models;
using ASP_201.Models.Home;
using ASP_201.Services;
using ASP_201.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_201.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DateService _dateService;
        private readonly TimeService _timeService;
        private readonly StampService _stampService;
        private readonly IHashService _hashService;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger,
                              DateService dateService,
                              TimeService timeService,
                              StampService stampService,
                              IHashService hashService,
                              DataContext dataContext,
                              IConfiguration configuration)
        {
            _logger         = logger;
            _dateService    = dateService;
            _timeService    = timeService;
            _stampService   = stampService;
            _hashService    = hashService;
            _dataContext    = dataContext;
            _configuration  = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Intro()
        {
            return View();
        }
        public IActionResult Url()
        {
            return View();
        }
        public IActionResult Scheme()
        {
            ViewBag.bagdata = "Data in ViewBag";
            ViewData["data"] = "Data in ViewData";

            return View();
        }
        public IActionResult Razor()
        {
            return View();
        }
        public IActionResult PassData()
        {
            PassDataModel model = new()
            {
                Header = "Моделі",
                Title = "Моделі передачі даних",
            };
            return View(model);
        }
        public IActionResult ProductTable()
        {
            ProductsModel model = new()
            {
                products = new()
                {
                    new() { Name = "Зарядний кабель",   Price = 210   },
                    new() { Name = "Мишка комп'ютерна", Price = 351.9 },
                    new() { Name = "Наліпка 'Smiley'",  Price = 5.5   },
                    new() { Name = "USB-кабель",        Price = 125.5 },
                    new() { Name = "Аккумулятор ААА",   Price = 280   }
                }
            };
            return View(model);
        }
        public IActionResult DisplayTemplates()
        {
            ProductsModel model = new()
            {
                products = new()
                {
                    new() { Name = "Зарядний кабель",   Price = 210   , Image = "puc3.jpg"},
                    new() { Name = "Мишка комп'ютерна", Price = 351.9 , Image = "puc2.jpg"},
                    new() { Name = "Наліпка 'Smiley'",  Price = 5.5   , Image = "puc3.jpg"},
                    new() { Name = "USB-кабель",        Price = 125.5 },
                    new() { Name = "Аккумулятор ААА",   Price = 280   }
                }
            };
            return View(model);
        }
        public IActionResult TagHelpers()
        {
            return View();
        }
        public ViewResult Services()
        {
            ViewData["date_service"]   = _dateService.GetMoment();
            ViewData["date_hashcode"]  = _dateService.GetHashCode();

            ViewData["time_service"]   = _timeService.GetMoment();
            ViewData["time_hashcode"]  = _timeService.GetHashCode();

            ViewData["stamp_service"]  = _stampService.GetMoment();
            ViewData["stamp_hashcode"] = _stampService.GetHashCode();

            ViewData["hash_service"]   = _hashService.Hash("123");
            return View();
        }
        public ViewResult Context()
        {
            ViewData["UserCount"] = _dataContext.Users.Count();
            return View();
        }
        public ViewResult Sessions([FromQuery(Name ="session-attr")]string? sessionAttr)
        {
            if(sessionAttr is not null)
            {
                HttpContext.Session.SetString("session-attribute", sessionAttr);
            }
            return View();
        }
        public ViewResult Middleware()
        {
            return View();
        }
        public ViewResult EmailConfirmation()
        {
            // дістаємо параметр з конфігурації
            EmailConfirmationModel model = new EmailConfirmationModel()
            {
                Host     = _configuration["Smtp:Gmail:Host"],
                Port     = Convert.ToInt32(_configuration["Smtp:Gmail:Port"]),
                Email    = _configuration["Smtp:Gmail:Email"],
                Password = _configuration["Smtp:Gmail:Password"],
                Ssl      = Convert.ToBoolean(_configuration["Smtp:Gmail:Ssl"])
            };
            return View(model);
        }
        public ViewResult WebApi()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}