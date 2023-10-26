import React from 'react';
import ReactDOM from 'react-dom';
import Login from './Login';
let element = document.createElement('div');
element.setAttribute("id", "login-page");
ReactDOM.render(
    <Login />,
    document.getElementById('panel-login').appendChild(element)
);