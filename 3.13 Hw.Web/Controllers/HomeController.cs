using _3._13_Hw.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _3._13_Hw.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=AdsDb; Integrated Security=true;";
        public IActionResult Index(int userId)
        {
            var mgr = new AdManager(_connectionString);
            AdViewModel vm = new();
            vm.Ads = mgr.GetAds();
            if (User.Identity.IsAuthenticated)
            {
                vm.UserId = mgr.GetByEmail(User.Identity.Name).Id;
            }

            return View(vm);
           
               
        }

        [Authorize]
        public IActionResult NewAd()
        {

            return View();

        }

        [HttpPost]
        public IActionResult NewAd(Ads ad)
        {
            var mgr = new AdManager(_connectionString);
            var user = mgr.GetByEmail(User.Identity.Name);
            mgr.AddAd(ad, user.Id);
            return Redirect("/home/index");

        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var mgr = new AdManager(_connectionString);
            mgr.Delete(id);
            return Redirect("/home/index");
        }
        public IActionResult MyAccount()
        {
            var mgr = new AdManager(_connectionString);
            var userId = mgr.GetByEmail(User.Identity.Name).Id;
            return View(new AdViewModel
            {
                Ads = mgr.Account(userId)

            });

        }
    }
}