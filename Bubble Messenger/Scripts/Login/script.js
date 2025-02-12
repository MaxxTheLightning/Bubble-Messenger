// Made by MaxxTheLightning, 2025
function login() {
    let username = document.getElementById('username_place').value;
    if (username.trim() === "") {
        alert("Please enter username");
        return;
    }
    localStorage.setItem("username", username);

    let account_password = document.getElementById('password').value;
    if (account_password.trim() === "") {
        alert("Please enter your password");
        return;
    }

    let ip = document.getElementById('ip-address').value;
    if (ip.trim() === "") {
        alert("Please enter IP-Address");
        return;
    }
    localStorage.setItem("ip-address", ip);

    const socket = new WebSocket(`ws://${ip}:8080`);
    socket.onopen = () =>
    {
        const time = new Date().toLocaleTimeString();
        const name = username; 
        const message =
        {
            type: 'info',
            name,
            password: account_password,
            text: `trying to connect`,
            time
        };
        socket.send(JSON.stringify(message));
    }

    socket.onmessage = (event) =>
    {
        try
        {
            const data = JSON.parse(event.data);
    
            const {type = "", text = ""} = data;

            if (type == "info")
            {
                if (text != "Correct.")
                {
                    alert(text);
                }
                else
                {
                    window.location.href = "Bubble.html";
                }
            }
        } 
        catch (error)
        {
            console.log(error);
        }
    };
}