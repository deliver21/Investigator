document.getElementById("exportButton").addEventListener("click", () => {
    const formId = document.getElementById("exportButton").dataset.formId;

    axios.post(`/Admin/Form/ExportToCvs?formId=${formId}`, null, { responseType: 'blob' })
        .then(response => {
            const blob = new Blob([response.data], { type: "text/csv" });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'submissions.csv');
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        })
        .catch(error => {
            console.error("Export error:", error);
            toastr.error("Error exporting submissions. Please try again.");
        });
});
