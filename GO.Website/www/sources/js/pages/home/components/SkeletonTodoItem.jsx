import React from 'react';
import Skeleton from 'react-loading-skeleton';

const SkeletonTodoItem = () => {
    return (
        <div className="todolist-items">
            {
                [1, 2, 3].map(item =>
                    <div key={`hrm-todolist-${item}`} className="todolist-item green-border-left">
                        <div className="txt">
                            <Skeleton baseColor="#d5d5d5" highlightColor="#c3b4b4" duration={2} />
                            <Skeleton baseColor="#d5d5d5" highlightColor="#c3b4b4" duration={2} />
                        </div>
                        <div className="todolist-actions">
                            <Skeleton baseColor="#d5d5d5" highlightColor="#c3b4b4" duration={2} />
                        </div>
                    </div>
                )
            }
        </div>
    );
};

export default SkeletonTodoItem;