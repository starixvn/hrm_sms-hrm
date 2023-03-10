import React from 'react';

import THead from './THead';
import Row from './Row';

const Table = ({ isMobile, filter, data, onPrev, onNext, onToday, onChangeCompany }) => {
    return (
        <div className={`table-wrapper ${isMobile ? 'mobile' : ''}`}>
            <THead isMobile={isMobile}
                filter={filter}
                onPrev={onPrev}
                onNext={onNext}
                onToday={onToday}
                onChangeCompany={onChangeCompany}
            />
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