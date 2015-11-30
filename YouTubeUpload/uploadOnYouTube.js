
$(document).ready(function () {
    $(document).on("click", "#btnVedioSubmit", uploadMetaDataYouTube);

});

var uploadMetaDataYouTube = function () {

    beginFileUpload();

    var lectureId = 144;
    var formData = new FormData();
    var totalFiles = document.getElementById("flyupdVideo").files.length;

    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("flyupdVideo").files[i];
        formData.append("flyupdVideo", file);
    }

    jqxhr = $.ajax({
        async: true,
        url: ('/YouTube/YouTubeUpload?lectureId=' + lectureId),
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'POST'
    }).done(function (notice) {
        if (notice.error) {
            alert(notice.error);
        }
        if (notice.message) {
            alert(notice.message)
        }
        $('#statusMessage').text("Upload Finished.");
        return;
    });
}

//function beginFileUpload() {
//    var url = "/YouTube/GetYouTubeUploadingStatus";
//    $.get(url)
//    .done(function (result) {
//        if (result.statusMessage !== 'Completed') {

//            $("#progressBar").progressbar({ value: parseInt(result.totalSent) }, { max: parseInt(result.totalSize) });

//            var message = result.statusMessage + ". File Uploaded " + result.totalSize + "/" + result.totalSent;
//            $("#statusMessage").text(message);

//            console.log(message);
//            beginFileUpload();
//        }
//    })
//    .fail(function () {
//        console.log("Unable to get YouTube uploading status.");
//    });
//}



function beginFileUpload() {
    var handle;
    console.log('Checking YouTube upload status');

    var url = "/YouTube/GetYouTubeUploadingStatus";
    $.get(url)
    .done(function (result) {
        $("#progressBar").progressbar({ value: parseInt(result.totalSent) }, { max: parseInt(result.totalSize) });
        var message = result.statusMessage;
        $("#statusMessage").text(message);
        console.log(message);
        if (message == "Completed") {
            return;
        }
        if (message === "Done") {
            return;
        }
    })
    .fail(function () {
        console.log("Unable to get YouTube upload status");
        return;
    });

    setTimeout(function () {
        beginFileUpload();
    }, 6000);
}