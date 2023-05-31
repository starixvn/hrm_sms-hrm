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
    manpower_employee_index: ['./www/sources/css/pages/manpower/employee/index.scss']
};

const scriptPaths = {
    build: 'wwwroot/js',

    language: ['./www/sources/js/common/languages/ViewResources.js'],
    main: ['./www/sources/js/main.js'],
    login: ['./www/sources/js/pages/login/index.js'],
    home: ['./www/sources/js/pages/home/index.js'],
    unit_head_weekly_schedule: ['./www/sources/js/pages/unit-head/index.js'],
    report_manpower: ['./www/sources/js/pages/report/manpower/index.js'],
    manpower_employee_index: ['./www/sources/js/pages/manpower/employee/index.js']
};

module.exports = {
    projectRoot: projectRoot,
    cssPaths: cssPaths,
    scriptPaths: scriptPaths
};