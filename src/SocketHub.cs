﻿using Microsoft.AspNetCore.Mvc.Rendering;
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
            foreach ( var room in app.cache.room.rooms )
                foreach ( var participant in room.participants )
                    if ( participant.connection_id == Context.ConnectionId )
                    {
                        Console.WriteLine( $"Disconnected participant with username {participant.user_data.username} and peer_id {participant.peer_id}" );
                        await room.disconnectParticipant( participant );
                        break;
                    }

         
            
            await base.OnDisconnectedAsync(exception);
        }
        public async Task voice(object buffer)
        {
          await  Clients.All.SendAsync("voice", buffer);

        }
        public async Task connectedUser(string auth, string room_id, string peer_id)
        {
            Console.WriteLine( "new socket connection" );
            var userData = await cache.user.fetchUserData( auth );
            var index = cache.user.list.FindIndex(a => a.auth == auth);
            var roomIndex = cache.room.rooms.FindIndex(a => a.uuid == room_id);
            if ( userData.username != null && roomIndex != -1 )
            {
                await cache.room.rooms[ roomIndex ].onParticipantConnect( userData.uuid, Context.ConnectionId, peer_id );
            }
        }
    }

}