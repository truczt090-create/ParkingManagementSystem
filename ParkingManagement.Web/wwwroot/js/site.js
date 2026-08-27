// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function closeLoginToast() {

    const toast =
        document.getElementById("login-success-toast");

    if (!toast)
        return;

    toast.classList.add("login-toast-hide");

    setTimeout(() => {
        toast.remove();
    }, 300);
}


document.addEventListener("DOMContentLoaded", function () {

    const toast =
        document.getElementById("login-success-toast");

    if (!toast)
        return;

    setTimeout(() => {

        closeLoginToast();

    }, 3500);

});
document.addEventListener("DOMContentLoaded", function () {

    const chatToggle =
        document.getElementById("chat-toggle");

    const chatBox =
        document.getElementById("chat-box");

    const chatClose =
        document.getElementById("chat-close");


    if (chatToggle && chatBox) {

        chatToggle.addEventListener("click", function () {

            chatBox.hidden = !chatBox.hidden;

            chatToggle.setAttribute(
                "aria-expanded",
                String(!chatBox.hidden)
            );

        });

    }


    if (chatClose && chatBox) {

        chatClose.addEventListener("click", function () {

            chatBox.hidden = true;

            if (chatToggle) {
                chatToggle.setAttribute(
                    "aria-expanded",
                    "false"
                );
            }

        });

    }

});