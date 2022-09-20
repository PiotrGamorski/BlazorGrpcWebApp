function GetPositionBottom() {
    const minPaddingTop = 21.72;
    const footerHeight = 102;
    const scrollHeight = document.body.scrollHeight;
    const tableElem = document.querySelector("#table-grid");

    if (typeof tableElem !== 'undefined' && tableElem != null) {
        const tableBottom = tableElem.getBoundingClientRect().bottom;
        let paddingTop = scrollHeight - tableBottom - footerHeight;

        if (paddingTop <= minPaddingTop) {
            paddingTop = minPaddingTop;
        }

        const footerElem = document.getElementById("footer-container");
        if (typeof footerElem !== 'undefined' && footerElem != null) {
            footerElem.style.paddingTop = paddingTop + "px";
        }
    }
}

export { GetPositionBottom }