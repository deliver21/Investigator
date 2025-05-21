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


let fileInput = document.getElementById("file-input");
let fileList = document.getElementById("files-list");
let numOfFiles = document.getElementById("num-of-files");

fileInput?.addEventListener("change", () => {
    fileList.innerHTML = "";
    numOfFiles.textContent = `${fileInput.files.length} Files Selected`;

    for (i of fileInput.files) {
        let reader = new FileReader();
        let listItem = document.createElement("li");
        let fileName = i.name;
        let fileSize = (i.size / 1024).toFixed(1);
        listItem.innerHTML = `<p>${fileName}</p><p>${fileSize}KB</p>`;
        if (fileSize >= 1024) {
            fileSize = (fileSize / 1024).toFixed(1);
            listItem.innerHTML = `<p>${fileName}</p><p>${fileSize}MB</p>`;
        }
        fileList.appendChild(listItem);
    }
});
