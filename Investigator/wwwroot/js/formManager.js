var manageQuestions = document.getElementById("manageQuestions").value;
var editFormHeader = document.getElementById("editFormHeader").value;
var deleteForm = document.getElementById("deleteForm").value;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("allForm")) {
        loadDataTable("allForm");
    }
    else {
        loadDataTable("ownForm");
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "responsive": true,
        "ajax": { url: '/admin/form/getall?status=' + status },
        "columns": [
            { data: 'title', "width": "40%" },
            {
                data: "formId",
                "render": function (data) {
                    return `<div class="w-75 btn-group text-center" role="group">
                           <a id="templateField" href="#" class="btn btn-info mx-2 rounded-1" title="${manageQuestions}">
                               <i class="bi bi-info-circle-fill"></i> 
                           </a>                                     
                    </div>`
                },
                "width": "20%"
            },
            {
                data: "formId",
                "render": function (data) {
                    return `<div class="w-75 btn-group text-center" role="group">
                           <a id="templateField" href="#" class="btn btn-success mx-2 rounded-1" title="${manageQuestions}">
                               <i class="bi bi-pencil-square"></i>
                           </a>                                    
                    </div>`
                },
                "width": "20%"
            },
            {
                data: "formId",
                "render": function (data) {
                    return `<div class="w-75 btn-group text-center" role="group">
                           <a id="templateField" onClick="Delete('/Admin/Form/Delete?id=${data}')" class="btn btn-danger mx-2 rounded-1" title="${deleteForm}">
                               <i class="bi bi-trash-fill"></i>
                           </a>                                  
                    </div>`
                },
                "width": "20%"
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