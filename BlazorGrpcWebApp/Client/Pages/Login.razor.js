function TurnOffSignInButton() {
    const btn = document.getElementById("SignInBtn");
    btn.disabled = true;
}

function TurnOnSignInButton() {
    const btn = document.getElementById("SignInBtn");
    btn.disabled = false;
}

export { TurnOffSignInButton, TurnOnSignInButton }