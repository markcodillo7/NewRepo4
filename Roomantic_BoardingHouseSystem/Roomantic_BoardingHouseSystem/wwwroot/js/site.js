document.addEventListener("DOMContentLoaded", function () {

    const loginForm = document.getElementById("loginForm");
    const usernameInput = document.querySelector("input[name='Username']");
    const passwordInput = document.querySelector("input[name='Password']");
    const passwordToggle = document.getElementById("passwordToggle");

    /* ---------------------------------------------------
       PASSWORD SHOW / HIDE TOGGLE
    --------------------------------------------------- */
    passwordToggle.addEventListener("click", function () {
        const type = passwordInput.getAttribute("type") === "password" ? "text" : "password";
        passwordInput.setAttribute("type", type);

        // Change icon
        const icon = this.querySelector("i");
        icon.classList.toggle("bi-eye");
        icon.classList.toggle("bi-eye-slash");
    });

    /* ---------------------------------------------------
       SIMPLE FRONT-END VALIDATION
    --------------------------------------------------- */
    loginForm.addEventListener("submit", function (e) {

        let isValid = true;
        clearErrors();

        // Username validation
        if (usernameInput.value.trim() === "") {
            showError(usernameInput, "Username is required");
            isValid = false;
        }

        // Password validation
        if (passwordInput.value.trim() === "") {
            showError(passwordInput, "Password is required");
            isValid = false;
        }

        // Stop submit if invalid
        if (!isValid) {
            e.preventDefault();
            return;
        }

        // Disable button to prevent double click
        const loginBtn = loginForm.querySelector("button[type='submit']");
        loginBtn.disabled = true;
        loginBtn.textContent = "Please wait...";
    });

    /* ---------------------------------------------------
       HELPER FUNCTIONS
    --------------------------------------------------- */

    function showError(input, message) {
        input.classList.add("input-error");

        const errorSpan = document.createElement("div");
        errorSpan.classList.add("js-error-message");
        errorSpan.textContent = message;

        input.closest(".form-group").appendChild(errorSpan);
    }

    function clearErrors() {
        document.querySelectorAll(".input-error").forEach(el => el.classList.remove("input-error"));
        document.querySelectorAll(".js-error-message").forEach(el => el.remove());
    }

});
