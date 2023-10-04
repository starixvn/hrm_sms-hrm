import React from 'react';
import ReactDOM from 'react-dom';
import Login from './Login';

ReactDOM.render(
    <Login />,
    document.getElementById("panel-login").appendChild(document.createElement("div"))
);