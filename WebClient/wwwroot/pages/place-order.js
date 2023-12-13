function orderFormValid() {
    $("[data-val-required]").valid();
    let hasErrors = false;

    document
        .querySelectorAll("[data-dropdown]")
        .forEach(dropdown => {
            dropdown.removeAttribute("open");
            const summary = dropdown.querySelector("summary");

            let validationError = dropdown.querySelector(".validation-message.field-validation-error");
            if (!validationError) {
                summary.classList.add("strike-through");
                summary.classList.add("text-gray-nurse-300");
                summary.classList.remove("text-zuccini-950");
                return;
            }

            dropdown.setAttribute("open", "true");
            summary.classList.remove("strike-through");
            summary.classList.remove("text-gray-nurse-300");
            summary.classList.add("text-zuccini-950");             
            hasErrors = true;
        });

    return hasErrors ? false : true;
}

document.body.addEventListener('htmx:afterSettle', (evt) => {
    reloadOrderFormValidators();
});

function reloadOrderFormValidators() {
    let newForm = $('#orderForm');
    newForm.removeData("validator");
    newForm.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(newForm);
} 

function applyShippingMethodCountrySelected() {
    const dropdown = document.getElementById("ShippingAddress_CountryName");
    const shippingMethod = document.getElementById('shippingMethodContainer');
    let url = shippingMethod.getAttribute("hx-get");
    url = url.split("&countryName=")[0];
    url += `&countryName=${dropdown.value}`;
    shippingMethod.setAttribute("hx-get", url);

    htmx.process(shippingMethod);
    htmx.trigger("#shippingMethodContainer", "country-changed");
}