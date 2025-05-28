document.getElementById("exportButton").addEventListener("click", () => {

    axios.post('/Form/Export', globalBooks, { responseType: 'blob' })
        .then(response => {
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'submissions.csv');
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        })
        .catch(error => toastr.error("Error exporting submissions:", error));
});