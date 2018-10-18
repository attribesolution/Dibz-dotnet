$(function () {

});

// Ajax call methods
function ajaxCoreCall(url, contentType, processData, data, cbSuccess, cbFailure, cbError) {
    var errorMsg = "Ett fel uppstod. Kontakta systemadministratren fr mer information.";
    $.ajax({
        type: "POST",
        url: url,
        contentType: contentType,
        processData: processData,
        data: data,
        success: function (response) {
            if (cbSuccess) { cbSuccess(response); }
        },
        failure: function (response) {  //alert(errorMsg);
            if (cbFailure) { cbFailure(response); } else { ShowError(errorMsg); console.log(response.responseText) }
        },
        error: function (response) {
            if (cbError) { cbError(response); } else { ShowError(errorMsg); console.log(response.responseText) }
        }
    });
}

function ajaxCall(url, data, cbSuccess, cbFailure, cbError) {
    ajaxCoreCall(url, "application/json; charset=utf-8", true, JSON.stringify(data), cbSuccess, cbFailure, cbError);
}

function ajaxMIMECall(url, data, cbSuccess, cbFailure, cbError) {
    ajaxCoreCall(url, false, false, data, cbSuccess, cbFailure, cbError);
}

function ShowModal(elemId, backdrop) {
    if (backdrop && backdrop === true) {
        $(elemId).modal({
            escapeClose: false, clickClose: false, showClose: true
        });
    }
    else {
        $(elemId).modal();
    }
}

var Contants = {
    ROOT_URL: "",
    CONTENT_URL: ""
};