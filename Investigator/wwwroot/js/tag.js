var dataTable;

$(document).ready(function () {
    loadDataTable();
});
function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "responsive": true,
        "ajax": { url: '/admin/tag/getall'},
        "columns": [
            { data: 'tagName', "width": "40%" },
            {
                data: "tagId",
                "render": function (data) {
                    return `<a id="templateField" href="/Admin/Tag/Upsert?tagId=${data}" class="btn btn-primary mx-2 rounded-1">
                                <i class="bi bi-pencil-square"></i> 
                            </a>`
                },
                "width": "30%"
            },
            {
                data: "tagId",
                "render": function (data) {
                    return `<a id="templateField" onClick=Delete('/Admin/Tag/Delete?id=${data}') class="btn btn-danger mx-2 rounded-1">
                                 <i class="bi bi-trash-fill"></i>
                            </a>`
                },
                "width": "30%"
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