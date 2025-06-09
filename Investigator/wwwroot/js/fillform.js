const baseUrl = document.getElementById("baseUrl").value;
document.getElementById('form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const form = e.target;
    const formId = form.dataset.formId;
    const formData = new FormData();
    const answers = [];
    let formIsValid = true;

    // Iterate through all question containers
    const questionElements = document.querySelectorAll('.question-item');
    console.log("Found questions:", questionElements.length);    

    questionElements.forEach(q => {
        const questionId = parseInt(q.dataset.questionId);
        const questionType = q.dataset.questionType;
        const isRequired = q.dataset.required;

        if (questionType === "Phone") {
            const countryCodeInput = q.querySelector('input.input-group-text');
            const phoneNumberInput = q.querySelector('input[name="phone"]');
            if (countryCodeInput && phoneNumberInput) {
                const fullNumber = `${countryCodeInput.value}${phoneNumberInput.value}`;
                if (isRequired === "true" && phoneNumberInput.value.trim() === "") {
                    phoneNumberInput.classList.add("is-invalid");
                    formIsValid = false;
                } else {
                    answers.push({
                        questionId: questionId,
                        answer: fullNumber
                    });
                }
            }
            return; // skip further processing
        }

        const input = q.querySelector('input, textarea, select');
        if (!input) return;

        if (input.type === 'file') {
            const fileInput = input;
            if (fileInput.files.length > 0) {
                for (let i = 0; i < fileInput.files.length; i++) {
                    formData.append("Files", fileInput.files[i]);
                }
                answers.push({
                    questionId: questionId,
                    answer: Array.from(fileInput.files).map(f => f.name).join('||')
                });
            } else if (isRequired && fileInput.files.length == 0) {
                fileInput.classList.add("is-invalid");
                formIsValid = false;
            }
        }
        if (input.type === 'checkbox') {
            const checkboxes = q.querySelectorAll('input[type="checkbox"]:checked');
            const values = Array.from(checkboxes).map(cb => cb.value);
            if (isRequired && values.length === 0){
                input.classList.add("is-invalid");
                formIsValid = false;
            } else {
                answers.push({
                    questionId: questionId,
                    answer: values.join(', ')
                });
            }            
        }
        else if (input.type === 'radio') {
            const radioboxes = q.querySelectorAll('input[type="radio"]:checked');
            const values = Array.from(radioboxes).map(cb => cb.value);
            if (isRequired && values.length === 0) {
                input.classList.add("is-invalid");
                formIsValid = false;
            } else {
                answers.push({
                    questionId: questionId,
                    answer: values.join(', ')
                });
            }
        }
        else {
            if(isRequired && input.value.trim() === "") {
                input.classList.add("is-invalid");
                formIsValid = false;
            }
            else
            {
                answers.push({
                    questionId: questionId,
                    answer: input.value
                });
            }
        }
    });

    if (!formIsValid) {
        toastr.warning("Please fill all required fields.");
        return;
    }
    formData.append('FormId', formId);
    formData.append('Answers', JSON.stringify(answers));

   
    fetch('/Admin/Form/SubmitForm', {
        method: 'POST',
        body: formData
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("Failed to submit the form.");
        }
        return response.json();
    })
    .then(data => {
        toastr.success(data.message || "Form is submitted successfully!");
        window.location = baseUrl + "/Admin/Form/SubmissionConfirmation?formId="+ formId;   
    })
    .catch(error => {
        console.error("Error : ", error);
        toastr.error("An error occurred during submission.");
    });
    
});

// manage files input in UI

// Keep track of which inputs have already had the event listener attached
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




