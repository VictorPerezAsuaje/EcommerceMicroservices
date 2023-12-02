function setActiveTab() {
    const button = event.target;
    document.querySelectorAll(`[data-tab-group='${button.getAttribute("data-tab-group")}']`)
        .forEach(button => {
            button.classList.remove("border-b-white");
            document.querySelector(button.getAttribute("data-tab")).setAttribute("hidden", "");
        });

    button.classList.add("border-b-white");
    document.querySelector(button.getAttribute("data-tab")).removeAttribute("hidden");
}

function addOneToInputNumber(id) {
    const input = document.getElementById(id);
    if (!(input instanceof HTMLInputElement)) return;

    let number = Number(input.value);

    if (isNaN(number))
        number = 0;

    if (!input.max) {
        input.value = number + 1;
        return;
    }

    if (Number(input.max) < Number(number + 1))
        return;

    input.value = number + 1;
}

function removeOneFromInputNumber(id) {
    const input = document.getElementById(id);
    if (!(input instanceof HTMLInputElement)) return;    

    let number = Number(input.value);

    if (isNaN(number))
        number = 0;

    if (!input.min) {
        input.value = number - 1;
        return;
    }

    if (Number(input.min) > Number(number - 1))
        return;

    input.value = number - 1;
}

function setReviewRating(starSize) {
    const input = event.target;
    if (!(input instanceof HTMLInputElement)) return;

    if (Number(input.value) < 0)
        input.value = 0;

    if (Number(input.value) > 5)
        input.value = 5;

    const reviewRating = document.getElementById("reviewRating");
    const productRating = Math.round(Number(input.value) * 10) / 10;
    reviewRating.style.width = productRating * starSize + "px";
}