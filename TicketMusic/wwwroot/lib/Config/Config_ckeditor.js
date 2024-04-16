
class MyUploadAdapter {
    constructor(loader, editor) {
        this.loader = loader;
        this.temporaryImageUrl = null;
        this.editor = editor;
    }

    upload() {
        return this.loader.file.then(file => new Promise((resolve, reject) => {
            this._initRequest();
            this._initListeners(resolve, reject, file);
            this._sendRequest(file, resolve, reject);
        }));
    }

    abort() {
        if (this.xhr) {
            this.xhr.abort();
        }
    }

    _initRequest() {
        this.xhr = new XMLHttpRequest();
        this.xhr.open('POST', '/admin/upload-local-main', true);
        this.xhr.responseType = 'json';
    }

    _initListeners(resolve, reject, file) {
        const { xhr, loader } = this;
        const genericErrorText = `Couldn't upload file: ${file.name}.`;

        xhr.addEventListener('error', () => reject(genericErrorText));
        xhr.addEventListener('abort', () => reject());
        xhr.addEventListener('load', () => {
            const response = xhr.response;

            if (!response || response.error) {
                return reject(response && response.error ? response.error.message : genericErrorText);
            }

            resolve({
                default: response.url
            });
        });

        if (xhr.upload) {
            xhr.upload.addEventListener('progress', evt => {
                if (evt.lengthComputable) {
                    loader.uploadTotal = evt.total;
                    loader.uploaded = evt.loaded;
                }
            });
        }
    }

    _sendRequest(file) {
        return new Promise((resolve, reject) => {

            const data = new FormData();
            data.append('upload', file)
            const self = this;

            this.xhr.addEventListener('load', function () {
                const response = self.xhr.response;
                if (!response || response.error) {
                    return reject(response && response.error ? response.error.message : genericErrorText);
                }

                const imageUrl = response.urls[0];
                const altText = prompt("Please enter alt text for the image:", "Alt text");

                if (!self.editor) {
                    console.error("Editor instance not available.");
                    return;
                }

                const selection = self.editor.model.document.selection;

                const imageElement = document.createElement('img');
                imageElement.src = imageUrl;
                imageElement.alt = altText || "";



                self.editor.setData(`${self.editor.getData()}<img src="${imageUrl}" alt="${altText}" title="${altText}"> `);
                resolve({
                    default: imageUrl
                });
            });

            this.xhr.addEventListener('error', function () {
                console.error("Error during upload.");
            });

            this.xhr.addEventListener('abort', function () {
                console.error("Upload aborted.");
            });
            this.xhr.send(data);
        });
    }
}

function MyCustomUploadAdapterPlugin(editor) {
    editor.plugins.get('FileRepository').createUploadAdapter = loader => new MyUploadAdapter(loader, editor);
}
