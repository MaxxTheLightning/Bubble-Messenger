// Made by MaxxTheLightning, 2025
function signup() {
    let username = document.getElementById('username_place').value;
    if (username.trim() === "") {
        alert("Please enter username");
        return;
    }

    let account_password = document.getElementById('password').value;
    if (account_password.trim() === "") {
        alert("Please enter the password");
        return;
    }

    let password_confirm = document.getElementById('password_confirm').value;
    if (password_confirm.trim() === "") {
        alert("Please confirm the password");
        return;
    }

    let ip = document.getElementById('ip-address').value;
    if (ip.trim() === "") {
        alert("Please enter the server IP");
        return;
    }

    if (account_password != password_confirm)
    {
        alert("Invalid password confirm");
        return;
    }
    else
    {
        /*const socket = new WebSocket(`ws://${ip}:8080`);
        socket.onopen = () =>
        {
            const time = new Date().toLocaleTimeString();
            const name = username; 
            const message =
            {
                type: 'info',
                name,
                password: account_password,
                text: `created account`,
                time
            };
            socket.send(JSON.stringify(message));
        }*/

        const server = `http://${ip}:5141`;

        // Определяем функцию которая принимает в качестве параметров url и данные которые необходимо обработать:
        const postData = async (url = '', data = {}) => {
            // Формируем запрос
            const response = await fetch(url, {
            // Метод, если не указывать, будет использоваться GET
            method: 'POST',
            // Заголовок запроса
            headers: {
                'Content-Type': 'application/json'
            },
            // Данные
            body: JSON.stringify(data)
            });
            return response.json(); 
        }
        postData(`${server}/register`, { name: username,  password: account_password})
        .then((data) => {
            console.log(data); 
        });
    }
    alert("Account created successfully!");
    //window.location.href = "Login.html";
}