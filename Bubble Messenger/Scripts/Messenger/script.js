// Made by MaxxTheLightning, 2025
const chat = document.getElementById('chat');
const messageInput = document.getElementById('message');
const sendButton = document.getElementById('send');
const this_user = localStorage.getItem("username")
const state_text = document.getElementById("user_state")
    
// Подключение к WebSocket серверу
const socket = new WebSocket(`ws://${localStorage.getItem("ip-address")}:8080`);
    
// Обработка успешного подключения к серверу
socket.onopen = () =>
{
    addMessageToChat('Connected to server...', 'System', new Date().toLocaleTimeString());
    state_text.innerHTML = "Online"
    state_text.style.color = "lawngreen"
};
    
// Обработка сообщений от сервера
socket.onmessage = (event) =>
{
    try
    {
        // Предполагается, что сервер отправляет JSON-данные
        const data = JSON.parse(event.data);
    
        const {name = 'Client', text = '', time = new Date().toLocaleTimeString()} = data;
    
        // Добавляем сообщение в чат
        addMessageToChat(text, name, time);
    } 
    catch (error)
    {
        console.error('Invalid message from server:', event.data, error);
        addMessageToChat(event.data, 'Server', new Date().toLocaleTimeString());
    }
};
    
// Обработка закрытия соединения
socket.onclose = () =>
{
    addMessageToChat('Disconnected from server...', 'System', new Date().toLocaleTimeString());
    state_text.innerHTML = "Offline"
    state_text.style.color = "red"
};
    
// Обработка ошибок
socket.onerror = (error) =>
{
    console.error('WebSocket error:', error);
};
    
// Отправка сообщения
sendButton.addEventListener('click', () => sendMessage());
messageInput.addEventListener('keypress', (e) =>
{
    if (e.key === 'Enter')
    {
        sendMessage();
    }
});
    
// Функция для отправки сообщений
function sendMessage()
{
    const text = messageInput.value.trim();
    if (text)
    {
        const time = new Date().toLocaleTimeString();
        const name = `${this_user}`
        const message =
        {
            name,
            text,
            time
        };
    
        // Отправка сообщения на сервер
        socket.send(JSON.stringify(message));
    
        // Очистка поля ввода
        messageInput.value = '';    //  удалить эту строку, чтобы работал ChatGPT!!!

        if(text.startsWith("/chatGPT "))
        {
            ChatGPT()
        }
    }
}
    
// Функция для добавления сообщения в чат
function addMessageToChat(text, name, time)
{
    let type_of_message = '';
    if (name == this_user)
    {
        type_of_message = "sent"
    }
    else
    {
        type_of_message = "received"
    }
    const messageDiv = document.createElement('div');
    messageDiv.classList.add('message', type_of_message);
    
    const nameDiv = document.createElement('div');
    nameDiv.classList.add('name');
    nameDiv.textContent = name;
    
    const textDiv = document.createElement('div');
    textDiv.classList.add('text');
    textDiv.textContent = text;
    
    const timeDiv = document.createElement('div');
    timeDiv.classList.add('time');
    timeDiv.textContent = time;
    
    messageDiv.appendChild(nameDiv);
    messageDiv.appendChild(textDiv);
    messageDiv.appendChild(timeDiv);
    chat.appendChild(messageDiv);
    
    // Автопрокрутка вниз
    chat.scrollTop = chat.scrollHeight;
}

async function ChatGPT() {
    let input = document.querySelector("#message")
    if (input.value !== "" && input.value !== null && input.value.length > 0 && input.value.trim() !== "")
    {
        const url = 'https://open-ai21.p.rapidapi.com/conversationgpt35';
        const options = {
            method: 'POST',
            headers: {
                'content-type': 'application/json',
                'X-RapidAPI-Key': 'API-key',
                'X-RapidAPI-Host': 'open-ai21.p.rapidapi.com'
            },
            body: JSON.stringify({
                messages: [{
                    role: 'user',
                    content: `users new question: ${input.value}`
                }],
                web_access: false,
                system_prompt: '',
                temperature: 0.9,
                top_k: 5,
                top_p: 0.9,
                max_tokens: 512
            })
        };

        try {
            const response = await fetch(url, options);
            const result = await response.text();
            console.log(result)
            text = result.slice(11, -32)
            time = new Date().toLocaleTimeString()
            const chatGPT_message =
            {
                name: 'ChatGPT',
                text,
                time,
                type: 'received'
            };
            socket.send(JSON.stringify(chatGPT_message));
        }
        catch (error) {
            console.error(error);
        }
    }
    input.value = "";
}

document.addEventListener("DOMContentLoaded", function() {
    let username = localStorage.getItem("username");
    if (!username) {
        window.location.href = "Login.html";
    } else {
        document.getElementById("username_field").textContent = username;
    }

    let ip_address = localStorage.getItem("ip-address")
    if (!ip_address) {
        window.location.href = "Login.html";
    } else {
        document.getElementById("ip-address").textContent = ip_address;
    }
});