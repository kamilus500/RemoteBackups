window.tusInterop = {
    uploadFile: function (fileInputId, endpointUrl, dotnetHelper, token) {

        if (typeof tus === 'undefined') {
            console.warn("Biblioteka TUS jeszcze się ładuje, ponowna próba za 200ms...");
            setTimeout(() => { window.tusInterop.uploadFile(fileInputId, endpointUrl, dotnetHelper); }, 200);
            return;
        }

        var fileInput = document.getElementById(fileInputId);
        var file = fileInput.files[0];

        if (!file) {
            alert("Proszę wybrać plik.");
            return;
        }

        var upload = new tus.Upload(file, {
            endpoint: endpointUrl,
            headers: {
                "Authorization": "Bearer " + token
            },
            retryDelays: [0, 3000, 5000, 10000, 20000],
            metadata: {
                filename: file.name,
                filetype: file.type
            },
            onError: function (error) {
                console.error("Błąd przesyłania: " + error);

                dotnetHelper.invokeMethodAsync('OnUploadError', error.toString());
            },
            onProgress: function (bytesUploaded, bytesTotal) {
                var percentage = (bytesUploaded / bytesTotal * 100).toFixed(2);

                dotnetHelper.invokeMethodAsync('OnUploadProgress', percentage);
            },
            onSuccess: function () {

                dotnetHelper.invokeMethodAsync('OnUploadSuccess', upload.url);
            }
        });

        upload.start();
    }
};