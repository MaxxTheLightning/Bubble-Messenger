// Made by MaxxTheLightning, 2025
function login() {
    let username = document.getElementById('username_place').value;
    if (username.trim() === "") {
        alert("Enter username");
        return;
    }
    localStorage.setItem("username", username);

    let ip = document.getElementById('ip-address').value;
    if (ip.trim() === "") {
        alert("Enter IP-Address");
        return;
    }
    localStorage.setItem("ip-address", ip);

    window.location.href = "Bubble.html";
}