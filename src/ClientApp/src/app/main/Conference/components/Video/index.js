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
       
    }

    componentDidMount() {

console.log('rendered  for ' + this.props.name )
    }
    render() {


        return (
            <div className="view" style={{order: this.props.order}}>
                <div className="name">{this.props.name}</div>
                <video autoPlay muted  ref={this.videoTag}/>
            </div>
           
        );
    }
}
