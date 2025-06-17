const questionsList = document.getElementById('questions-list');
const addQuestionButton = document.getElementById('add-question');
const baseUrl = document.getElementById("baseUrl").value;
const questionsDiv = document.getElementById("questions-list");

let questionCounter = document.querySelectorAll('.question-item').length + 1;

function createQuestionElement() {
    const questionId = `new-${questionCounter++}`;
    const questionItem = document.createElement('div');
    questionItem.className = 'question-item list-group-item m-1';
    questionItem.setAttribute('data-question-id', questionId);
    questionItem.setAttribute('data-question-type', "SingleLine");
    questionItem.setAttribute('dragabble', 'true');

    questionItem.innerHTML = `
        <input type="text" name="questions[${questionId}].text" placeholder="Question text" 
               class="form-control mb-2 font-weight-bold" required>
        <select class="form-control mb-2 question-type-selector">
            <option value="SingleLine">Single Line</option>
            <option value="MultiLine">Multi Line</option>
            <option value="Integer">Integer</option>
            <option value="CheckBox">Checkbox</option>
            <option value="RadioBox">RadioBox</option>
            <option value="Phone">Phone</option>
            <option value="Date">Date</option>
            <option value="File">File</option>
        </select>
        <div class="response-container">
            <input type="text" class="form-control mb-1" placeholder="Response text" required>
        </div>
        <button class="btn btn-sm btn-warning rounded-1 text-start toggle-required">Mark Optional</button>
        <button type="button" class="btn btn-sm rounded-1 btn-danger text-end m-1 delete-question">
            <i class="bi bi-file-earmark-x"></i> Delete Question
        </button>
    `;

    questionItem.querySelector('.question-type-selector').addEventListener('change', (e) => handleQuestionTypeChange(e, questionItem));
    questionItem.querySelector('.delete-question').addEventListener('click', () => questionItem.remove());

    return questionItem;
}

function handleQuestionTypeChange(event, questionItem) {
    const responseContainer = questionItem.querySelector('.response-container');
    responseContainer.innerHTML = '';

    switch (event.target.value) {
        case 'SingleLine':
            responseContainer.innerHTML = '<input type="text" class="form-control mb-1" placeholder="Response text" required>';
            questionItem.setAttribute('data-question-type', "SingleLine");
            break;
        case 'MultiLine':
            responseContainer.innerHTML = '<textarea class="form-control mb-1" placeholder="Response text" required></textarea>';
            questionItem.setAttribute('data-question-type', "MultiLine");
            break;
        case 'Integer':
            responseContainer.innerHTML = '<input type="number" class="form-control mb-1" placeholder="Response text" required>';
            questionItem.setAttribute('data-question-type', "Integer");
            break;
        case 'CheckBox':
            responseContainer.innerHTML = `
                <div class="question-container">
                   <button type="button" class="btn btn-sm btn-secondary mb-1 add-option" data-question-id="${questionItem.getAttribute('data-question-id')}">Add Option</button>
                   <div class="new-options my-2" data-question-id="${questionItem.getAttribute('data-question-id')}"></div>
                </div>
            `;
            questionItem.setAttribute('data-question-type', "CheckBox");

            document.addEventListener('click', function (e) {
                if (e.target && e.target.classList.contains('add-option')) {
                    const button = e.target;
                    const questionId = button.getAttribute('data-question-id');
                    const container = document.querySelector(`.new-options[data-question-id="${questionId}"]`);

                    if (!container) {
                        console.error(`Container not found for questionId: ${questionId}`);
                        return;
                    }

                    addCheckboxOption(container, questionId);
                }
            });

            break;
        case 'RadioBox':
            responseContainer.innerHTML = `
              <div class="question-container">
                <button type="button" class="btn btn-sm btn-secondary mb-1 add-option" data-question-id="${questionItem.getAttribute('data-question-id')}">Add Option</button>
                <div class="new-options my-2" data-question-id="${questionItem.getAttribute('data-question-id')}"></div>
              </div>
            `;
            questionItem.setAttribute('data-question-type', "RadioBox");

            document.addEventListener('click', function (e) {
                if (e.target && e.target.classList.contains('add-option')) {
                    const button = e.target;
                    const questionId = button.getAttribute('data-question-id');
                    const container = document.querySelector(`.new-options[data-question-id="${questionId}"]`);

                    if (!container) {
                        console.error(`Container not found for questionId: ${questionId}`);
                        return;
                    }

                    addRadioboxOption(container, questionId);
                }
            });

            break;
        case "Date":
            responseContainer.innerHTML = '<input type="date" class="form-control mb-1" placeholder="Select a date" required />';
            questionItem.setAttribute('data-question-type', "Date");
            break;

        case "Phone":
            responseContainer.innerHTML = `
            <div class="input-group">
                <span class="input-group-text" id="basic-addon1">+375</span>
                <input 
                    type="tel" 
                    id="phone" 
                    name="phone" 
                    maxlength="11" 
                    pattern="[0-9]{3}-[0-9]{3}-[0-9]{3}" 
                    placeholder="123-456-789"
                    class="form-control" 
                  oninput="this.value = this.value
                                            .replace(/\D/g, '')
                                            .replace(/(\d{3})(\d{0,3})(\d{0,3})/, function(_, a, b, c) {
                                              return [a, b, c].filter(Boolean).join('-');
                                            });
                                        "
                />                               
            </div>`;
            questionItem.setAttribute('data-question-type', "Phone");
            break;


        case "File":
            responseContainer.innerHTML = `
              <div class="container-file">
                  <input type="file" id="file-input" multiple required="@(!question.IsOptional)" />
                  <label disabled id="label-file" for="file-input" name="questions[@question.QuestionId].text" value="@question.Text" class="form-control text-start mb-2 font-weight-bold">
                      <i class="fa-solid fa-arrow-up-from-bracket"></i>
                  </label>
              </div>
            `
            questionItem.setAttribute('data-question-type', "File");
            break;
    }
}


