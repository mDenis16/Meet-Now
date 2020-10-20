using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace app.cache {

    public static class user {
        public enum rpc
        {
            message
        }
        public class model {
            public int id { get; set; }
            public string uuid { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string auth { get; set; }
            public string email { get; set; }
            public string lastChannel { get;set; }
            public string connectionId { get; set; }
            public string avatar { get; set; }
          

        }

       
        public static List<model> list = new List<model> ();

        public static async Task<int> requestUserLogin (string email, string password) {

            int id = -1;
            await databaseManager.selectQuery ("SELECT username, id FROM users WHERE email = @email AND password = @password LIMIT 1", delegate (DbDataReader reader) {
               if (reader.HasRows)
                {
                    id = (int)reader["id"];
                }
            }).addValue ("@email", email).addValue ("@password", password).Execute ();

            return id;
        }
        public static async Task<model> onUserLogin(int id, string auth)
        {
            model user = new model();
            user.auth = auth;
            await databaseManager.updateQuery("UPDATE users SET auth = @auth WHERE id = @id ").addValue("@id", id).addValue("@auth",auth).Execute();
            await loadUserData(id);
            return user;
        }
        public static async Task<model> loadUserData(int id, string auth = "")
        {
            model user = new model();
            await databaseManager.selectQuery("SELECT * FROM users WHERE id = @id OR auth = @auth LIMIT 1", delegate (DbDataReader reader) {
                user.id = (int)reader["id"];
                user.username = (string)reader["username"];
                user.password = (string)reader["password"];
                user.uuid = (string)reader["uuid"];
                user.auth = (string)reader["auth"];
                user.avatar = (string)reader["avatar"];
                user.lastChannel = (string)reader["lastChannel"];
            }).addValue("@id", id).addValue("@auth", auth).Execute();

            var index = list.FindIndex(a => a.id == user.id);
            if (index != -1)
                list[index] = user;
            else
                list.Add(user);
            return user;
        }
        public static async Task<model> fetchUserData(string auth)
        {
            var index = list.FindIndex(a => a.auth == auth);
    
            return index != -1 ? list[index]  : await loadUserData(-1, auth);
           
        }
    }
}