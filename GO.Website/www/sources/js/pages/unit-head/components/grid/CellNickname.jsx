import React from 'react';

const CellNickname = ({ data, isMobile }) => {
    return (
        <div className={`${isMobile ? 'user-row' : 'table-cell cell-nickname'}`}>
            <div className="fullname">
                {data.fullName} {`${data.nickname ? ` (${data.nickname})` : ''}`}
            </div>
            <div>
                <span className="department">{data.departmentCode}</span>
                <span className={`company ${data.companyName ? data.companyName.toLowerCase().replaceAll(' ', '-') : ''}`}>{data.companyName}</span>
            </div>
        </div>
    );
};

export default CellNickname;