function addCheckboxOption(button, questionId) {
    const container = button.closest(".question-container");
    const optionsContainer = container.querySelector(`.new-options[data-question-id="${questionId}"]`);

    const optionCount = optionsContainer.querySelectorAll('.form-check').length + 1;
    const optionId = `${questionId}-option-${optionCount}`;

    const optionElement = document.createElement('div');
    optionElement.className = 'form-check mb-2 mt-2';
    optionElement.innerHTML = `
        <input class="form-check-input" type="checkbox" id="${optionId}">
        <input 
            class="form-control form-check-label" 
            type="text" 
            value="Option ${optionCount}" 
            placeholder="Enter option text" 
            data-option-id="${optionId}" 
            style="display: inline-block; width: auto; margin-left: 10px;"            
        />
        <button type="button"
            class="btn btn-sm btn-danger rounded-1 text-end m-1 delete-questionOption"
            data-option-id="${optionId}"
            data-question-id="${questionId}"
            >
            <i class="bi bi-file-earmark-x"></i>
        </button>
    `;
    optionsContainer.appendChild(optionElement);
}

function deleteCheckboxOption(button, questionId, optionId) {
    const optionElement = button.closest(".form-check");

    // If it's from DB (real ID), delete from backend
    if (parseInt(questionId) > 0 && optionId > 0) {
        fetch(`/Admin/Form/DeleteQuestionOption/${optionId}`, {
            method: "DELETE"
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Failed to delete option.");
                }
                return response.json();
            })
            .then(() => {
                optionElement?.remove();
            })
            .catch(error => {
                console.error("Error:", error);
                alert("Failed to delete option. Please try again.");
            });
    } else {
        // If question isn't saved yet — remove just from UI
        optionElement?.remove();
    }
}


function addRadioboxOption(button, questionId) {
    const container = button.closest(".question-container");
    const optionsContainer = container.querySelector(`.new-options[data-question-id="${questionId}"]`);

    const optionCount = optionsContainer.querySelectorAll('.form-check').length + 1;
    const optionId = `${questionId}-option-${optionCount}`;

    const optionElement = document.createElement('div');
    optionElement.className = 'form-check mb-2 mt-2';
    optionElement.innerHTML = `
        <input class="form-check-input" type="radio" id="${optionId}">
        <input 
            class="form-control form-check-label ms-2" 
            type="text" 
            value="Option ${optionCount}" 
            placeholder="Enter option text" 
            data-option-id="${optionId}" 
            style="display: inline-block; width: auto; margin-left: 10px;"
        />
        <button type="button"
             class="btn btn-sm btn-outline-danger rounded-1 text-end m-1 delete-questionOption"
             data-option-id="${optionId}"
             data-question-id="${questionId}"
             >
            <i class="bi bi-file-earmark-x"></i>
        </button>
    `;
    optionsContainer.appendChild(optionElement);
}


function deleteRadioboxOption(button, questionId, optionId) {

    // If it's from DB (real ID), delete from backend
    if (parseInt(questionId) > 0 && optionId > 0) {
        fetch(`/Admin/Form/DeleteQuestionOption/${optionId}`, {
            method: "DELETE"
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Failed to delete option.");
                }
                return response.json();
            })
            .then(() => {
                optionElement?.remove();
            })
            .catch(error => {
                console.error("Error:", error);
                alert("Failed to delete option. Please try again.");
            });
    } else {
        // If question isn't saved yet — remove just from UI
        optionElement?.remove();
    }
}

document.addEventListener("click", (e) => {
    if (e.target.classList.contains("toggle-required")) {
        const input = e.target.closest(".list-group-item").querySelector("input[type='text']");
        if (input.hasAttribute("required")) {
            input.removeAttribute("required");
            e.target.textContent = "Mark as Required";
        } else {
            input.setAttribute("required", "required");
            e.target.textContent = "Mark as Optional";
        }
    }
});

addQuestionButton.addEventListener('click', () => {
    const newQuestion = createQuestionElement();
    questionsList.appendChild(newQuestion);
});


questionsDiv.addEventListener("dragstart", (e) => {
    e.dataTransfer.setData("text/plain", e.target.dataset.questionId);
});

