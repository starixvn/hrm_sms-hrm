import React from 'react';

import THead from './THead';
import Row from './Row';

const Table = ({ isMobile, fromDate, data }) => {
    return (
        <div className={`table-wrapper ${isMobile ? 'mobile' : ''}`}>
            <div className={`table-header`}>
                <THead isMobile={isMobile} fromDate={fromDate} />
            </div>
            <div className="table-body">
                {
                    data.length === 0 &&
                    <div className="nodata">
                        No information available
                    </div>
                }
                {
                    data.length > 0 &&
                    data.map(item => <Row
                        key={`unit-head-${item.siteId}-${item.empId}`}
                        isMobile={isMobile} data={item}
                    />)
                }
            </div>
        </div>
    );
};

export default Table;