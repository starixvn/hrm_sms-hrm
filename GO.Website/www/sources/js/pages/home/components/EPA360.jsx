import React, { Component } from 'react';

export default class EPA360 extends Component {
    constructor(props) {
        super(props);

        this.state = {
            todolist: null
        };
    }

    render() {
        return (
            <div className="card">
                <div className="card-content">
                    <div className="card-title">
                        <span className="txt">360° EPA</span>
                    </div>
                    <div className="card-body">
                        <div className="todolist-items">
                            <p className="todolist-item green-border-left">
                                <span className="txt">
                                    Nothing to do
                                </span>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}