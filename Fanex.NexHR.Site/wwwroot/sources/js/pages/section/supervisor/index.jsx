import React from 'react';
import ReactDOM from 'react-dom';
import Supervisor from './Supervisor';

ReactDOM.render(
    <Supervisor pageType={1} objectId={document.getElementById('SectionId').value} />,
    document.getElementById('supervisor-wrapper')
);