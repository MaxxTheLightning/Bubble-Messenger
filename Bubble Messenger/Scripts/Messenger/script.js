// Made by MaxxTheLightning, 2025

const chat = document.getElementById('chat');
const messageInput = document.getElementById('message');
const sendButton = document.getElementById('send');
const this_user = localStorage.getItem("username")
const state_text = document.getElementById("user_state")
const sidebar = document.getElementById("sidebar")
const defchat = document.getElementById("default-chat")

let selected_chat = "main-chat";

let users_list = []

users_list.push("main-chat");

function ClearMessages(name)
{
    chat.innerHTML = '';
    selected_chat = `${name}`;

    let time = new Date().toLocaleTimeString();
    const message =
    {
        type: 'info',
        name: this_user,
        opponent: selected_chat,
        text: `load history`,
        time
    };

    socket.send(JSON.stringify(message));
}

defchat.addEventListener("click", function() {
    ClearMessages("main-chat");
    console.log("main-chat");
});

// Подключение к WebSocket серверу

const socket = new WebSocket(`ws://${localStorage.getItem("ip-address")}:8080`);
    
// Обработка успешного подключения к серверу

socket.onopen = () =>
{
    addMessageToChat('chat', 'Connected to server...', 'System', new Date().toLocaleTimeString());

    const name = this_user;
    let to = "chat";
    let time = new Date().toLocaleTimeString();
    
    const message =
    {
        type: 'info',
        name,
        text: 'connected to server',
        time
    };

    // Отправка сообщения на сервер

    socket.send(JSON.stringify(message));

    state_text.innerHTML = "Online";
    state_text.style.color = "lawngreen";
};
    
// Обработка сообщений от сервера

socket.onmessage = (event) =>
{
    try
    {
        // Сервер отправляет JSON-данные

        const data = JSON.parse(event.data);
    
        const {type = '', from = 'System', to = '', time = new Date().toLocaleTimeString(), text = '', filetype = '', specialization = ''} = data;
    
        //  Ниже - обработка случайного попадания на эту страницу без логина и перенаправление на Login.html

        if (type == "error")
        {
            if (text == "Not registered")
            {
                window.location.href = "Login.html";
            }
        }
        if (type == "new connect")
        {
            if (users_list.includes(text))
            {
                let chat_with_this_user = document.getElementById(text);
                let avatar = chat_with_this_user.querySelector('.chat-avatar');

                const online_bar = document.createElement('div');
                online_bar.classList.add('isOnline');
                avatar.appendChild(online_bar);
            }

            else if (text != this_user)
            {
                const conversationDiv = document.createElement('div');
                conversationDiv.classList.add('chatlist');
                conversationDiv.id = text;
                
                const chatAvatarDiv = document.createElement('div');
                chatAvatarDiv.classList.add('chat-avatar');

                const textsDiv = document.createElement('div');
                textsDiv.classList.add('texts');

                const conversationName = document.createElement('p');
                conversationName.classList.add('chatlist-name');
                conversationName.textContent = text;

                const lastMsg = document.createElement('p');
                lastMsg.classList.add('last-message');
                lastMsg.textContent = '...';

                conversationDiv.appendChild(chatAvatarDiv);
                textsDiv.appendChild(conversationName);
                textsDiv.appendChild(lastMsg);
                conversationDiv.appendChild(textsDiv);
                sidebar.appendChild(conversationDiv);

                conversationDiv.addEventListener("click", function() {
                    ClearMessages(text);
                    console.log(text);
                });

                let chat_with_this_user = document.getElementById(text);
                let avatar = chat_with_this_user.querySelector('.chat-avatar');

                const online_bar = document.createElement('div');
                online_bar.classList.add('isOnline');
                avatar.appendChild(online_bar);

                users_list.push(text);
            } 
        }
        if (type == "history")
        {
            console.log("History.");
            test_text = text;
            if (specialization == this_user)
            {
                const old_msg_data = JSON.parse(test_text);
    
                const {type = '', from = 'System', to = '', time = new Date().toLocaleTimeString(), text = '', filetype = ''} = old_msg_data;
                new_text = text;

                addMessageToChat(type, new_text, from, time);
            }
        }
        if (type == "users-history")
        {
            const [uz_text, state] = text.split(/\s/, 2);
            if (specialization == this_user && uz_text != this_user)
            {
                const conversationDiv = document.createElement('div');
                conversationDiv.classList.add('chatlist');
                conversationDiv.id = uz_text;
                    
                const chatAvatarDiv = document.createElement('div');
                chatAvatarDiv.classList.add('chat-avatar');

                const textsDiv = document.createElement('div');
                textsDiv.classList.add('texts');

                const conversationName = document.createElement('p');
                conversationName.classList.add('chatlist-name');
                conversationName.textContent = uz_text;

                const lastMsg = document.createElement('p');
                lastMsg.classList.add('last-message');
                lastMsg.textContent = '...';

                conversationDiv.appendChild(chatAvatarDiv);
                textsDiv.appendChild(conversationName);
                textsDiv.appendChild(lastMsg);
                conversationDiv.appendChild(textsDiv);
                sidebar.appendChild(conversationDiv);

                conversationDiv.addEventListener("click", function() {
                    ClearMessages(uz_text);
                    console.log(uz_text);
                });

                if (state == "True")
                {
                    const online_bar = document.createElement('div');
                    online_bar.classList.add('isOnline');
                    chatAvatarDiv.appendChild(online_bar);
                }

                users_list.push(uz_text);
            }
        }
        if (type == "user_disconnected")
        {
            let disconnected_user = document.getElementById(text);
            let avatar = disconnected_user.querySelector('.chat-avatar');
            avatar.innerHTML = '';
        }
        if (type == "chat", (from == this_user && to == selected_chat) || (from == selected_chat && to == this_user) || to == selected_chat)
        {
            addMessageToChat(type, text, from, time);
        }
    } 
    catch (error)
    {
        console.error('Invalid message from server:', event.data, error);
        addMessageToChat('chat', event.data, 'Server', new Date().toLocaleTimeString());
    }
};
    
