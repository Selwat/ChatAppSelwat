using Microsoft.AspNetCore.Mvc;
using ChatApp.Models;
using System.Diagnostics;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChatContext _context;
        private const string UserKey = "USER_KEY";

        public HomeController(ILogger<HomeController> logger, ChatContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var userName = HttpContext.Session.GetString(UserKey);

            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogInformation("User is not signed in, redirecting to SignIn.");
                return RedirectToAction("SignIn");
            }

            try
            {
                var messages = _context.ChatMessages.OrderByDescending(m => m.CreatedOn).Take(50).ToList();

                if (messages == null || !messages.Any())
                {
                    _logger.LogWarning("No messages found in the database.");
                }

                var vm = new IndexVm
                {
                    UserName = userName,
                    Messages = messages
                };

                _logger.LogInformation($"Loaded {messages.Count} messages for user {userName}.");

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading messages.");
                return View(new IndexVm { UserName = userName, Messages = new List<ChatMessage>() });
            }
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(SignInVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            SignInUser(vm.UserName);
            return RedirectToAction("Index");
        }

        private void SignInUser(string vmUserName)
        {
            HttpContext.Session.SetString(key: UserKey, value: vmUserName);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Dodaj metodę TestSession tutaj
        public IActionResult TestSession()
        {
            // Ustaw wartość sesji
            HttpContext.Session.SetString("TestKey", "TestValue");
            // Pobierz wartość sesji
            var value = HttpContext.Session.GetString("TestKey");
            // Sprawdź, czy wartość sesji jest prawidłowa
            if (value == "TestValue")
            {
                return Content("Sesja działa poprawnie.");
            }
            else
            {
                return Content("Sesja nie działa.");
            }
        }
    }
}