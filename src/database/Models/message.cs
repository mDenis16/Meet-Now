using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace app.cache {

    public static class message {
        public class model {
            public int id { get; set; }
            public string uuid { get; set; }
            public string senderuuid { get; set; }
            public string sender { get; set; }
            public string channel { get; set; }
            public DateTime time { get; set; }
            public  string content { get; set; }
            public string avatar { get; set; }

        }
        public static List<model> list = new List<model> ();

   
        public static async Task<List<model>> fetchChannelData(string channel)
        {
            List<model> messageList = new List<model>();
            await databaseManager.selectQuery("SELECT * FROM messages, users WHERE messages.channel = @channel AND messages.sender = users.uuid", delegate (DbDataReader reader) {
                model message = new model();
                message.id = (int)reader["id"];
                message.uuid = (string)reader["uuid"];
                message.senderuuid = (string)reader["sender"];
                message.sender = (string)reader["username"];
                message.channel = (string)reader["channel"];
                message.content = (string)reader["content"];
                message.time = (DateTime)reader["time"];
                message.avatar = (string)reader["avatar"];
                messageList.Add(message);
            }).addValue("@channel", channel).Execute();

          
            return messageList;
        }
       
    }
}