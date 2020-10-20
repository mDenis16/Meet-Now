import React, { Component } from 'react';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import { Layout } from './app/main/Layout'
import { Conference } from './app/main/Conference/'
import { Login } from './app/main/Login/'
import { Main } from './app/main/Main'
import axios from "axios";

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');

export default class App extends Component {
  constructor(props) {
    super(props);
    this.state = { userData: {}, logged: false, loading: true };
  }
  componentDidMount() {

    axios.get('/api/fetchUserData')
      .then((response) => {

      
        
        setTimeout(() =>{
         
          this.setState({ userData: response.data.user, logged:  response.data.success, loading: false });
        }, 2000);
      }, (error) => {
        console.log(error);
      });
  }

  render() {
    if (this.state.loading) {
       return (
         <div className="loader-container">
           <div className="loader"></div>
         </div>

       );
    }
    return (
      <Switch>
         <Route path="/" exact render={(props) => <Main logged={this.state.logged  } {...props}/>} />
        <Route path="/login" render={(props) => <Login logged={this.state.logged  } {...props}/>} />
        <Route path="/meet/:roomId" render={(props) => <Conference  logged={this.state.logged  }  {...props}/>} />

      </Switch>


    );
  }
}
