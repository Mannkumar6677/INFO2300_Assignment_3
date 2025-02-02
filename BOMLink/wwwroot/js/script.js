// Function to toggle the theme of the website
function toggleTheme() {
    let theme = document.documentElement.getAttribute("data-bs-theme");
    let newTheme = theme === "light" ? "dark" : "light";
    document.documentElement.setAttribute("data-bs-theme", newTheme);
    document.getElementById("theme-icon").className = newTheme === "dark" ? "fa-solid fa-moon" : "fa-solid fa-sun";

}

// Function to display a temporary message
setTimeout(function () {
    var messageElement = document.getElementById('tempMessage');
    if (messageElement) {
        messageElement.style.display = 'none';
    }
}, 3000);
