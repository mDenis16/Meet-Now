import React, { Component } from 'react';
import './style/index.css'
import axios from "axios";

export class Login extends Component {

    constructor() {
        super();

        this.state = {
            email: '',
            password: ''
        };
        this.handleChange = this.handleChange.bind(this);
        this.handleLogin = this.handleLogin.bind(this);

    }
    componentDidMount() {
        if (this.props.logged) {
            this.props.history.push("/");
        }
    }
    handleChange(ev) {
        this.setState({ [ev.target.name]: ev.target.value });
    }
    handleLogin(e) {
        e.preventDefault();
        axios.post('https://localhost:5001/auth/login', {
            email: this.state.email,
            password: this.state.password
        })
            .then((response) => {

                if (response.data.success) {
                    this.props.history.push("/");
                }
            }, (error) => {
                console.log(error);
            });
    }
    render() {


        return (
            <div className="container">

                <div className="login-form">
                    <div className="header">Google Meets</div>
                    <div className="input"><input className="field" value={this.state.email} type="email" placeholder="example@mail.com" name="email" onChange={this.handleChange}></input></div>
                    <div className="input"><input className="field" value={this.state.password} placeholder="***" onChange={this.handleChange} name="password" type="password"></input></div>
                    <div className="button" onClick={this.handleLogin}>Login</div>
                </div>
            </div>
        );
    }
}
