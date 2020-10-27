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
                return Json(new { success = false, error_code = 1, message = "You are not logged in." });

            var fetchUser = await cache.user.fetchUserData((string)Request.Cookies["auth"]);
            if (fetchUser.username == null)
                return Json(new { success = false, error_code = 1, message = "You are not logged in." });

            var _roomData = await cache.room.fetchRoomData(uuid);
            if (_roomData.uuid == null)
                return Json( new { success = false, error_code = 1, message = "This room doesnt exist." } );


            return Json(new { success = true,  roomData = _roomData } );
        }
        [HttpGet( "room/fetchParticipants/{uuid}/{peer_uuid}" )]
        public async Task<JsonResult> fetchParticipants( string uuid, string peer_uuid )
        {
            if ( Request.Cookies[ "auth" ] == null )
                return Json( new { success = false, error_code = 1, message = "You are not logged in." } );

            var fetchUser = await cache.user.fetchUserData((string)Request.Cookies["auth"]);
            if ( fetchUser.username == null )
                return Json( new { success = false, error_code = 1, message = "You are not logged in." } );

            var _roomData = await cache.room.fetchRoomData(uuid);
            if ( _roomData.uuid == null )
                return Json( new { success = false, error_code = 1, message = "This room doesnt exist." } );
            _roomData.addParticipant( fetchUser.uuid );
            await _roomData.fetchParticipants( );
            Console.WriteLine( $"User {fetchUser.username} is fetching participants with peer id {peer_uuid}" );
            _roomData.participants.Find( participant => participant.uuid == fetchUser.uuid ).peer_id = peer_uuid;
            //this.participants[ index ].connection_id = connection_id;
            //this.participants[ index ].peer_id = peer_id;
            return Json( new { success = true, roomData = _roomData } );
        }
        
        [ HttpGet( "room/requestJoin/{uuid}" )]
        public async Task<JsonResult> requestJoinRoom( string uuid )
        {
            if ( Request.Cookies[ "auth" ] == null )
                return Json( new { success = false, error_code = 1, message = "You are not logged in." } );

            var fetchUser = await cache.user.fetchUserData((string)Request.Cookies["auth"]);
            if ( fetchUser.username == null )
                return Json( new { success = false, error_code = 1, message = "You are not logged in." } );

            var _roomData = await cache.room.fetchRoomData(uuid);
            if ( _roomData.uuid == null )
                return Json( new { success = false, error_code = 1, message = "This room doesnt exist." } );

           
      
            return Json( new { success = true, roomData = _roomData } );
        }
        [HttpGet( "room/fetchParticipant/{room_id}/{peer_id}" )]
        public async Task<JsonResult> fetchParticipant( string room_id, string peer_id )
        {
            if ( Request.Cookies[ "auth" ] == null )
                return Json( new { success = false, error_code = 1, message = "You are not logged in." } );

            var fetchUser = await cache.user.fetchUserData((string)Request.Cookies["auth"]);
            if ( fetchUser.username == null )
                return Json( new { success = false, error_code = 1, message = "You are not logged in." } );

            var _roomData = await cache.room.fetchRoomData(room_id);
            if ( _roomData.uuid == null )
                return Json( new { success = false, error_code = 1, message = "This room doesnt exist." } );

            
            return Json( new { success = true, participant = _roomData.participants.Find(participant => participant.peer_id == peer_id)  } );
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
      

            cache.message.model message = new cache.message.model();
           
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
