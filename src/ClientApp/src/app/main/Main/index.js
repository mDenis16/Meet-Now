import React, { Component } from 'react';
import axios from 'axios';
import { Video } from '../Conference/components/Video'
import './style/index.css'
import { withRouter } from 'react-router-dom';
export class Main extends Component {

    constructor(props) {
        super(props);

        this.state = {
            user: 'denis',
            hubConnection: null,
            toggleMute: false,
            generateLink: 'none'
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

        var constraints = { audio: false, video: { width: 1280, height: 720 } };
        navigator.mediaDevices.getUserMedia(constraints)
            .then(function (mediaStream) {


                document.getElementById('video-preview').srcObject = mediaStream;
            })
            .catch(function (err) { console.log(err.name + ": " + err.message); });
    }
    createNew() {
        axios.get('room/createRoom')
            .then(res => {
                this.setState({ generateLink: res.data.link });
            })

    }

    render() {

        return (
            <>
               

                <div className="content">
                    <div className="left-side">
                        <div className="header">
                            Întâlniri video premium. Acum gratuite pentru toată lumea.
                       </div>
                        <div className="text">
                            Am reproiectat Meet Now, serviciul pe care l-am creat pentru întâlniri de companie în siguranță. Acum, este gratuit și disponibil pentru toată lumea.
                       </div>
                        <div className="button-group">
                            <div className="button" onClick={this.createNew.bind(this)}>Intalnire noua</div>
                            <div className="input"><input type="text" value={this.state.generateLink} className="element" /></div>
                        </div>
                    </div>


                    <video className="right-side" autoPlay id="video-preview">

                    </video>
                </div>
            </>
        );
    }
}
