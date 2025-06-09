let button = document.getElementById("login-submit");
let templateId = document.getElementById("templateId").innerText;

document.addEventListener("DOMContentLoaded", function () {
    const submitButton = document.getElementById("login-submit");

    submitButton.addEventListener("click", function (event) {
        event.preventDefault(); // Prevent default submission

        // Collect all input and textarea fields inside the questions list
        const inputs = document.querySelectorAll("#questions-list input[required], #questions-list textarea[required]");

        let isValid = true;

        inputs.forEach(input => {
            if (!input.value.trim()) {
                isValid = false;
                input.classList.add("is-invalid"); // Add bootstrap validation style
            } else {
                input.classList.remove("is-invalid");
            }
        });

        // Display Toastr message based on validation
        if (isValid) {
            toastr.success("Template has successfully worked!");
        } else {
            toastr.error("Please fill in all required fields!");
        }
    });
});

var connectionTemplateDetails = new signalR.HubConnectionBuilder().withUrl("/hubs/templateDetails").build();

function getLikes() {
    connectionTemplateDetails.send("JoinTemplate",templateId).then((value))
}
function fullfilled() {
    console.log("Connection is succesfully established");
    getLikes();
}

function rejected() {
    console.log("Connection has failed");
}
connectionTemplateDetails.start().then(fullfilled, rejected);


const fileInputsInitialized = new Set();

function collectFiles(questionId) {
    const fileInput = document.getElementById("file-input_" + questionId);
    const fileList = document.getElementById("files-list_" + questionId);
    const numOfFiles = document.getElementById("num-of-files_" + questionId);

    if (!fileInput || fileInputsInitialized.has(questionId)) return;

    fileInputsInitialized.add(questionId); // Mark as initialized

    fileInput.addEventListener("change", () => {
        fileList.innerHTML = "";
        const files = fileInput.files;

        if (files.length === 0) {
            numOfFiles.textContent = "No Files Chosen";
            return;
        }

        numOfFiles.textContent = `${files.length} File${files.length > 1 ? "s" : ""} Selected`;

        for (let file of files) {
            const listItem = document.createElement("li");
            const fileName = file.name;
            let fileSize = (file.size / 1024);
            let fileSizeText = fileSize >= 1024
                ? `${(fileSize / 1024).toFixed(1)} MB`
                : `${fileSize.toFixed(1)} KB`;

            listItem.innerHTML = `<p>${fileName}</p><p>${fileSizeText}</p>`;
            fileList.appendChild(listItem);
        }
    });
}