questionsDiv.addEventListener("dragover", (e) => {
    e.preventDefault();
});

questionsDiv.addEventListener("drop", (e) => {
    e.preventDefault();
    const draggedId = e.dataTransfer.getData("text/plain");
    const draggedElement = document.querySelector(`[data-question-id="${draggedId}"]`);
    if (draggedElement) {
        e.target.closest(".list-group-item").before(draggedElement);
    }
});

//CheckOptionsLenght
function validateCheckboxQuestions() {
    let isValid = true;
    document.querySelectorAll('.question-item[data-question-type="RadioBox"], .question-item[data-question-type="CheckBox"]').forEach(q => {
        const options = q.querySelectorAll('.new-options input[type="text"]');
        let filledOptions = 0;

        options.forEach(input => {
            if (input.value.trim() !== "") filledOptions++;
        });

        if (filledOptions < 2) {
            isValid = false;
        }
    });

    return isValid;
}

//Delete Question
document.getElementById("questions-list").addEventListener("click", (e) => {
    if (e.target.closest(".btn-danger")) {
        const questionItem = e.target.closest(".list-group-item");
        const questionId = parseInt(questionItem.dataset.questionId, 10);

        if (questionId > 0) {
            fetch(`/Admin/Form/DeleteQuestion/${questionId}`, {
                method: "DELETE",
            })
                .then((response) => {
                    if (!response.ok) {
                        throw new Error("Failed to delete question.");
                    }
                    return response.json();
                })
                .then(() => {

                    questionItem.remove();
                })
                .catch((error) => {
                    console.error("Error:", error);
                    alert("Failed to delete question. Please try again.");
                });
        } else {
            questionItem.remove();
        }
    }
});

document.getElementById('update-form').addEventListener('click', function () {
    console.log("We got inside");

    const formData = new FormData();

    formData.append('formId', parseInt(document.querySelector('[name="formId"]').value || "0"));
    formData.append('title', document.querySelector('.card-title').innerText.trim());
    formData.append('description', document.querySelector('.card-body p').innerHTML.trim());
    formData.append('templateId', parseInt(document.querySelector('[name="templateId"]').value || "0"));
    formData.append('creatorId', document.querySelector('[name="creatorId"]').value || "");


    const questionElements = document.querySelectorAll('.question-item');
    questionElements.forEach((questionElement, index) => {
        const questionId = parseInt(questionElement.dataset.questionId || "0");
        //Check input
        let input = questionElement.querySelector('input[name^="questions"]');
        let text = '';
        const type = questionElement.dataset.questionType || "Unknown";
        if (input) {
            text = input.value.trim();
        } else {
            // Special handling for "File" type questions
            if (type === "File") {
                const label = questionElement.querySelector('label[for="file-input"]');
                if (label) {
                    text = label.textContent.trim();
                }
            } else {
                console.warn('Missing question input in element:', questionElement);
            }
        }
        const isOptional = questionElement.querySelector('.toggle-required').innerText.includes("Mark Required") ? false : true;

        formData.append(`questions[${index}].questionId`, questionId);
        formData.append(`questions[${index}].text`, text);
        formData.append(`questions[${index}].type`, type);
        formData.append(`questions[${index}].order`, index + 1);
        formData.append(`questions[${index}].isOptional`, isOptional);

        // Handle options for CheckBox type
        if (type == 'CheckBox') {
            questionElement.querySelectorAll('.form-check-input').forEach((optionElement, optionIndex) => {
                const optionText = optionElement.nextElementSibling.value.trim(); // Use .value, not innerText
                const optionId = parseInt(optionElement.getAttribute('data-option-id') || "0");

                formData.append(`questions[${index}].options[${optionIndex}].optionId`, optionId);
                formData.append(`questions[${index}].options[${optionIndex}].questionId`, questionId);
                formData.append(`questions[${index}].options[${optionIndex}].text`, optionText);
            });
        }
        if (type == 'RadioBox') {
            questionElement.querySelectorAll('.form-check-input').forEach((optionElement, optionIndex) => {
                const optionText = optionElement.nextElementSibling.value.trim(); // Use .value, not innerText
                const optionId = parseInt(optionElement.dataset.optionId || "0");

                formData.append(`questions[${index}].options[${optionIndex}].optionId`, optionId);
                formData.append(`questions[${index}].options[${optionIndex}].questionId`, questionId);
                formData.append(`questions[${index}].options[${optionIndex}].text`, optionText);
            });

        }
    });
    if (!validateCheckboxQuestions()) {
        toastr.error("You must have at least 2 options for your questions of type Checkbox");
        /*e.preventDefault(); */// stop form if invalid
        return;
    }

    fetch('/Admin/Form/UpdateForm', {
        method: 'POST',
        body: formData
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to save form data.');
            }
            return response.json();
        })
        .then(data => {
            toastr.success(data.message || "Form saved successfully!");
            window.location = baseUrl + "/Admin/Form/Index";
        })
        .catch(error => {
            console.error("Error saving form: ", error);
            toastr.error("An error occurred while saving the form.");
        });

});
