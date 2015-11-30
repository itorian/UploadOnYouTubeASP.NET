
$(document).ready(function () {
    $(document).on("click", "#btnVedioSubmit", uploadMetaDataYouTube);
});

var uploadMetaDataYouTube = function () {
    beginFileUpload();
    var formData = new FormData();
    var totalFiles = document.getElementById("flyupdVideo").files.length;

    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("flyupdVideo").files[i];
        formData.append("flyupdVideo", file);
    }

    jqxhr = $.ajax({
        async: true,
        url: ('/YouTube/YouTubeUpload'),
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