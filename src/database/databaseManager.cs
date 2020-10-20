using System;
using System.Data.Common;
using System.Threading.Tasks;
using MySqlConnector;


namespace app {
    class databaseManager : Singleton<databaseManager> {
        public databaseManager( ) { }
        public string password { get; set; }

        private MySqlConnection connection = null;
        public MySqlConnection Connection {
            get { return connection; }
        }

        public static MySqlCommand SimpleQuery( String query ) {
            databaseManager.Instance( ).connection = databaseManager.Instance( ).newConnection( );
            return new MySqlCommand( query, databaseManager.Instance( ).connection );
        }

        public static async Task<bool> isTableEmpty( String tableName ) {
            bool isEmpty = true;
            await databaseManager.selectQuery( "SELECT * FROM" + tableName, ( DbDataReader reader ) => {
                isEmpty = !reader.HasRows;
            } ).Execute( );
            return isEmpty;
        }

        public static  selectQuery selectQuery( String query, Action<DbDataReader> code ) {
            return new selectQuery( query, code );
        }

        public static  query updateQuery( String query ) {
            return new normalQuery( query );
        }

        private String loadConnection( ) {
            string host = "localhost";
            string database = "app";
            string user = "root";
            string password = "";
            return String.Format( "Server={0};database={1}; UID={2}; password={3}", host, database, user, password );
        }

        public MySqlConnection newConnection( ) {
            var connection = new MySqlConnection( loadConnection( ) );
            connection.Open( );

            return connection;
        }

        public void closeConnection( ) {
            connection.Close( );
        }
    }
}
