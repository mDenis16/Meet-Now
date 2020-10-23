import React, { Component } from 'react';
import { Video } from '../Video/'
import './style/index.css'
import axios from 'axios';
import { withRouter } from 'react-router-dom';
class ErrorPage extends Component {

    render() {
        return (<div className="content">

            <div className="actions">
                <div className="text">{this.props.errorMessage}</div>

                <div className="button-group">
                    <div className="button auto-width">Go back to main page</div>
                </div>
            </div>
        </div >)
    }
}
export class WaitingPage extends Component {

    constructor(props) {
        super(props);

        this.state = {
            toggleMute: false,
            roomData: undefined,
            error_message: undefined,
            loading: true,
            error_code: 0,
            is_me_owner: false
        };
        console.log(props)
        this.toggleMute = this.toggleMute.bind(this);
        this.copyLink = this.copyLink.bind(this);
        this.linkRef = React.createRef();
        this.requestJoin = this.props.requestJoin.bind(this);
    }
    toggleMute() {
        this.setState({ toggleMute: !this.state.toggleMute });
    }
    copyLink(e) {

        navigator.clipboard.writeText(this.linkRef.current.innerHTML)
        e.target.focus();
    }
    componentDidMount() {

        axios.get('room/fetchRoomData/' + this.props.roomId)
            .then((response) => {

                this.setState({ loading: false, error_code: response.data.error_code, roomData: response.data.success ? response.data.roomData : undefined, error_message: response.data.message,
                is_me_owner: this.props.userData.auth == response.data.roomData.uuid_owner });

                console.log(response.data);
            }, (error) => {
                console.log(error);
            });
    }

    render() {
        if (this.state.loading)
           return (<></>)
        if (this.state.error_code > 0) {
            return <ErrorPage errorMessage={this.state.error_message} />
        }

        var constraints = { audio: false, video: { width: 1280, height: 720 } };
        navigator.mediaDevices.getUserMedia(constraints)
            .then(function (mediaStream) {


                document.getElementById('video-preview').srcObject = mediaStream;
            })
            .catch(function (err) { console.log(err.name + ": " + err.message); });
        return (
            <>


                <div className="content">
                    <div className="video-preview">
                        <video autoPlay className="video" id="video-preview" />

                    </div>


                    <div className="actions">
        <div className="text">{ this.state.is_me_owner ? ' Întâlnirea este pregătită' : 'Ești gata să participi?'}</div>
                        <div className="text-small" ref={this.linkRef} onClick={this.copyLink}>meet.google.com/xpy-yqss-gsm</div>
                        <div className="button-group">

                            <div className="button" onClick={ () => {this.requestJoin(this.props.roomId)}}>Meet Now</div>
                            <div className="button">Present</div>
                        </div>
                    </div>
                </div >
            </>
        );
    }
}
