function clearCheckableFilter(filterName, formName) {
    document.querySelectorAll(`[name='${filterName}']`).forEach(inpt => inpt.checked = false); 
    const formInput = document.querySelector(`[name='${formName}']`);
    formInput.value = "";
}

function applyFilter(filterFunc) {
    if (!filterFunc) return;

    filterFunc();
    document.querySelector('#searchForm button').click();
}