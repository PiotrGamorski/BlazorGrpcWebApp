function NavigateToLoginPage() {
    const origin = window.location.origin;
    window.location.href = `${origin}/login`;
}

export { NavigateToLoginPage }