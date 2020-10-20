using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace app.cache {

    public static class channel {
        public class participant
        {
            public string uuid { get; set; }
            public string username { get; set; }
            public string avatar { get; set; }


        }
        public class model {
            public string uuid { get; set; }
            public int type { get; set; }
            public string title { get; set; }
            public string avatar { get; set; }
        
            
        }
        public class channelData
        {
            public model channel { get; set; }
            public List<participant> participants {get;set; }

        }
        public static List<model> list = new List<model> ();

        public static async Task<List<model>> loadUserChannels(string uuid)
        {
            List<model> channelList = new List<model>();
            await databaseManager.selectQuery("SELECT * FROM participants, channels WHERE participants.channel = channels.uuid AND participants.user = @uuid", delegate (DbDataReader reader) {
                model channel = new model();
                channel.uuid = (string)reader["uuid"];
                channel.title = (string)reader["title"];
                channel.type = (int)reader["type"];
                channel.avatar = (string)reader["avatar"];
                channelList.Add(channel);
            }).addValue("@uuid", uuid).Execute();


            return channelList;
        }

        public static async Task<List<model>> fetchUserChannels(string uuid)
        {
            return await loadUserChannels(uuid);


        }
    }
}