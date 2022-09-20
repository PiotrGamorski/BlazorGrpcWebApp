function SetFooterHorizontalLine(id) {
    const elem = document.getElementById(id);

    if (typeof elem !== 'undefined' && elem != null) {
        const hr = document.getElementById('footer-hr');

        if (typeof hr !== 'undefined' && hr != null) {
            hr.style.width = elem.clientWidth;
        }
    }
}

export { SetFooterHorizontalLine }