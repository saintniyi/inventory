
var dataTable;
$(function () {
    loadSupplier();
});


function loadSupplier() {
    dataTable = $("#tblSupplier").DataTable({
        ajax: "/SupplierMvc/GetAll",
        columns: [
            { data: "name" },
            { data: "email" },
            { data: "phone" },
            {
                data: "id",
                render: function (data) {
                    return `
                             
                              <div class="w-100 btn-group" role="group">

                                <a href="/SupplierMvc/Upsert?id=${data}" 
                                class="btn btn-primary btn-sm mx-2"> <i class="bi bi-pencil-square"></i>&nbsp;Edit</a>

                                <a onClick=Remove('/SupplierMvc/Delete/${data}') 
                                class="btn btn-danger btn-sm mx-2"> <i class="bi bi-trash-fill"></i>&nbsp;Delete</a>

					        </div>
                        `
                },
                "width": "30%"
            }
        ]
    })
}



function Remove(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}

