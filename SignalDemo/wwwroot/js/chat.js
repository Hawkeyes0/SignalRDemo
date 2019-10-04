"use strict"

var conn = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var btn = document.getElementById('send');
btn.disabled = true;
btn.addEventListener('click', function (event) {
    var msg = {
        sender: document.getElementById('sender').value,
        message: document.getElementById('message').value,
        receiver: document.querySelector('input[name="receiver"]:checked').value
    };
    conn.invoke('SendMessage', msg).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById('join').addEventListener('click', function (event) {
    conn.invoke('JoinIn', document.getElementById('sender').value);
    $("#joinin").modal('hide');
});

conn.on('ReceiveMessage', function (pack) {
    var msg = `${pack.sender} says ${pack.message}`;
    var li = document.createElement('li');
    li.innerText = msg;
    document.getElementById('messagesList').appendChild(li);
});

conn.on('AddMember', function (roomName, userName) {
    if (userName === document.getElementById('sender').value)
        return;
    var id = roomName ? 'users-' + roomName : 'users';
    var list = document.getElementById(id);
    var item = list.appendChild(document.createElement('li'));
    var el = item.appendChild(document.createElement('input'));
    el.value = userName;
    el.type = 'radio';
    el.name = 'receiver';
    item.appendChild(document.createElement('div')).innerText = userName;
});

conn.start().then(function () {
    btn.disabled = false;
}).catch(err=>console.error(err));

$("#joinin").modal('show');