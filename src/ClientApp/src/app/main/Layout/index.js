import React, { Component } from 'react';
import { Video } from '../Conference/components/Video'
import './style/index.css'
import { withRouter } from 'react-router-dom';
export class Layout extends Component {

    constructor(props) {
        super(props);
      
        this.state = {
            toggleMute: false
        };
        console.log(props)
        this.toggleMute = this.toggleMute.bind(this);
        this.copyLink = this.copyLink.bind(this);
        this.linkRef = React.createRef();
    }
    toggleMute() {
        this.setState({ toggleMute: !this.state.toggleMute });
    }
    copyLink(e) {

        navigator.clipboard.writeText(this.linkRef.current.innerHTML)
        e.target.focus();
    }
    componentDidMount() {
       

    }


    render() {

        var constraints = { audio: this.state.toggleMute, video: { width: 1280, height: 720 } };
        navigator.mediaDevices.getUserMedia(constraints)
            .then(function (mediaStream) {


                document.getElementById('video-preview').srcObject = mediaStream;
            })
            .catch(function (err) { console.log(err.name + ": " + err.message); });
        return (
            <>
                <div className="header">

                    <div className="title">Meet Now</div>
                    <div className="name">{this.props.user}</div>
                </div>

                <div className="content">
                    <div className="video-preview">
                        <video autoPlay className="video" id="video-preview" />

                    </div>


                    <div className="actions">
                        <div className="text">Întâlnirea este pregătită</div>
                        <div className="text-small" ref={this.linkRef} onClick={this.copyLink}>meet.google.com/xpy-yqss-gsm</div>
                        <div className="button-group">

                            <div className="button">Meet Now</div>
                            <div className="button">Present</div>
                        </div>
                    </div>
                </div >
            </>
        );
    }
}
