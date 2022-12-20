"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/ASPaceHub").build();

document.getElementById("sendButton").disabled = true;

var bookmark = document.getElementById("bookmark");
if(bookmark){
    bookmark.scrollIntoView();
} else {
    scrollToBottom();
}


connection.on("ReceiveMessage", function (user, message) {
    var currentUser = document.getElementById("currentUser").value;
    var li = document.createElement("li");
    li.classList.add('d-flex', `justify-content-${currentUser == user ? 'end' : 'start'}`, 'mb-4');
    li.innerHTML = `
        <div class="card chat-message">
            <div class="card-header d-flex justify-content-between p-3">
                <p class="fw-bold mb-0">${user} &nbsp;&nbsp;</p>
                <p class="text-muted small mb-0"><i class="far fa-clock"></i>${getTime()}</p>
            </div>
            <div class="card-body">
                <p class="mb-0">${message}</p>
            </div>
        </div>`
    document.getElementById("messagesList").appendChild(li);

    if(currentUser == user){
        scrollToBottom();
    }
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var receiver = document.getElementById("receiverInput").value;
    var message = document.getElementById("messageInput").value;
    document.getElementById("messageInput").value = '';
    if(message != ''){
        connection.invoke("SendMessageToGroup", receiver, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    }
});


function getTime(){
    var today = new Date();
    var date = today.getFullYear()+'/'+(today.getMonth()+1)+'/'+today.getDate();
    var time = (today.getHours() % 12) + ":" + (today.getMinutes() > 9 ? "" : "0") + today.getMinutes() + ":" + (today.getSeconds() > 9 ? "" : "0") + today.getSeconds();
    var meridian = (Math.floor(today.getHours() / 12) > 0) ? "PM" : "AM"
    return date + ' ' + time + ' ' + meridian;
}

function scrollToBottom(){
    var messagesContainer = document.getElementById("messagesList");
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}