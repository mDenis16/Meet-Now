import React, { Component } from 'react';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import { ConferenceLayout } from './app/main/Conference/'
import { Login } from './app/main/Login/'
import { Main } from './app/main/Main'
import axios from "axios";

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
class AppHeader extends Component {


  render() {
    return (
      <div className="header">

        <div className="title">Meet Now</div>
        <div className="name">{this.props.user}</div>
      </div>

    );
  }
}

export default class App extends Component {
  constructor(props) {
    super(props);
    this.state = { userData: {}, joined: false, logged: false, loading: true };
    this.mainRef = React.createRef();
  }
  componentDidMount() {

    axios.get('/api/fetchUserData')
      .then((response) => {



        setTimeout(() => {

          this.setState({ userData: response.data.user, logged: response.data.success, loading: false });
        }, 2000);
      }, (error) => {
        console.log(error);
      });
  }

  requestJoin(roomId) {

    axios.get('room/requestJoin/' + roomId)
      .then((response) => {
        console.log(response.data);
        if (response.data.success) {
          this.setState({ joined: true });
        }
      }, (error) => {
        console.log(error);
      });

  }
  render() {
   /* if (this.state.loading) {
      return (
        <div className="loader-container">
          <div className="loader"></div>
        </div>

      );
    }*/

    return (
      <>
       
        <AppHeader user={this.state.userData ? this.state.userData.username : "not logged"} />
        <Switch>
          <Route path="/" exact render={(props) => <Main logged={this.state.logged} {...props} />} />
          <Route path="/login" render={(props) => <Login logged={this.state.logged} {...props} />} />
          <Route path="/room/:roomId" exact render={(props) => <ConferenceLayout requestJoin={this.requestJoin.bind(this)} joined={this.state.joined}  userData={this.state.userData} {...props} />} />

        </Switch>
      </>


    );
  }
}
