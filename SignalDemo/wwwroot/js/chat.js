"use strict"

var conn = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var btn = document.getElementById('send');
btn.disabled = true;
btn.addEventListener('click', function (event) {
    var msg = {
        sender: document.getElementById('sender').value,
        message: document.getElementById('message').value
    };
    conn.invoke('SendMessage', msg).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

conn.on('ReceiveMessage', function (pack) {
    var msg = `${pack.sender} says ${pack.message}`;
    var li = document.createElement('li');
    li.innerText = msg;
    document.getElementById('messagesList').appendChild(li);
});

conn.start().then(function () {
    btn.disabled = false;
}).catch(err=>console.error(err));