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

            public void removeParticipant( string uuid )
            {
                var index = this.participants.FindIndex( participant => participant.uuid == uuid );
                if ( index == -1 )
                    return;

                this.participants.ForEach( ( participant participant ) => { participant.send( rpc_types.ON_USER_LEAVE, uuid ); } );
            }
            public void addParticipant( string uuid )
            {
                var index = this.participants.FindIndex( participant => participant.uuid == uuid );
                if ( index == -1 )
                    return;

                this.participants.ForEach( ( participant participant ) => { participant.send( rpc_types.ON_USER_JOIN, uuid ); } );
            }
            public roomData( )
            {
                this.participants = new List<participant>( );
            }
            public roomData( int participants_limit, string uuid_creator)
            {
                this.participants_limit = participants_limit;
                this.participants = new List<participant>( );
                this.uuid = Guid.NewGuid( ).ToString( );
            }
        }
        public class participant
        {
            public string uuid { get; set; }
            public bool owner { get; set; }
            public string name { get; set; }
            public string connection_id { get; set; }
            public async void send( string event_name, string data )
            {
                await ChatHub.Current.Clients.Client( this.connection_id ).SendAsync( event_name, data );
            }
        }
        public static async Task<string> createRoom( string uuid_creator, int partcipants_limit  = 30 )
        {
          

            roomData nonCached = new roomData(partcipants_limit, uuid_creator);

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

            await databaseManager.selectQuery( "SELECT uuid, uuid_creator, participants_limit FROM meets WHERE uuid = @uuid LIMIT 1", delegate ( DbDataReader reader ) {
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
