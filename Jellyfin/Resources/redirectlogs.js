// Visual Studio 2019 dropped WebView debugging support.
// This redirects the console logs so they're visible
window.console.log = function (...data) {
    WebViewBridge.logMessage('window.console.log ' + JSON.stringify(data));
};
window.console.debug = function (...data) {
    WebViewBridge.logMessage('window.console.debug ' + JSON.stringify(data));
};
window.console.info = function (...data) {
    WebViewBridge.logMessage('window.console.info ' + JSON.stringify(data));
};
window.console.warn = function (...data) {
    WebViewBridge.logMessage('window.console.warn ' + JSON.stringify(data));
};
window.console.error = function (...data) {
    WebViewBridge.logMessage('window.console.error ' + JSON.stringify(data));
};
window.onerror = function (...data) {
    WebViewBridge.logMessage('window.onerror ' + JSON.stringify(data));
};
