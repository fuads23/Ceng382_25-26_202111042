// Forming array
const userLogins = [];

// Select login button
const loginButton = document.querySelector("button");

// Login butonuna tıklanınca çalışacak fonksiyon
loginButton.addEventListener("click", function () {
    // User name and password field
    const usernameInput = document.querySelector("input[type='text']");
    const passwordInput = document.querySelector("input[type='password']");

    // Receive username and password 
    const username = usernameInput.value.trim();
    const password = passwordInput.value.trim();

    // If username and password are not empty save them
    if (username && password) {
        userLogins.push({ username, password });

        // Write user inputs to console
        console.log("All user inputs:");
        console.table(userLogins);

        // Clean input areas
        usernameInput.value = "";
        passwordInput.value = "";
    } else {
        console.warn("Enter username and password!");
    }
});

// Live clock
function updateClock() {
    const clockElement = document.getElementById("liveClock");
    
    if (clockElement) {
        const now = new Date();
        const hours = now.getHours().toString().padStart(2, "0");
        const minutes = now.getMinutes().toString().padStart(2, "0");
        const seconds = now.getSeconds().toString().padStart(2, "0");

        clockElement.textContent = `${hours}:${minutes}:${seconds}`;
    }
}

// In every second update the clock and update when page loaded
document.addEventListener("DOMContentLoaded", () => {
    updateClock(); // First run
    setInterval(updateClock, 1000); // Update every second
});

// Toggle form visibility with 'H' key
document.addEventListener("keydown", function (event) {
    if (event.key.toLowerCase() === "h") {  // 'H' tuşuna basıldığında
        const form = document.querySelector(".pacman-login");
        if (form) {
            if (form.style.display === "none") {
                form.style.display = "block";
            } else {
                form.style.display = "none";
            }
        }
    }
});