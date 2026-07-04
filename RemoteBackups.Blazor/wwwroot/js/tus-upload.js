window.tusUpload = {
    MAX_CONCURRENT_UPLOADS: 2,
    activeUploads: 0,
    queue: [],

    uploadFiles: function (fileInputId, endpointUrl, dotnetHelper, token) {
        var fileInput = document.getElementById(fileInputId);
        var files = fileInput.files;

        for (var i = 0; i < files.length; i++) {
            this.queue.push(files[i]);
        }

        this.processQueue(endpointUrl, dotnetHelper, token);
    },

    processQueue: function (endpointUrl, dotnetHelper, token) {
        if (this.activeUploads >= this.MAX_CONCURRENT_UPLOADS || this.queue.length === 0) {
            return;
        }

        var file = this.queue.shift();
        this.activeUploads++;

        var upload = new tus.Upload(file, {
            endpoint: endpointUrl,
            headers: { "Authorization": "Bearer " + token },
            metadata: { filename: file.name, filetype: file.type },
            onProgress: (bytes, total) => {
                dotnetHelper.invokeMethodAsync('OnUploadProgress', file.name, (bytes / total * 100).toFixed(0));
            },
            onSuccess: () => {
                this.activeUploads--;
                dotnetHelper.invokeMethodAsync('OnUploadSuccess', file.name);
                this.processQueue(endpointUrl, dotnetHelper, token);
            },
            onError: (error) => {
                this.activeUploads--;
                dotnetHelper.invokeMethodAsync('OnUploadError', file.name, error.toString());
                this.processQueue(endpointUrl, dotnetHelper, token);
            }
        });

        upload.start();
        this.processQueue(endpointUrl, dotnetHelper, token);
    }
};