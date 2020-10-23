import React, { Component } from 'react';
import { WaitingPage } from './components/Waiting/'
import { LayoutViews } from './components/LayoutViews/'

import * as signalR from '@aspnet/signalr';
import './style/index.css'
import { Socket } from 'react-socket-io';
import adapter from 'webrtc-adapter';
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

export class ConferenceLayout extends Component {

    constructor() {
        super();

        this.state = { joined: false }; 
 
    }
    componentDidMount() {

        //  if (!this.props.logged)
        // this.props.history.push('/login');

    }
  

    render() {
        const { roomId } = this.props.match.params;

        if (this.props.joined)
            return (<LayoutViews userData={this.props.userData} roomId={roomId} />);
        else
            return (<WaitingPage  requestJoin={this.props.requestJoin.bind(this)} userData={this.props.userData} roomId={roomId} />);

    }
}
