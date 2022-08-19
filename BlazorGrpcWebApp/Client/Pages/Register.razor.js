export function TurnOffAutoComplete() {
    const formCollection = document.getElementsByClassName("mud-form");
    const inputCollection = document.querySelectorAll(".mud-input-slot");
    
    if (formCollection != undefined && formCollection.length > 0) {
        Array.from(formCollection).forEach(e => e.setAttribute("autocomplete", "off"));
    }

    if (inputCollection != undefined && inputCollection.length > 0) {
        inputCollection.forEach(e => e.setAttribute("autocomplete", "off"));
    }
}