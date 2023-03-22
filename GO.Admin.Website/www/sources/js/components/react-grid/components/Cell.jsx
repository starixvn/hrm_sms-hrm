import React from 'react';

const Cell = props => <td {...props}>{props.children}</td>;

export default Cell;