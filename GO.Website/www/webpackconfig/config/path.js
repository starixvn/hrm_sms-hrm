const path = require('path');

const projectRoot = path.resolve(__dirname, '../../../');

const cssPaths = {
    build: 'wwwroot/css',

    main: ['./www/sources/css/main.scss'],
    error: ['./www/sources/css/pages/error.scss'],
    login: ['./www/sources/css/pages/login.scss'],
    home: ['./www/sources/css/pages/home.scss'],
    unit_head_weekly_schedule: ['./www/sources/css/pages/unit-head/weekly-schedule.scss'],
    report_manpower: ['./www/sources/css/pages/report/manpower.scss'],
    manpower_employee_index: ['./www/sources/css/pages/manpower/employee.scss'],
    manpower_period_index: ['./www/sources/css/pages/manpower/period/index.scss'],
    manpower_flow_index: ['./www/sources/css/pages/manpower/flow.scss'],
    annual_manpower_forecast_request_mypage: ['./www/sources/css/pages/manpower/annual-request/my-request.scss'],
    annual_manpower_forecast_request_memberpage: ['./www/sources/css/pages/manpower/annual-request/member-request.scss'],
    annual_manpower_forecast_request_create: ['./www/sources/css/pages/manpower/annual-request/create.scss'],
    manpower_period_detail: ['./www/sources/css/pages/manpower/period/detail.scss']
};

const scriptPaths = {
    build: 'wwwroot/js',

    language: ['./www/sources/js/common/languages/ViewResources.js'],
    main: ['./www/sources/js/main.js'],
    login: ['./www/sources/js/pages/login/index.js'],
    home: ['./www/sources/js/pages/home/index.js'],
    unit_head_weekly_schedule: ['./www/sources/js/pages/unit-head/index.js'],
    report_manpower: ['./www/sources/js/pages/report/manpower/index.js'],
    manpower_employee_index: ['./www/sources/js/pages/manpower/employee/index.js'],
    manpower_period_index: ['./www/sources/js/pages/manpower/period/index.js'],
    manpower_flow_index: ['./www/sources/js/pages/manpower/flow/index.js'],
    manpower_period_detail: ['./www/sources/js/pages/manpower/period/detail.js'],
    annual_manpower_forecast_request_mypage: ['./www/sources/js/pages/manpower/annual-request/my-request/index.js'],
    annual_manpower_forecast_request_memberpage: ['./www/sources/js/pages/manpower/annual-request/member-request/index.js'],
    annual_manpower_forecast_request_create: ['./www/sources/js/pages/manpower/annual-request/create/index.js'],
    annual_manpower_forecast_request_detail: ['./www/sources/js/pages/manpower/annual-request/detail/index.js']
};

module.exports = {
    projectRoot: projectRoot,
    cssPaths: cssPaths,
    scriptPaths: scriptPaths
};