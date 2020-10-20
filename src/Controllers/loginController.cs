using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;


namespace app.Controllers
{

   
    public class auth : Controller
    {
        public class LoginJson
        {
            public string email { get; set; }
            public string password { get; set; }
        }
        [HttpPost]
        public async Task<JsonResult>  login([FromBody] LoginJson data)
        {
            int id = await cache.user.requestUserLogin(data.email, data.password);
            if (id != -1)
            {
                CookieOptions option = new CookieOptions();



                option.Expires = new DateTimeOffset?(DateTime.Now.AddDays(5));
                var auth = Guid.NewGuid().ToString();
                Response.Cookies.Append("auth", auth, option);
                await cache.user.onUserLogin(id, auth);
                return Json(new { success = true, auth = auth });
            }
            return Json(new { success = false, message = "Wrong password." });
        }
      
    }
}
