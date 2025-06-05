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
                //answers.push({
                //    questionId: questionId,
                //    answer: fullNumber
                //});
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
        } else if (input.type === 'checkbox') {
            const checkboxes = q.querySelectorAll('input[type="checkbox"]:checked');
            const values = Array.from(checkboxes).map(cb => cb.value);
            if (isRequired && values.length === 0) {
                input.classList.add("is-invalid");
                formIsValid = false;
            } else {
                answers.push({
                    questionId: questionId,
                    answer: values.join(', ')
                });
            }            
        } else {
            if (isRequired && input.value.trim() === "") {
                input.classList.add("is-invalid");
                formIsValid = false;
            } else {
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

