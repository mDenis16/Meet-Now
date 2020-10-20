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
        public async Task connectedUser(string auth)
        {

            var index = cache.user.list.FindIndex(a => a.auth == auth);
            if (index != -1)
            {


               

                if (cache.user.list[index].username != null)
                {

                    cache.user.list[index].connectionId = Context.ConnectionId;

                    Console.WriteLine("conected user " + cache.user.list[index].username + "connection id " + cache.user.list[index].connectionId);
                }
                else
                    Console.WriteLine("User is'nt connected.");
            }

        }
    }

}