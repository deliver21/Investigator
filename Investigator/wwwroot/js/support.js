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
                render: function (data) {
                    return data
                },
                "width": "20%"
            }
        ]
    });
}

//<div class="w-75 btn-group" role="group">
//    < a class="help-link p-2 m-1 rounded-1 shadow border-1" href="${data}" target="_blank" title="View on Jira" >
//        <img src="~/Images/Icon/jira.png" style="width:15x;height:15px" />
//    </a >
//</div>   