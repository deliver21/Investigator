var dataTable;

$(document).ready(function () {
    loadDataTable(); ``
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "responsive": true,
        "ajax": { url: '/Admin/User/GetAll' },
        "columns": [
            {
                data: "id",
                "render": function (data) {
                    return `<input class="form-check-input text-info row-checkbox" type="checkbox" value="${data}">`
                },
                "width": "5%"
            },
            {
                data: 'displayName',
                "render": function (data) {
                    return `<span class="name-field">${data}</span>`;
                },
                "width": "25%"
            },
            { data: 'userName', "width": "25%" },
            {
                data: "role",
                "render": function (data, type, row) {
                    return `<div class="btn-group" role="group" aria-label="Button group with nested dropdown">                                
                                <div class="btn-group" role="group">
                                    <button id="btnRolepDrop" type="button" class="btn btn-success rounded-1 dropdown-toggle" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">${data}</button>
                                  <div class="dropdown-menu" aria-labelledby="btnRolepDrop" style="">
                                    <a class="dropdown-item" onClick=SwitchRole('/Admin/User/SwitchRole?id=${row.id}')>${data == 'Admin' ? 'Customer' : 'Admin'}</a>
                                  </div>
                                </div>
                            </div>
                           `
                },
                "width": "20%"
            },
            {
                data: 'interval',
                "render": function (data, type, row) {
                    return `<span class="interval-field" title="Last seen: ${row.lastSeen}">${data}</span>`;
                },
                "width": "25%"
            }

        ],
        "rowCallback": function (row, data) {
            
            if (data.isBlocked) {
                $(row).addClass('blocked-row');

            }
        }
    });

    $('#flexCheckBoxes').on('change', function () {
        const isChecked = $(this).is(':checked'); 
        $('.row-checkbox').prop('checked', isChecked); 
    });
}

$(document).on('click', '#bulkDeleteButton', function () {
    const selectedIds = []; 
    
    $('.row-checkbox:checked').each(function () {
        selectedIds.push($(this).val()); 
    });

    if (selectedIds.length === 0) {
       
        toastr.warning("No users selected!");
        return;
    }

    
    Delete('/Admin/User/BulkDelete', selectedIds);
});
function SwitchRole(url) {
    Swal.fire({
        title: "Do you want to update this user role?",
        text: "You can still reconsider your choice :)",
        icon: "info",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, update it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "PUT",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}
function Delete(url, selectedIds = []) {
    
    Swal.fire({
        title: "Are you sure?",
        text: selectedIds.length > 0
            ? `You are about to delete ${selectedIds.length} user(s).` 
            : "You won't be able to revert this!", 
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            
            $.ajax({
                url: url, 
                type: selectedIds.length > 0 ? "POST" : "DELETE", 
                contentType: "application/json", 
                data: selectedIds.length > 0 ? JSON.stringify(selectedIds) : null, 
                success: function (data) {
                    location.reload(); 
                    toastr.success(data.message); 
                },
                error: function () {
                    toastr.error("Error occurred while deleting.");
                    location.reload(); 
                }
            });
        }
    });
}


$(document).on("click", "#bulkLockButton", function () {
    const selectedIds = [];
    $(".row-checkbox:checked").each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        toastr.warning("No users selected for locking!");
        return;
    }

  
    BulkAction("/Admin/User/BulkLock", selectedIds, "lock");
});

$(document).on("click", "#bulkUnlockButton", function () {
    const selectedIds = [];
    $(".row-checkbox:checked").each(function () {
        selectedIds.push($(this).val());
    });

    if (selectedIds.length === 0) {
        toastr.warning("No users selected for unlocking!");
        return;
    }

    
    BulkAction("/Admin/User/BulkUnlock", selectedIds, "unlock");
});

function BulkAction(url, selectedIds, actionName) {
    
    Swal.fire({
        title: `Are you sure?`,
        text: `You are about to ${actionName} ${selectedIds.length} user(s).`,
        icon: actionName === "unlock" ? "info" : "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: `Yes, ${actionName} them!`
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(selectedIds), 
                success: function (data) {
                    toastr.success(data.message);
                    location.reload();
                },
                error: function () {
                    toastr.error(`Error occurred while trying to ${actionName} users.`);
                    location.reload();
                }
            });
        }
    });
}