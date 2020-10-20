using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Data.Common;
using Microsoft.AspNetCore.SignalR;

namespace app.Controllers
{

    public class roomController : Controller
    {
       
        [HttpGet("room/fetchRoomData/{uuid}")]
        public async Task<JsonResult> fetchRoomData(string uuid)
        {
            if (Request.Cookies["auth"] == null)
                return Json(new { success = false, message = "You are not logged in." });

            var fetchUser = await cache.user.fetchUserData((string)Request.Cookies["auth"]);
            if (fetchUser.username == null)
                return Json(new { success = false, message = "You are not logged in." });

        
            return Json(new { success = true, roomData = await cache.room.fetchRoomData(uuid) });
        }
        [HttpGet( "room/createRoom" )]
        public async Task<JsonResult> createRoom( )
        {
            if ( Request.Cookies[ "auth" ] == null )
                return Json( new { success = false, message = "You are not logged in." } );

            var fetchUser = await cache.user.fetchUserData((string)Request.Cookies["auth"]);
            if ( fetchUser.username == null )
                return Json( new { success = false, message = "You are not logged in." } );

            string generated_uuid = await app.cache.room.createRoom(fetchUser.uuid);
            string generated_link = "https://localhost:5001/room/" + generated_uuid;

            return Json( new { success = true, link = generated_link } );
        }
        public class messageJson
        {
            public string content { get; set; }

        }
        [HttpPost]
        public async Task<JsonResult> sendMessage([FromBody] messageJson data)
        {
            if (Request.Cookies["auth"] == null)
                return Json(new { success = false, message = "You are not logged in." });

            var fetchUser = await cache.user.fetchUserData((string)Request.Cookies["auth"]);
            if (fetchUser.username == null)
                return Json(new { success = false, message = "You are not logged in." });
            
            if (fetchUser.lastChannel.Length == 0)
                return Json(new { success = false, message = "You are not in a channel." });


            cache.message.model message = new cache.message.model();
            message.channel = fetchUser.lastChannel;
            message.content = data.content;
            message.time = DateTime.Now;
            message.senderuuid = fetchUser.uuid;
            message.sender = fetchUser.username;
            message.uuid = Guid.NewGuid().ToString();
            message.id = await databaseManager.updateQuery("INSERT INTO messages (uuid, sender, channel, content) VALUES (@uuid, @sender, @channel, @content)")
                .addValue("@uuid", message.uuid)
                .addValue("@sender", message.senderuuid)
                .addValue("@channel", message.channel)
                .addValue("@content", message.content)
                .Execute();

          
            return Json(new { success = true, message = message });
        }

        

    }
}
