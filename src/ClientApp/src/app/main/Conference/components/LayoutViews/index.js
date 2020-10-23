import React, { Component } from 'react';
import { Video } from '../Video/';
import * as signalR from '@aspnet/signalr';
import './style/index.css'
import Peer from 'peerjs'
import axios from 'axios';

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

    componentDidMount() {
        var constraints = { audio: true, video: { width: 1280, height: 720 } };
        const peer = new Peer(this.props.userData.uuid, {
            host: 'localhost',
            port: 5001,
            secure: false,
            path: '/myapp'
        });






        navigator.mediaDevices.getUserMedia(constraints)
            .then(function (mediaStream) {
                var connection = new signalR.HubConnectionBuilder()
                    .withUrl('/ws')
                    .build();

                peer.on('open', function (id) {
                   console.log(id);
                    axios.get('room/fetchRoomData/' + this.props.roomId)
                        .then((response) => {
                            console.log(response.data);
                            if (response.data.success) {
                                response.data.roomData.participants.forEach((participant) => participant.ref = React.createRef());
                                this.setState({ participants: response.data.roomData.participants });

                            }
                        }, (error) => {
                            console.log(error);
                        });

                    connection.start()
                        .then(function () {
                            console.log('connection started');

                            connection.invoke('connectedUser', readCookie('auth'), this.props.roomId, id);
                            connection.on('ON_USER_JOIN', (uuid, connection_id) => {
                                console.log('JOINED USER ');

                            });

                        })
                        .catch(error => {
                            console.error(error.message);
                        });
                });

                //response.data.participants[0].ref.current.setObjStream(mediaStream);

            })
            .catch(function (err) { console.log(err.name + ": " + err.message); });



    }
    render() {


        return (

            <div className="layout">

                <div className="views">
                    <div className="grid">
                        {this.state.participants.map((participant) => (
                            <Video ref={participant.ref} />
                        ))}
                    </div>
                    <div className="control"></div>

                </div>
                <div className="details">sd</div>
            </div>
        );
    }
}
