window.uploadPortal = {
    currentController: null,

    getSelectedFileInfo(input) {
        const files = input && input.files ? Array.from(input.files) : [];
        const file = files[0] || null;
        if (!file) {
            return null;
        }

        return {
            name: file.name,
            sizeBytes: file.size,
            type: file.type || "application/octet-stream"
        };
    },

    getSelectedFilesInfo(input) {
        const files = input && input.files ? Array.from(input.files) : [];
        return files.map((file) => ({
            name: file.name,
            sizeBytes: file.size,
            type: file.type || "application/octet-stream"
        }));
    },

    clearFileInput(inputId) {
        const input = document.getElementById(inputId);
        if (!input) {
            return;
        }

        input.value = "";
    },

    updateStatus(options) {
        const progressBar = document.getElementById("upload-progress-bar");
        const progressText = document.getElementById("upload-progress-text");
        const speedText = document.getElementById("upload-speed-text");
        const etaText = document.getElementById("upload-eta-text");
        const statusText = document.getElementById("upload-status-text");

        if (progressBar) {
            progressBar.style.width = `${Math.max(0, Math.min(100, Number(options.percentage || 0)))}%`;
        }

        if (progressText) {
            progressText.textContent = `${Math.round(Number(options.percentage || 0))}%`;
        }

        if (speedText) {
            const speedInMb = Number(options.speedBytesPerSecond || 0) / (1024 * 1024);
            speedText.textContent = `${speedInMb >= 1 ? speedInMb.toFixed(2) : speedInMb.toFixed(4)} MB/s`;
        }

        if (etaText) {
            etaText.textContent = `ETA: ${options.etaText || "--"}`;
        }

        if (statusText) {
            statusText.textContent = options.text || "Preparing upload…";
        }
    },

    cancelCurrentUpload() {
        if (this.currentController) {
            this.currentController.abort();
            this.currentController = null;
        }
    },

    async uploadFile(options) {
        const input = document.getElementById(options.inputId);
        const files = input && input.files ? Array.from(input.files) : [];
        const fileIndex = Number.isInteger(options.fileIndex) ? options.fileIndex : 0;
        const file = files[fileIndex] || files[0] || null;
        if (!file) {
            throw new Error("Please select a file to upload.");
        }

        let storageBlob;
        try {
            storageBlob = await import("./vendor/azureStorageBlob.js");
        }
        catch (error) {
            throw new Error("The Azure Blob SDK failed to load. Please refresh the page and try again.", { cause: error });
        }

        const blockBlobClient = new storageBlob.BlockBlobClient(options.sasUrl);
        const controller = new AbortController();
        this.currentController = controller;

        let uploadedBytes = 0;
        const startedAt = performance.now();
        const totalBytes = file.size;

        try {
            await blockBlobClient.uploadData(file, {
                blockSize: 4 * 1024 * 1024,
                concurrency: 4,
                abortSignal: controller.signal,
                onProgress: (progress) => {
                    if (controller.signal.aborted) {
                        return;
                    }

                    uploadedBytes = progress.loadedBytes;
                    const elapsedSeconds = Math.max((performance.now() - startedAt) / 1000, 0.001);
                    const speed = uploadedBytes / elapsedSeconds;
                    const remainingBytes = Math.max(totalBytes - uploadedBytes, 0);
                    const etaSeconds = speed > 0 ? remainingBytes / speed : 0;
                    const percent = totalBytes > 0 ? (uploadedBytes / totalBytes) * 100 : 0;

                    this.updateStatus({
                        text: `Uploading ${file.name}…`,
                        percentage: percent,
                        speedBytesPerSecond: speed,
                        etaText: `${etaSeconds >= 60 ? `${(etaSeconds / 60).toFixed(1)} min` : `${etaSeconds.toFixed(0)} s`}`
                    });
                }
            });

            await blockBlobClient.setMetadata({
                companyname: options.metadata.companyName || "",
                contactname: options.metadata.contactName || "",
                emailaddress: options.metadata.emailAddress || "",
                supportticket: options.metadata.supportTicket || "",
                originalfilename: options.metadata.originalFileName || "",
                uploaddateutc: options.metadata.uploadDateUtc || new Date().toISOString(),
                uploadid: (options.uploadId || "").toString()
            }, {
                abortSignal: controller.signal
            });

            this.updateStatus({
                text: "Upload completed successfully.",
                percentage: 100,
                speedBytesPerSecond: 0,
                etaText: "0s"
            });

            this.currentController = null;
            return {
                success: true,
                uploadedBytes: uploadedBytes,
                fileName: file.name
            };
        }
        catch (error) {
            if (error && error.name === "AbortError") {
                this.currentController = null;
                throw new Error("Upload cancelled.");
            }

            this.currentController = null;
            throw new Error(error && error.message ? error.message : "The upload failed.");
        }
    }
};
