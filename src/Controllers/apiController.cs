using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;


namespace app.Controllers
{

    public class api : Controller
    {



        [HttpGet]
        public async Task<JsonResult> fetchUserData()
        {
            if (Request.Cookies["auth"] == null)
                return Json(new { success = false, message = "You are not logged in." });

            var fetchUser = await cache.user.fetchUserData((string)Request.Cookies["auth"]);
            if (fetchUser.username == null)
                return Json(new { success = false, message = "You are not logged in." });

            return Json(new { success = true, user = new { username = fetchUser.username } });
        }


    }
}
