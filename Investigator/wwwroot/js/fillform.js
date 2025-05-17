document.getElementById('form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const form = e.target;
    const formId = form.dataset.formId;
    const formData = new FormData();
    const answers = [];

    // Iterate through all question containers
    const questionElements = document.querySelectorAll('.question-item');
    console.log("Found questions:", questionElements.length);
    questionElements.forEach(q => {
        const questionId = parseInt(q.dataset.questionId);

        // Check for input type
        const input = q.querySelector('input, textarea, select');

        if (!input) return;

        if (input.type === 'file') {
            const fileInput = input;
            if (fileInput.files.length > 0) {
                // Append the file using key format: files_123
                formData.append(`files_${questionId}`, fileInput.files[0]);
            }
        } else if (input.type === 'checkbox') {
            const checkboxes = q.querySelectorAll('input[type="checkbox"]:checked');
            const values = Array.from(checkboxes).map(cb => cb.value);
            answers.push({
                questionId: questionId,
                answer: values.join(',') // or JSON.stringify(values)
            });
        } else {
            answers.push({
                questionId: questionId,
                answer: input.value
            });
        }
    });

    formData.append('FormId', formId);
    formData.append('Answers', JSON.stringify(answers));

   
    fetch('/Admin/Form/SubmitForm', {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) {
            toastr.succes('Form submitted successfully!');
        } else {
            toastr.error('Submission failed.');
        }
    }).then(data => {
        console.log("It did work");
        toastr.success(data.message || "Form saved successfully!");
    })
    .catch(error => {
        console.error('Submission error:', error);
        alert('An error occurred during submission.');
    });        
    
});

