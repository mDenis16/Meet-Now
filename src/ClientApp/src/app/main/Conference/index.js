import React, { Component } from 'react';
import { LayoutViews } from './components/LayoutViews/'
import { Layout } from '../Layout/'
import * as signalR from '@aspnet/signalr';
import './style/index.css'
import { Socket } from 'react-socket-io';
import adapter from 'webrtc-adapter';

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

export class Conference extends Component {

    constructor() {
        super();

        this.state = { connection: {}, joined: false };

    }
    componentDidMount() {
        if (!this.props.logged)
            this.props.history.push('/login');


    }


    render() {
        const { roomId } = this.props.match.params;

        if (!this.state.joined)
            return (<Layout />);
        return (
            <LayoutViews></LayoutViews>
        );
    }
}
