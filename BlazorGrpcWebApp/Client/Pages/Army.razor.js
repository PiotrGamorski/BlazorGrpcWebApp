function GetPositionBottom() {
    const scrollHeight = document.body.scrollHeight;
    const footerHeight = 125;
    const tableBottom = document.querySelector("#table-grid").getBoundingClientRect().bottom;

    let paddingTop = scrollHeight - tableBottom - footerHeight;
    console.log(paddingTop + "px");

    document.getElementById("footer-container").style.paddingTop = paddingTop + "px";
}

export { GetPositionBottom }