import React, { Component } from 'react';
import * as dayjs from 'dayjs';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';
import Filter from './Filter';
import Grid from './grid/index';

const DateFormat = 'YYYY-MM-DD';
const MobileWidth = 576;

class AppContainer extends Component {
    constructor(props) {
        super(props);

        const fromDate = dayjs().day(1),
            toDate = dayjs(fromDate, DateFormat).add(6, 'day');

        this.state = {
            isMobile: window.innerWidth <= MobileWidth ? true : false,
            filter: {
                fromDate: dayjs(fromDate, DateFormat).format(DateFormat),
                toDate: dayjs(toDate, DateFormat).format(DateFormat)
            },
            schedules: []
        };
    }

    async componentDidMount() {
        window.addEventListener('resize', this.handleResize);

        await this.getWeeklySchedule();
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.handleResize);
    }

    handleResize = () => {
        const isMobile = this.state.isMobile;
        if (!isMobile && window.innerWidth <= MobileWidth) {
            this.setState({ isMobile: true });
        } else {
            if (isMobile && window.innerWidth > MobileWidth) {
                this.setState({ isMobile: false });
            }
        }
    };


    getWeeklySchedule = async () => {
        const url = Site.resolveClientUrl('UnitHead/getWeeklySchedule');
        await DataService.postDataAsync(url, this.state.filter).then(response => {
            this.setState({ schedules: response.data });
        });
    };

    onPrev = event => {
        event.preventDefault();

        let { fromDate } = this.state.filter;
        fromDate = dayjs(fromDate, DateFormat).subtract(7, 'day').format(DateFormat);
        const toDate = dayjs(fromDate, DateFormat).add(6, 'day').format(DateFormat);
        this.setState({
            filter: {
                fromDate,
                toDate
            }
        }, () => {
            this.getWeeklySchedule();
        });
    };

    onNext = () => {
        event.preventDefault();

        let { fromDate } = this.state.filter;
        fromDate = dayjs(fromDate, DateFormat).add(7, 'day').format(DateFormat);
        const toDate = dayjs(fromDate, DateFormat).add(6, 'day').format(DateFormat);
        this.setState({
            filter: {
                fromDate,
                toDate
            }
        }, () => {
            this.getWeeklySchedule();
        });
    };

    onThisWeek = () => {
        event.preventDefault();

        const today = dayjs().day(1).format(DateFormat);
        const { fromDate } = this.state.filter;

        if (!dayjs(fromDate).isSame(dayjs(today))) {
            const toDate = dayjs(today, DateFormat).add(6, 'day').format(DateFormat);
            this.setState({
                filter: {
                    fromDate: today,
                    toDate
                }
            }, () => {
                this.getWeeklySchedule();
            });
        }
    };

    render() {
        const { isMobile, filter, schedules } = this.state;

        return (
            <div className={`unit-head-weekly-schedule`}>
                <Filter isMobile={isMobile}
                    fromDate={filter.fromDate}
                    toDate={filter.toDate}
                    onPrev={this.onPrev}
                    onNext={this.onNext}
                    onThisWeek={this.onThisWeek}
                />
                <Grid isMobile={isMobile}
                    fromDate={filter.fromDate}
                    data={schedules}
                />
            </div>
        );
    }
}

export default AppContainer;