$(() => {
    loadProduct();
});


dataTable = $("#tblProduct").DataTable({
    ajax: "/Product/GetAll",
    columns: [
        { data: "name" },
        {
            data: "category",
            render: function (data) {
                switch (data) {
                    case 1: return "Electronics";
                    case 2: return "Apparel";
                    case 3: return "Groceries";
                    case 4: return "Furniture";
                    case 5: return "Books";
                    default: return "Others";
                }
            }
        },
        { data: "price" },
        { data: "stockQty" },
        { data: "supplier.name" },
        {
            data: "id",
            render: function (data) {
                //console.log("Product ID:", data); 
                return `
                    <div class="w-100 btn-group" role="group">
                        <a href="/Product/Edit?id=${data}" 
                           class="btn btn-primary btn-sm mx-2"> 
                           <i class="bi bi-pencil-square"></i>&nbsp;Edit</a>

                        <a onClick="Remove('/Product/Delete/${data}')" 
                           class="btn btn-danger btn-sm mx-2"> 
                           <i class="bi bi-trash-fill"></i>&nbsp;Delete</a>

                        <a href="/Product/DeleteServer/${data}" class="btn btn-danger btn-sm mx-2">
                            <i class="bi bi-trash-fill"></i>&nbsp;Delete
                        </a>
                    </div>
                `;
            },
            width: "40%"
        }
    ]
});





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











