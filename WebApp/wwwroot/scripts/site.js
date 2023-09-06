﻿var JsFunctions = window.JsFunctions || {};

window.saveAsFile = function (filename, bytesBase64) {
    if (navigator.msSaveBlob) {
        // download document in Edge browser
        var data = window.atob(bytesBase64);
        var bytes = new Uint8Array(data.length);
        for (var i = 0; i < data.length; i++) {
            bytes[i] = data.charCodeAt(i);
        }
        var blob = new Blob([bytes.buffer], { type: "application/octet-stream" });
        navigator.msSaveBlob(blob, filename);
    }
    else {
        var link = document.createElement('a');
        link.download = filename;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // needed for Firefox
        link.click();
        document.body.removeChild(link);
    }
}