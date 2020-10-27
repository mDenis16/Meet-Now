using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace app.cache
{
    public static class room {
        public static List<roomData> rooms = new List<roomData>();
        public static class rpc_types
        {
            public const string
            ON_USER_JOIN = "ON_USER_JOIN",
            ON_USER_LEAVE = "ON_USER_LEAVE";
        }
        
        public class roomData
        {
            public string uuid { get; set; }
            public string uuid_creator { get; set; }
            public int participants_limit { get; set; }

            public List<participant> participants { get; set; }

         
            public  async Task   disconnectParticipant( cache.room.participant participant )
            {

                participant.online = false;
                await participant.getUserModel( );
                this.participants.ForEach( async ( participant part ) => { await part.send( rpc_types.ON_USER_LEAVE, Newtonsoft.Json.JsonConvert.SerializeObject( new { uuid = participant.uuid, username = participant.user_data.username } ) ); } );

            }
            public void addParticipant( string uuid )
            {
                var index = this.participants.FindIndex( participant => participant.uuid == uuid );
                if ( index != -1 )
                    return;

                var new_participant = new participant( uuid );

              
                this.participants.Add( new_participant );
            }
            public async Task  onParticipantConnect( string uuid, string connection_id, string peer_id )
            {
                Console.WriteLine( $"CONNECTED USER WITH UID {uuid} and peer_id {peer_id}" );
                var index = this.participants.FindIndex( participant => participant.uuid == uuid );
                if ( index == -1 )
                    return;

                this.participants[ index ].connection_id = connection_id;
                this.participants[ index ].peer_id = peer_id;
                this.participants[ index ].online = true;
                this.participants.ForEach( async ( participant participant ) => { await participant.send( rpc_types.ON_USER_JOIN, Newtonsoft.Json.JsonConvert.SerializeObject( new { uuid = this.participants[ index ].uuid, username = this.participants[ index ].user_data.username } ) ); } );
            }
            public async Task fetchParticipants( )
            {
               this.participants.ForEach( async participant => {
                  if ( participant.user_data == null )
                      await participant.getUserModel( );
              } );
            }
            public roomData( )
            {
                this.participants = new List<participant>( );
            }
            public roomData( int participants_limit, string uuid_creator)
            {
                this.participants_limit = participants_limit;
                this.uuid_creator = uuid_creator;
                this.participants = new List<participant>( );
                this.uuid = Guid.NewGuid( ).ToString( );
            }
        }
        public class participant
        {
            
            public bool owner { get; set; }
            public string connection_id { get; set; }
            public string uuid { get; set; }
            public string peer_id { get; set; }

            public bool online { get; set; }
            public async  Task getUserModel( )
            {
              this.user_data = await user.fetchUserData( "", this.uuid );
            }
            public user.model user_data { get; set; }
            public async Task send( string event_name, string data )
            {
                if ( this.connection_id != null)
                   await ChatHub.Current.Clients.Client( this.connection_id ).SendAsync( event_name, data );
            }
            public participant(string uuid )
            {
                this.uuid = uuid;
            }
        }
        public static async Task<string> createRoom( string uuid_creator, int partcipants_limit  = 30 )
        {
          

            roomData nonCached = new roomData(partcipants_limit, uuid_creator);
            Console.WriteLine( "nonCached.uuid_creator" + nonCached.uuid_creator );
            await databaseManager.updateQuery( "INSERT INTO rooms (uuid, uuid_creator, participants_limit) VALUES(@uuid, @uuid_creator, @participants_limit)" )
                .addValue("@uuid", nonCached.uuid)
                .addValue( "@uuid_creator", nonCached.uuid_creator )
                .addValue("@participants_limit", nonCached.participants_limit)
                .Execute( );

            rooms.Add( nonCached );
            return nonCached.uuid;
        }
   
        public static async Task<roomData> fetchRoomData( string uuid )
        {
            var roomIndex = rooms.FindIndex(room => room.uuid == uuid);
            if ( roomIndex != -1 )
                return rooms[ roomIndex ];

            roomData nonCached = new roomData();

            await databaseManager.selectQuery( "SELECT uuid, uuid_creator, participants_limit FROM rooms WHERE uuid = @uuid LIMIT 1", delegate ( DbDataReader reader ) {
                if ( reader.HasRows )
                {
                    nonCached.participants_limit = ( int ) reader[ "participants_limit" ];
                    nonCached.uuid = ( string ) reader[ "uuid" ];
                    nonCached.uuid_creator = ( string ) reader[ "uuid_creator" ];

                }
            } ).addValue( "@uuid", uuid ).Execute( );

            rooms.Add( nonCached );
            return nonCached;
        }
    }
}
