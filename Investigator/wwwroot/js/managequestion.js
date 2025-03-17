let questionId = 0;
const baseUrl = document.getElementById("baseUrl");
// Add new question dynamically
document.getElementById("addQuestion").addEventListener("click", () => {
    const questionsDiv = document.getElementById("questions");
    const questionHtml = `
        <div class="list-group-item m-2" data-question-id="0" draggable="true">
            <input type="text" name="questions[${questionId}].text" placeholder="Question text" 
                   class="form-control mb-2 font-weight-bold" required>
            <select name="questions[${questionId}].type" class="form-select">
                <option value="SingleLine">Single Line</option>
                <option value="MultiLine">Multi Line</option>
                <option value="Integer">Integer</option>
                <option value="CheckBox">CheckBox</option>
            </select>
            <button type="button" class="btn btn-sm btn-warning m-1 text-start toggle-required">Mark as Optional</button>            
            <button type="button" class="btn btn-sm btn-danger text-end m-1">
                <i class="bi bi-file-earmark-x"></i> Delete Question
            </button>
        </div>`;
    questionsDiv.insertAdjacentHTML("beforeend", questionHtml);
    questionId++;
});
//Delete Question
document.getElementById("questions").addEventListener("click", (e) => {
    if (e.target.closest(".btn-danger")) {
        const questionItem = e.target.closest(".list-group-item");
        const questionId = parseInt(questionItem.dataset.questionId, 10);

        if (questionId > 0) {            
            fetch(`/DeleteQuestion/${questionId}`, {
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
// Toggle required/optional
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
// Drag-and-Drop functionality for ordering
const questionsDiv = document.getElementById("questions");
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

// Save questions
document.getElementById("saveQuestions").addEventListener("click", () => {
    const templateId = document.getElementById("templateId").value;
    if (!templateId) {
        alert("Please save the template first.");
        return;
    }

    const questions = [...document.querySelectorAll("#questions .list-group-item")].map((item, index) => ({
        questionId: parseInt(item.dataset.questionId) || 0,
        text: item.querySelector("input[type='text']").value,
        type: item.querySelector("select").value,
        order: index,
        isOptional: !item.querySelector("input[type='text']").hasAttribute("required")
    }));

    if (questions.length < 1) {
        toastr.success("Oops, there are no questions to add to this template.");
        return;
    }

    fetch(`/SaveQuestions/${templateId}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(questions)
    })
    .then((response) => {
        if (!response.ok) throw new Error("Failed to save questions.");
        return response.json();
    })
        .then(            
            (data) => {
                window.location = baseUrl + "/Template/Index";
                toastr.success(data.message || "Questions saved successfully!");
            }
        )
    .catch((error) => {
        console.error("Error:", error);
        toastr.error("An error occurred while saving questions.");
    });
});