// Обработка закрытия соединения

socket.onclose = () =>
{
    addMessageToChat('chat', 'Disconnected from server...', 'System', new Date().toLocaleTimeString());
    state_text.innerHTML = "Offline"
    state_text.style.color = "red"
};

//  Ниже - обработка закрытия страницы и отправка на сервер сообщения об отключении

window.addEventListener("unload", function() {
    const time = new Date().toLocaleTimeString();
    const name = `${this_user}`
    const message =
    {
        type: 'info',
        name,
        text: `disconnected from server`,
        time
    };

    socket.send(JSON.stringify(message));
});
    
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
        const from = this_user;
        let to = selected_chat;
        const message =
        {
            type: 'chat',
            from,
            to,
            text,
            time,
            filetype: "text"
        };
    
        // Отправка сообщения на сервер

        socket.send(JSON.stringify(message));
    
        // Очистка поля ввода

        messageInput.value = '';

        if(text.startsWith("/chatGPT "))
        {
            ChatGPT()
        }
    }
}
    
// Функция для добавления сообщения в чат

function addMessageToChat(type, text, name, time)
{
    if (type == "chat")
    {
        let type_of_message = '';
        if (name == this_user)          //  Если имя совпадает с именем этого пользователя, то значит сообщение было им отправлено
        {
            type_of_message = "sent";
        }
        else
        {
            type_of_message = "received";        //  Иначе - сообщение было получено
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
                'X-RapidAPI-Key': '644f580fa1msh7b4949b883fa3f1p196caejsn6190c076107a',
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
                type: 'chat',
                name: 'ChatGPT',
                text,
                time
            };
            socket.send(JSON.stringify(chatGPT_message));
        }
        catch (error) {
            console.error(error);
        }
    }
    input.value = "";
}

//  Получаем данные об имени и IP со страницы логина

document.addEventListener("DOMContentLoaded", function() {
    let username = localStorage.getItem("username");
    if (!username)
    {
        window.location.href = "Login.html";
    }
    else
    {
        document.getElementById("username_field").textContent = username;
    }

    let ip_address = localStorage.getItem("ip-address")
    if (!ip_address)
    {
        window.location.href = "Login.html";
    }
    else
    {
        document.getElementById("ip-address").textContent = ip_address;
    }
});