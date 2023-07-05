const alias = require('../config/alias');
const pathConfig = require('../config/path');
const pluginGenerator = require('../config/plugin');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');

function generateConfig(environment) {
    const jsPath = `${pathConfig.scriptPaths.build}/${environment}`;
    const cssPath = `${pathConfig.cssPaths.build}/${environment}`;
    return {
        resolve: {
            alias: alias,
            extensions: ['.js', '.jsx']
        },
        entry: {
            /*js*/
            [`${jsPath}/lib/lang-en-US`]: pathConfig.scriptPaths.language,
            [`${jsPath}/main`]: pathConfig.scriptPaths.main,
            [`${jsPath}/pages/login`]: pathConfig.scriptPaths.login,
            [`${jsPath}/pages/home`]: pathConfig.scriptPaths.home,
            [`${jsPath}/pages/unit-head/weekly-schedule`]: pathConfig.scriptPaths.unit_head_weekly_schedule,
            [`${jsPath}/pages/report/manpower/index`]: pathConfig.scriptPaths.report_manpower,
            [`${jsPath}/pages/manpower/employee/index`]: pathConfig.scriptPaths.manpower_employee_index,
            [`${jsPath}/pages/manpower/period/index`]: pathConfig.scriptPaths.manpower_period_index,
            [`${jsPath}/pages/manpower/flow/index`]: pathConfig.scriptPaths.manpower_flow_index,
            [`${jsPath}/pages/manpower/annual-request/my-request`]: pathConfig.scriptPaths.manpower_forecast_my_annual_request,
            [`${jsPath}/pages/manpower/annual-request/member-request`]: pathConfig.scriptPaths.manpower_forecast_member_annual_request,
            [`${jsPath}/pages/manpower/annual-request/index`]: pathConfig.scriptPaths.manpower_forecast_index,
            [`${jsPath}/pages/manpower/detail-annual-request/index`]: pathConfig.scriptPaths.manpower_forecast_detail_index,
            [`${jsPath}/pages/manpower/period/detail`]: pathConfig.scriptPaths.manpower_period_detail,

            /*css*/
            [`${cssPath}/main`]: pathConfig.cssPaths.main,
            [`${cssPath}/error`]: pathConfig.cssPaths.error,
            [`${cssPath}/pages/login`]: pathConfig.cssPaths.login,
            [`${cssPath}/pages/home`]: pathConfig.cssPaths.home,
            [`${cssPath}/pages/unit-head/weekly-schedule`]: pathConfig.cssPaths.unit_head_weekly_schedule,
            [`${cssPath}/pages/report/manpower/index`]: pathConfig.cssPaths.report_manpower,
            [`${cssPath}/pages/manpower/employee/index`]: pathConfig.cssPaths.manpower_employee_index,
            [`${cssPath}/pages/manpower/period/index`]: pathConfig.cssPaths.manpower_period_index,
            [`${cssPath}/pages/manpower/flow/index`]: pathConfig.cssPaths.manpower_flow_index,
            [`${cssPath}/pages/manpower/annual-request/my-request`]: pathConfig.cssPaths.manpower_forecast_my_annual_request,
            [`${cssPath}/pages/manpower/annual-request/member-request`]: pathConfig.cssPaths.manpower_forecast_member_annual_request,
            [`${cssPath}/pages/manpower/annual-request/index`]: pathConfig.cssPaths.manpower_forecast_index,
            [`${cssPath}/pages/manpower/detail-annual-request/index`]: pathConfig.cssPaths.manpower_forecast_detail_index,
            [`${cssPath}/pages/manpower/period/detail`]: pathConfig.cssPaths.manpower_period_detail
        },
        output: {
            path: pathConfig.projectRoot,
            filename: "[name].chunk.js",
            chunkFilename: `${jsPath}/lib/[name].chunk.js`
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    loader: ["babel-loader"],
                    exclude: /node_modules/
                }
            ]
        },
        plugins: pluginGenerator(environment),
        stats: {
            assets: false,
            colors: true,
            version: false,
            hash: false,
            timings: true,
            chunks: false,
            chunkModules: false,
            children: true
        },
        optimization: {
            minimizer: [
                new UglifyJsPlugin({
                    sourceMap: true,
                    uglifyOptions: {
                        output: {
                            comments: false
                        }
                    }
                }),
                new OptimizeCssAssetsPlugin()
            ],
            splitChunks: {
                chunks: 'all',
                minSize: 30000,
                maxSize: 0,
                minChunks: 2,
                maxAsyncRequests: 5,
                maxInitialRequests: 3,
                name: false,
                cacheGroups: {
                    // disable default cache group
                    default: false,
                    vendors: false,
                    vendor: {
                        name: 'vendors',
                        test: /[\\/]node_modules[\\/](react|react-dom|react-bootstrap)[\\/]/,
                        minChunks: 2,
                        priority: -10
                    }
                }
            }
        }
    };
}

module.exports = generateConfig;