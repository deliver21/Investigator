let questionId = 0;
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "responsive": true,
        "ajax": { url: '/admin/template/getall' },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'topic', "width": "25%" },
            {
                data: "templateId",
                "render": function (data) {
                    return `<div class="w-50 btn-group" role="group" style="font-size:12px">
                                    <a id="templateField" href="/Admin/Template/ManageQuestions?id=${data}" class="btn btn-primary mx-2 rounded-1">
                                        <i class="bi bi-pencil-square"></i> <span style="font-size:12px"> Manage questions</span>
                                    </a>
                                     <a id="templateField" href="/Admin/Template/Upsert?id=${data}" class="btn btn-success mx-2 rounded-1">
                                        <i class="bi bi-pencil-square"></i> <span style="font-size:12px"> Edit template header </span>
                                    </a>
                                   <a id="templateField" onClick=Delete('/Admin/Template/Delete?id=${data}') class="btn btn-danger mx-2 rounded-1">
                                        <i class="bi bi-trash-fill"></i> <span style="font-size:12px">Delete Template</span>
                                    </a>
                             </div>`
                },
                "width": "50%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}



