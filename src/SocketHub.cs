using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace app
{
    public class ChatHub : Hub
    {
        public static IHubContext<ChatHub> Current { get; set; }
      
       

        public override async Task OnDisconnectedAsync(Exception exception)
        {
           
            await base.OnDisconnectedAsync(exception);
        }
        public async Task voice(object buffer)
        {
          await  Clients.All.SendAsync("voice", buffer);

        }
        public async Task connectedUser(string auth, string room_id, string peer_id)
        {
            var userData = await cache.user.fetchUserData( auth );
            var index = cache.user.list.FindIndex(a => a.auth == auth);
            var roomIndex = cache.room.rooms.FindIndex(a => a.uuid == room_id);
            if ( userData.username != null && roomIndex != -1)
                cache.room.rooms[ roomIndex ].onParticipantConnect( userData.uuid, Context.ConnectionId, peer_id );

        }
    }

}