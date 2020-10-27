import React, { Component } from 'react';
import { Video } from '../Video/';
import * as signalR from '@aspnet/signalr';
import './style/index.css'
import Peer from 'peerjs'
import axios from 'axios';
import { NotificationContainer, NotificationManager } from 'react-notifications';
function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}


export class LayoutViews extends Component {

    constructor() {
        super();

        this.state = {
            participants: []
        }

    }
    addParticipant() {

    }
componentDidUpdate(){
    alert('updated');
}
    componentDidMount() {
        var constraints = { audio: true, video: { width: 1280, height: 720 } };
        const peer = new Peer({
            host: 'localhost',
            port: 5001,
            secure: false,
            path: '/myapp'
        });
        const auth = readCookie('auth');
        const { roomId } = this.props;
        var connection = new signalR.HubConnectionBuilder()
            .withUrl('/ws')
            .build();
        var parent = this;

        peer.on('open', function (id) {


            axios.get('room/fetchParticipants/' + roomId + "/" + id).then((response) => {
                navigator.mediaDevices.getUserMedia({ video: true, audio: true }).then((localStream) => {
                    console.log(response.data.roomData.participants)
                    var myIndex = response.data.roomData.participants.findIndex(participant => participant.uuid == parent.props.userData.uuid);
                    response.data.roomData.participants[myIndex].online = true;

                    response.data.roomData.participants.forEach(part => part.ref = React.createRef());
                    parent.setState({ participants: response.data.roomData.participants }, () => {
                        parent.state.participants.forEach((part) => {
                            if (part.peer_id == id) {
                                setTimeout(() => {
                                    part.ref.current.setObjStream(localStream);
                                }, (1000));
                            } else {
                                console.log('calling ' + part.peer_id);
                                var call = peer.call(part.peer_id, localStream);
                                call.on('stream', function (remoteStream) {
                                    console.log(part.uuid + 'responsed to your call.');
                                    setTimeout(() => {
                                        part.ref.current.setObjStream(remoteStream);
                                    }, (1000));
                                });
                                call.on('close', function () {
                                    alert("The videocall has finished");
                                });
                            }

                        });

                    });
                    peer.on('call', function (call) {

                        axios.get('room/fetchParticipant/' + roomId + "/" + call.peer).then((response) => {
                            console.log(response);
                            call.answer(localStream); // Answer the call with an A/V stream.
                            call.on('stream', function (remoteStream) {
                                var participant = parent.state.participants.find(part => part.uuid == response.data.participant.uuid);
                                if (!participant) {
                                    console.log(response.data.participant);
                                    response.data.participant.ref = React.createRef();
                                    parent.state.participants.push(response.data.participant);
                                    /*parent.setState({ participants: parent.state.participants },
                                        setTimeout(() => {
                                            response.data.part.ref.current.setObjStream(remoteStream);
                                        }, 2000));*/
                                      
                                      //  setTimeout(() => {
                                            response.data.participant.ref.current.setObjStream(remoteStream);
                                    //    }, 1000)

                                } else {
                                    
                                       // participant.ref = React.createRef();
                                     
                                  //    participant.ref.current.setObjStream(remoteStream);
                                     
                                }
                                parent.setState({ participants: parent.state.participants });
                            });
                            call.on('close', function () {
                                alert("The videocall has finished");
                            });

                        });
                    });
                });


            });


            connection.start()
                .then(function () {
                    console.log('connection started');

                    connection.invoke('connectedUser', auth, roomId, id);

                    connection.on('ON_USER_JOIN', (user) => {
                        user = JSON.parse(user);
                        var userView = parent.state.participants.find(part => part.uuid == user.uuid);
                        if (userView)
                            userView.online = true;

                        NotificationManager.info(user.username + ' s-a conectat.');
                    });

                    connection.on('ON_USER_LEAVE', (user) => {
                        user = JSON.parse(user);
                        //alert(JSON.stringify(user))
                        var userView = parent.state.participants.find(part => part.uuid == user.uuid);
                        if (userView)
                            userView.online = false;

                        NotificationManager.info(user.username + ' s-a deconectat.');
                    });

                })
                .catch(error => {
                    console.error(error.message);
                });
        });



    }
    render() {


        return (

            <div className="layout">

                <div className="views">
                    <div className="grid">

                        {this.state.participants.filter(participant => participant.online).map((participant) => (
                            <Video ref={participant.ref} show={true} order={(participant.uuid == this.props.userData.uuid) ? 0 : this.state.participants.indexOf(participant)} name={participant.user_data.username} />
                        ))}
                    </div>
                    <div className="control"></div>

                </div>
                <div className="details">sd</div>
            </div>
        );
    }
}
