function deleteItem(url) {
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
                type: 'POST', // Or 'DELETE' if your server supports it
                success: function (data) {
                    // Refresh the page or do something else on success
                    location.reload();
                },
                error: function (err) {
                    // Show an error message or do something else on failure
                    Swal.fire('Error', 'There was an error deleting the item.', 'error');
                }
            });
        }
    });
}
