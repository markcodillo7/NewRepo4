// ===============================
// GLOBAL ADMIN INPUT VALIDATION
// ===============================

// Highlight invalid fields
function setInvalid(input, message) {
    input.classList.add("input-error");

    let error = input.parentElement.querySelector(".error-text");
    if (!error) {
        error = document.createElement("span");
        error.classList.add("error-text");
        input.parentElement.appendChild(error);
    }
    error.textContent = message;
}

function setValid(input) {
    input.classList.remove("input-error");

    let error = input.parentElement.querySelector(".error-text");
    if (error) error.remove();
}

// Check if empty
function validateRequired(input) {
    if (!input.value.trim()) {
        setInvalid(input, "This field is required.");
        return false;
    }
    setValid(input);
    return true;
}

// Email format
function validateEmail(input) {
    const pattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!pattern.test(input.value.trim())) {
        setInvalid(input, "Enter a valid email address.");
        return false;
    }
    setValid(input);
    return true;
}

// Number-only fields
function validateNumber(input) {
    if (isNaN(input.value) || input.value.trim() === "") {
        setInvalid(input, "Enter a valid number.");
        return false;
    }
    setValid(input);
    return true;
}

// Date validation
function validateDate(input) {
    if (!input.value) {
        setInvalid(input, "Please select a valid date.");
        return false;
    }
    setValid(input);
    return true;
}

// Attach validation rules
function attachValidation() {
    document.querySelectorAll("[data-required]").forEach(input => {
        input.addEventListener("blur", () => validateRequired(input));
    });

    document.querySelectorAll("[data-email]").forEach(input => {
        input.addEventListener("blur", () => validateEmail(input));
    });

    document.querySelectorAll("[data-number]").forEach(input => {
        input.addEventListener("blur", () => validateNumber(input));
    });

    document.querySelectorAll("[data-date]").forEach(input => {
        input.addEventListener("change", () => validateDate(input));
    });
}

// Prevent double submit
function preventDoubleSubmit() {
    document.querySelectorAll("form").forEach(form => {
        form.addEventListener("submit", function (e) {
            const submitBtn = form.querySelector("button[type='submit']");
            submitBtn.disabled = true;
            submitBtn.textContent = "Processing...";

            setTimeout(() => {
                submitBtn.disabled = false;
                submitBtn.textContent = submitBtn.dataset.originalText || "Submit";
            }, 3000);
        });
    });
}

// Auto formatting for contact number
function autoFormatContact() {
    const contact = document.querySelector("[data-contact]");
    if (!contact) return;

    contact.addEventListener("input", () => {
        contact.value = contact.value.replace(/[^\d]/g, "").substring(0, 11);
    });
}

// Master init
document.addEventListener("DOMContentLoaded", function () {
    attachValidation();
    preventDoubleSubmit();
    autoFormatContact();
});
