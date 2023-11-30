function updateInputValueFromValue(inputFrom, toId) {
    const to = document.getElementById(toId);
    to.value = inputFrom.value;
}

function updateInputValueFromCheckbox(inputFrom, toId) {
    const to = document.getElementById(toId);
    if (!to) return;

    let currentValues = to.value.trim() !== '' ? to.value.split(",") : [];

    if (inputFrom.checked)
        currentValues.push(inputFrom.value);
    else
        currentValues = currentValues.filter(x => x !== inputFrom.value);

    to.value = currentValues.join(",");
}