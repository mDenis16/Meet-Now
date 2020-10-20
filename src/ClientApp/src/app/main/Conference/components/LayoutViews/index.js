import React, { Component } from 'react';
import { Video } from '../Video/';
import * as signalR from '@aspnet/signalr';
import './style/index.css'
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

        var participants = [{ name: 'Denis', stream: {}, ref: {} }];

        var constraints = { audio: true, video: { width: 1280, height: 720 } };


        this.setState({ participants: participants });

        navigator.mediaDevices.getUserMedia(constraints)
            .then(function (mediaStream) {

                participants[0].ref.current.setObjStream(mediaStream);

            })
            .catch(function (err) { console.log(err.name + ": " + err.message); });
        participants.forEach(participant => {
            participant.ref = React.createRef();
        });

        var connection = new signalR.HubConnectionBuilder()
            .withUrl('/ws')
            .build();




        connection.start()
            .then(function () {
                console.log('connection started');

                connection.invoke('connectedUser', readCookie('auth'));
                connection.on('message', (message) => {
                    //   addChannelMessage(message);

                });
                connection.on('voice', (arrayBuffer) => {


                });
            })
            .catch(error => {
                console.error(error.message);
            });
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
