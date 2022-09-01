function NavigateToLoginPage() {
    const origin = window.location.origin;
    window.location.href = `${origin}/login`;
}

function TurnOffVerifyButton() {
    const btn = document.getElementById("VerifyBtn");
    btn.disabled = true;
}

function TurnOnVerifyButton() {
    const btn = document.getElementById("VerifyBtn");
    btn.disabled = false;
}

export { NavigateToLoginPage, TurnOffVerifyButton, TurnOnVerifyButton }