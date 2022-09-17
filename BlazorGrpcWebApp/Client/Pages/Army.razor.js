function GetPositionBottom() {
    const footerHeight = 102;
    const minPaddingTop = 21.72;
    const scrollHeight = document.body.scrollHeight;
    const tableBottomElem = document.querySelector("#table-grid");
    let tableBottom;

    if (tableBottomElem != 'undefined' && tableBottomElem != null) {
        tableBottom = tableBottomElem.getBoundingClientRect().bottom;

        let paddingTop = scrollHeight - tableBottom - footerHeight;
        if (paddingTop <= minPaddingTop) {
            paddingTop = minPaddingTop;
        }

        const footerContainerElem = document.getElementById("footer-container");
        if (footerContainerElem != 'undefined' && footerContainerElem != null) {
            document.getElementById("footer-container").style.paddingTop = paddingTop + "px";
        }

        console.log(paddingTop + "px");
    }
}

export { GetPositionBottom }