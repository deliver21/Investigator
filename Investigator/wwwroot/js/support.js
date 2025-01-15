var dataTable;

$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {

    dataTable = $('#tblData').DataTable({
        "responsive": true,
        "ajax": { url: '/admin/support/getall'},
        "columns": [
            { data: 'summary', "width": "30%" },
            { data: 'priority', "width": "25%" },
            { data: 'status', "width": "25%" },
            {
                data: "jiraLink",
                "render": function (data, type, row) {
                    return
                    `
                        <a class="btn btn-info mx-2 rounded-1" href='${row.jiraLink}' target="_blank" title="View on Jira" >
                             <i class="bi bi-box-arrow-up-right"></i> Visualise ticket
                        </a>
                    `
                },
                "width": "20%"
            }
        ]
    });
}
