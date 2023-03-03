import React from 'react';

const CellNickname = ({ data }) => {
    return (
        <div className="table-cell cell-nickname">
            <div className="font-weight-bold">
                {data.fullName} {`${data.nickname ? ` (${data.nickname})` : ''}`}
            </div>
            <div>
                <span className="department bg-secondary">{data.departmentCode}</span>
            </div>
        </div>
    );
};

export default CellNickname;