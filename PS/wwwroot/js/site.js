function fire(message) {
    return new Promise(() => { Swal.fire(message); });
}

function sweetConfirm(title, message, type, confirmText, cancelText, confirmButtonColor, cancelButtonColor) {
    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            html: message,
            icon: type,
            confirmButtonColor: confirmButtonColor,
            cancelButtonColor: cancelButtonColor,
            showCancelButton: true,
            confirmButtonText: confirmText,
            cancelButtonText: cancelText
        }).then((result) => {
            resolve(result.isConfirmed);
        });
    });
}
