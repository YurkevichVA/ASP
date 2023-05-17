using ASP_201.Data;
using ASP_201.Data.Entity;
using ASP_201.Migrations;
using ASP_201.Models;
using ASP_201.Models.User;
using ASP_201.Services.Email;
using ASP_201.Services.Hash;
using ASP_201.Services.Kdf;
using ASP_201.Services.Random;
using ASP_201.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ASP_201.Controllers
{
    public class UserController : Controller
    {
        private readonly IHashService _hashService;
        private readonly ILogger<UserController> _logger;
        private readonly DataContext _dataContext;
        private readonly IRandomService _randomService;
        private readonly IKdfService _kdfService;
        private readonly IValidationService _validationService;
        private readonly IEmailService _emailService;

        public UserController(IHashService hashService,
                              ILogger<UserController> logger, 
                              DataContext dataContext, 
                              IRandomService randomService, 
                              IKdfService kdfService, 
                              IEmailService emailService, 
                              IValidationService validationService)
        {
            _hashService       = hashService;
            _logger            = logger;
            _dataContext       = dataContext;
            _randomService     = randomService;
            _kdfService        = kdfService;
            _emailService      = emailService;
            _validationService = validationService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }
        public IActionResult Register(RegistrationModel registrationModel)
        {
            bool isModelValid = true;
            byte minPasswordLength = 3;
            RegisterValidationModel registerValidation = new();

            #region Login Validation
            if (String.IsNullOrEmpty(registrationModel.Login))
            {
                registerValidation.LoginMessage = "Логін не може бути порожнім!";
                isModelValid = false;
            }
            if (_dataContext.Users.Any(u => u.Login == registrationModel.Login))
            {
                registerValidation.LoginMessage = "Логін вже використовується!";
                isModelValid = false;
            }
            #endregion

            #region Password / Repeat Validation
            if (String.IsNullOrEmpty(registrationModel.Password))
            {
                registerValidation.PasswordMessage = "Треба ввести пароль!";
                registerValidation.RepeatPasswordMessage = "";
                isModelValid = false;
            }
            else if(registrationModel.Password.Length < minPasswordLength)
            {
                registerValidation.PasswordMessage = $"Пароль закороткий. Щонайменьше {minPasswordLength} символів";
                registerValidation.RepeatPasswordMessage = "";
                isModelValid = false;
            }
            else if(!registrationModel.Password.Equals(registrationModel.RepeatPassword)) 
            {
                registerValidation.PasswordMessage =
                    registerValidation.RepeatPasswordMessage = "Паролі не збігаються!";
                isModelValid = false;
            }

            if (String.IsNullOrEmpty(registrationModel.RepeatPassword))
            {
                registerValidation.RepeatPasswordMessage = "Треба повторити пароль!";
                isModelValid = false;
            }

            #endregion

            #region Email Validation
            if (!_validationService.Validate(registrationModel.Email, ValidationTerms.NotEmpty))
            {
                registerValidation.EmailMessage = "Необхідно ввести e-mail!";
                isModelValid = false;
            }
            else if (!_validationService.Validate(registrationModel.Email, ValidationTerms.Email))
            {
                registerValidation.EmailMessage = "Email не відповідає шаблону";
                isModelValid = false;
            }
            #endregion

            #region RealName Validation
            if (String.IsNullOrEmpty(registrationModel.RealName))
            {
                registerValidation.RealNameMessage = "Ім'я не може бути порожнім!";
                isModelValid = false;
            }
            else
            {
                String nameRegex = @"^.+$";
                if (!Regex.IsMatch(registrationModel.RealName, nameRegex))
                {
                    registerValidation.RealNameMessage = "Ім'я не відповідає шаблону";
                    isModelValid = false;
                }
            }
            #endregion

            #region Avatar
            // вважаємо аватар не обов'язковим
            string savedName = null!;
            if (registrationModel.Avatar is not null)
            {
                if (registrationModel.Avatar.Length < 1024)
                {
                    registerValidation.AvatarMessage = "Файл замалий!";
                    isModelValid = false;
                }
                else
                {
                    // Генеруємо для файла нове ім'я, зберігаючи розширення
                    string ext = Path.GetExtension(registrationModel.Avatar.FileName);
                    // TODO: перевірити розширення на перелік дозволених

                    string path = "wwwroot/avatars/";
                    do
                    {
                        savedName = _randomService.RandomFileName() + ext;
                        path = "wwwroot/avatars/" + savedName;
                    } while (System.IO.File.Exists(path));

                    using FileStream fs = new(path, FileMode.Create);
                    registrationModel.Avatar.CopyTo(fs);
                    ViewData["savedName"] = savedName;
                }
            }
            else
            {
                savedName = "no-avatar.png";
            }
            #endregion

            #region IsAgree Validation
            if (!registrationModel.IsAgree)
            {
                registerValidation.IsAgreeMessage = "Необхідно погодитись з правилами сайту!";
                isModelValid = false;
            }
            #endregion


            // якщо всі перевірки пройдені, то переходимо на нову сторінку з вітаннями
            if (isModelValid)
            {
                string salt = _randomService.RandomString(16);
                string confirmEmailCode = _randomService.ConfirmCode(6);
                User user = new()
                {
                    Id = Guid.NewGuid(),
                    Login = registrationModel.Login,
                    RealName = registrationModel.RealName,
                    Email = registrationModel.Email,
                    EmailCode = confirmEmailCode,
                    PasswordSalt = salt,
                    PasswordHash = _kdfService.GetDerivedKey(registrationModel.Password, salt),
                    Avatar = savedName,
                    RegisterDt = DateTime.Now,
                    LastEnter = null
                };
                _dataContext.Users.Add(user);
                // Якщо дані до БД додано, надсилаємо код підтвердження на пошту
                // Генеруємо токен автоматичного підтвердження
                var emailConfirmToken = _GenerateEmailConfirmToken(user);

                // Надсилаємо листа з токеном
                _SendConfirmEmail(user, emailConfirmToken);

                _dataContext.SaveChangesAsync();

                _emailService.Send(
                    "welcome_letter",
                    new Models.Email.ConfirmEmailModel
                    {
                        Email = user.Email,
                        RealName = user.RealName,
                        EmailCode = user.EmailCode,
                        ConfirmLink = "#"
                    });


                return View(registrationModel);
            }
            else // не всі форми влаідні - повертаємо на форму реєстрації
            {
                // передаємо дані щодо перевірок
                ViewData["registerValidation"] = registerValidation;
                // спосіб перейти на View з іншою назвою, ніж метод
                return View("Registration");
            }
        }
        [HttpPost] // метод доступний тільки для POST-запитів
        public string AuthUser()
        {
            // альтернативний (до моделей) спосіб отримання параметрів запиту
            StringValues loginValues = Request.Form["user-login"];
            StringValues passValues = Request.Form["user-password"];
            // колекція loginValues формується при будь-якому ключі, але для неправильних (відсутніх) ключів вона порожня.
            if(loginValues.Count == 0)
            {
                // немає логіну у складі полів
                return "Missed required parameter: user-login";
            }
            if(passValues.Count== 0)
            {
                return "Missed required parameter: user-password";
            }

            string login = loginValues[0] ?? "";
            string password = passValues[0] ?? "";

            User? user = _dataContext.Users.Where(u => u.Login == login).FirstOrDefault();
            if(user is not null)
            {
                // якщо знайшли - перевіряємо пароль (derived key)
                if(user.PasswordHash == _kdfService.GetDerivedKey(password, user.PasswordSalt))
                {
                    // дані перевірені - користувач автентифікований - зберігаємо у сесії
                    HttpContext.Session.SetString("authUserId", user.Id.ToString());
                    return "OK";
                }
            }

            return "Авторизацію відхилено";
        }
        //public ViewResult Logout()
        //{
        //    HttpContext.Session.Remove("authUserId");
        //    HttpContext.Items.Remove("AuthUser");
        //    return View("~/Views/Home/Index.cshtml");
        //}
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Remove("authUserId");
            return RedirectToAction("Index", "Home");
            /* Redirect та інші питання з перенаправлення
             * Browser          Server
             *  GET /home ----> (routing)->Home::Index()->View()
             *       page <---- 200 OK <!DOCTYPE html>...   
             *       
             *          GET /Logout
             * <a Logout> ----> User::Logout()->Redirect(...)
             *     follow <---- 302 (Redirect) Location: /home
             *  GET /home ----> (routing)->Home::Index()->View()
             *       page <---- 200 OK <!DOCTYPE html>...  
             *       
             * 
             * 301 - Permanent redirect - перенесено на постійній основі, 
             *  як правило, сайт змінив URL
             * Довільний редірект слідується GET запитом, якщо потрібно зберігти початковий метод, то вживається
             * Redirect...PreserveMethod
             * 
             * 30x Redirect називають зовнішніми, тому що інформація доходить до бравзера і змінюється URL в адресному рядку
             * http://..../addr1  ---> 302 Location /addr2
             * http://..../addr2  ---> 200 html
             * 
             *                             addr1.asp
             * http://..../addr1 (if..) \  addr2.asp
             *                           \ addr3.asp
             *                      froward - внутрішнє перенаправлення
             *              (у бравзурі /addr1, фле фактично відображено addr3.asp)
             */
        }
        // /User/Profile/Admin : User-controller, Profile-action, Admin-id
        public IActionResult Profile([FromRoute] string id)
        {
            // Задача: реалізувати можливість розрізнення власного та чужого профіля
            Data.Entity.User? user = _dataContext.Users.FirstOrDefault(u => u.Login == id);
            if (user is not null)
            {
                Models.User.ProfileModel model = new(user);
                if(HttpContext.User.Identity is not null && HttpContext.User.Identity.IsAuthenticated)
                {
                    string userLogin = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                    if (userLogin == user.Login) 
                    {
                        model.IsPersonal = true;
                    }
                }
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut]
        public IActionResult Update([FromBody] UpdateRequestModel model)
        {
            UpdateResponseModel responseModel = new();
            try
            {
                if (model is null) throw new Exception("No or empty data");
                if(HttpContext.User.Identity?.IsAuthenticated == false)
                {
                    throw new Exception("UnAuthenticated");
                }
                User? user = _dataContext.Users.Find(
                    Guid.Parse(
                        HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value
                ));
                if (user is null) throw new Exception("UnAuthorized");
                switch(model.Field)
                {
                    case "realname":
                        if (!_validationService.Validate(model.Value, ValidationTerms.RealName))
                        {

                            throw new Exception($"Validation error: field '{model.Field}' with value '{model.Value}'");
                        }
                        user.RealName = model.Value;
                        _dataContext.SaveChanges();
                        break;
                    case "email":
                        if (!_validationService.Validate(model.Value, ValidationTerms.Email))
                        {
                            throw new Exception("Invalid 'Value' attribute");
                        }
                        user.Email = model.Value;

                        _dataContext.SaveChanges();
                        
                        ResendConfirmEmail();

                        break;
                    default:
                        throw new Exception("Invalid 'Field' attribute");
                }
                responseModel.Status = "OK";
                responseModel.Data = $"Field '{model.Field}' updated by value '{model.Value}'";
            }
            catch(Exception ex) 
            {
                responseModel.Status = "Error";
                responseModel.Data = ex.Message;
            }
            return Json(responseModel);
            /* Метод для оновлення даних про користувача
             * Приймає асинхронні запити з JSON даними
             * Повертає JSON із результатом роботи
             * Треба описати модель даних, що приймає метод
             * Треба описати модель даних, що він повертає
             */

        }
        [HttpPost]
        public JsonResult ConfirmEmail([FromBody] string emailCode)
        {
            StatusDataModel model = new();
            if(string.IsNullOrEmpty(emailCode))
            {
                model.Status = "406";
                model.Data   = "Empty code not acceptable";
            }
            else if(HttpContext.User.Identity?.IsAuthenticated == false)
            {
                model.Status = "401";
                model.Data = "Unauthenticated";
            }
            else
            {
                User? user = _dataContext.Users.Find(
                    Guid.Parse(
                        HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value
                ));
                if(user is null)
                {
                    model.Status = "403";
                    model.Data = "Forbidden (UnAuthorized)";
                }
                else if(user.EmailCode is null)
                {
                    model.Status = "208";
                    model.Data = "Already confirmed";
                }
                else if(user.EmailCode != emailCode)
                {
                    model.Status = "406";
                    model.Data = "Code not Accepted";
                }
                else
                {
                    user.EmailCode = null;
                    _dataContext.SaveChanges();
                    model.Status = "200";
                    model.Data = "OK";
                }
            }
            return Json(model);
        }
        [HttpGet]
        public ViewResult ConfirmToken([FromQuery] string token)
        {
            try
            {
                var confirmToken = _dataContext.EmailConfirmTokens.Find(Guid.Parse(token)) ?? throw new Exception();
                var user = _dataContext.Users.Find(confirmToken.UserId) ?? throw new Exception();
                // перевіряємо збіг поштових адрес
                if(user.Email != confirmToken.UserEmail) throw new Exception();
                // Оновлюємо дані
                user.EmailCode = null;  // пошта підтверджена
                confirmToken.Used += 1; // ведемо підрахунок використання токена
                _dataContext.SaveChangesAsync();
                ViewData["result"] = "Вітаємо, пошта успішно підтверджена";
            }
            catch
            {
                ViewData["result"] = "Перевірка не пройдена, не змінюйте посилання з листа!";
            }
            return View();
        }
        [HttpPatch]
        public string ResendConfirmEmail()
        {
            if(HttpContext.User.Identity?.IsAuthenticated == false)
            {
                return "Unauthenticated";
            }
            try
            {
                User? user = _dataContext.Users.Find(
                    Guid.Parse(
                        HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value)) ?? throw new Exception();

                user.EmailCode = _randomService.ConfirmCode(6);

                var emailConfirmToken = _GenerateEmailConfirmToken(user);

                _dataContext.SaveChangesAsync();

                if (_SendConfirmEmail(user, emailConfirmToken)) return "OK";
                else return "Send error";
            }
            catch
            {
                return "Unauthorized";
            }
        }
        private EmailConfirmToken _GenerateEmailConfirmToken(User user)
        {
            Data.Entity.EmailConfirmToken emailConfirmToken = new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                UserEmail = user.Email,
                Moment = DateTime.Now,
                Used = 0
            };
            _dataContext.EmailConfirmTokens.Add(emailConfirmToken);
            return emailConfirmToken;
        }
        private bool _SendConfirmEmail(User user, EmailConfirmToken token)
        {
            // Формуємо посилання: схема://домен/User/ConfirmToken?token=...
            string confirmLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/User/ConfirmToken?token={token.Id}";

            return _emailService.Send(
                "confirm_email",
                new Models.Email.ConfirmEmailModel
                {
                    Email = user.Email,
                    RealName = user.RealName,
                    EmailCode = user.EmailCode!,
                    ConfirmLink = confirmLink
                });
        }
    }
}
