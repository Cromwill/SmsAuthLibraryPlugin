mergeInto(LibraryManager.library, {
    JsSetTimeout: function (message, timeout, callback) {
        // Create copy of message because it might be deleted before callback is run
        var stringMessage = UTF8ToString(message);
        var buffer = stringToNewUTF8(stringMessage);
        setTimeout(function () {
            {{{ makeDynCall('vi', 'callback') }}} (buffer);
            _free(buffer);
        }, timeout);
    }
});
