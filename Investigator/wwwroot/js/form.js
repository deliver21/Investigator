const questionsList = document.getElementById('questions-list');
const addQuestionButton = document.getElementById('add-question');

let questionCounter = document.querySelectorAll('.question-item').length + 1;

function createQuestionElement() {
    const questionId = `new-${questionCounter++}`;
    const questionItem = document.createElement('div');
    questionItem.className = 'question-item list-group-item m-1';
    questionItem.setAttribute('data-question-id', questionId);
    questionItem.setAttribute('dragabble', 'true');

    questionItem.innerHTML = `
        <input type="text" name="questions[${questionId}].text" placeholder="Question text" 
               class="form-control mb-2 font-weight-bold" required>
        <select class="form-control mb-2 question-type-selector">
            <option value="SingleLine">Single Line</option>
            <option value="MultiLine">Multi Line</option>
            <option value="Integer">Integer</option>
            <option value="CheckBox">Checkbox</option>
        </select>
        <div class="response-container">
            <input type="text" class="form-control mb-1" placeholder="Response text" required>
        </div>
        <button class="btn btn-sm btn-warning text-start toggle-required">Mark Optional</button>
        <button type="button" class="btn btn-sm btn-danger text-end m-1 delete-question">
            <i class="bi bi-file-earmark-x"></i> Delete Question
        </button>
    `;

    questionItem.querySelector('.question-type-selector').addEventListener('change', (e) => handleQuestionTypeChange(e, questionItem));
    questionItem.querySelector('.toggle-required').addEventListener('click', toggleRequired);
    questionItem.querySelector('.delete-question').addEventListener('click', () => questionItem.remove());

    return questionItem;
}

function handleQuestionTypeChange(event, questionItem) {
    const responseContainer = questionItem.querySelector('.response-container');
    responseContainer.innerHTML = '';

    switch (event.target.value) {
        case 'SingleLine':
            responseContainer.innerHTML = '<input type="text" class="form-control mb-1" placeholder="Response text" required>';
            break;
        case 'MultiLine':
            responseContainer.innerHTML = '<textarea class="form-control mb-1" placeholder="Response text" required></textarea>';
            break;
        case 'Integer':
            responseContainer.innerHTML = '<input type="number" class="form-control mb-1" placeholder="Response text" required>';
            break;
        case 'CheckBox':
            responseContainer.innerHTML = `
                <button type="button" class="btn btn-sm btn-secondary add-option" data-question-id="${questionItem.getAttribute('data-question-id')}">Add Option</button>
                <div class="new-options" data-question-id="${questionItem.getAttribute('data-question-id')}"></div>
            `;
            document.getElementById('questions-list').addEventListener('click', function (e) {
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
    }
}


function addCheckboxOption(container, questionId) {
    const optionCount = container.querySelectorAll('.form-check').length + 1;
    const optionId = `${questionId}-option-${optionCount}`;

    const optionElement = document.createElement('div');
    optionElement.className = 'form-check';
    optionElement.innerHTML = `
        <input class="form-check-input" type="checkbox" id="${optionId}">
        <label class="form-check-label" for="${optionId}">Option ${optionCount}</label>
    `;

    container.appendChild(optionElement);
}


function toggleRequired(event) {
    const button = event.target;
    button.textContent = button.textContent.includes('Optional') ? 'Mark Required' : 'Mark Optional';
}


addQuestionButton.addEventListener('click', () => {
    const newQuestion = createQuestionElement();
    questionsList.appendChild(newQuestion);
});

const questionsDiv = document.getElementById("questions-list");
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

document.getElementById('save-form').addEventListener('click', function () {
    const form = {
        formId: parseInt(document.querySelector('[name="formId"]').value || "0"),
        title: document.querySelector('.card-title').innerText.trim(),
        description: document.querySelector('.card-body p').innerHTML.trim(),
        templateId: parseInt(document.querySelector('[name="templateId"]').value || "0"),
        creatorId: document.querySelector('[name="creatorId"]').value || "",
        questions: []
    };

    const questionElements = document.querySelectorAll('.question-item');
    questionElements.forEach((questionElement, index) => {
        const questionId = parseInt(questionElement.dataset.questionId || "0"); 
        const text = questionElement.querySelector('input[name^="questions"]').value.trim();
        const type = questionElement.querySelector('input[type="text"],textarea,select,input[type="checkbox"]')?.type || "Unknown";
        const isOptional = questionElement.querySelector('.toggle-required').innerText.includes("Mark Required") ? false : true;

        const options = [];
        questionElement.querySelectorAll('.form-check-input').forEach(optionElement => {
            const optionText = optionElement.nextElementSibling.innerText.trim();
            const optionId = parseInt(optionElement.id || "0"); 
            options.push({ optionId, text: optionText });
        });

        form.questions.push({
            questionId,
            text,
            type,
            order: index + 1,
            isOptional,
            options
        });
    });

    console.log("Collected Form Data: ", JSON.stringify(form));

    fetch('/Form/save', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(form)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to save form data.');
            }
            return response.json();
        })
        .then(data => {
            alert(data.message || 'Form saved successfully!');
            console.log("API Response: ", data);
        })
        .catch(error => {
            console.error("Error saving form: ", error);
            alert("Failed to save the form. Please try again.");
        });
});
