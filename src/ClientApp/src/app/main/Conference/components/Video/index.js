import React, { Component } from 'react';
import Peer from 'peerjs';

export class Video extends Component {

    constructor() {
        super();


        this.state = {obj : {}};
        this.videoTag = React.createRef();
        this.setObjStream = this.setObjStream.bind(this);
    }
    setObjStream(stream){
        this.videoTag.current.srcObject = stream;
        console.log(stream);
    }

    componentDidMount() {


    }
    render() {


        return (
            <video autoPlay muted className="view"  ref={this.videoTag} ></video>
        );
    }
}
