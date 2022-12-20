"use strict";

let searchUserModal = new bootstrap.Modal(document.getElementById('searchUserModal'));

$(document).ready(function() {
    $(window).keydown(function(event){
        if(event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
});

document.getElementById("searchUserButton").addEventListener("click", createSearchUserFunction("searchUserInput"));
document.getElementById("searchUserButtonModal").addEventListener("click", createSearchUserFunction("searchUserInputModal"));

function createSearchUserFunction(inputId){
    function searchUser(event) {
        let searchUserString = document.getElementById(inputId).value;
        document.getElementById(inputId).value = '';
        if(searchUserString == '')
            return;
        
        $.ajax({
            type: "POST",
            url: '/Users/Search',
            data: { 
                str: searchUserString
            },
            dataType: 'text'
        }).done(function (response) {
            let {users, isSignedIn} = JSON.parse(response);
            let searchUsersListElem = document.getElementById("searchUsersList");
            searchUsersListElem.innerHTML = '';
            
            if(users.length == 0){
                let li = document.createElement("li");
                li.classList.add("p-2", "border-bottom");
                li.innerHTML = '<p><i>No results found</i></p>';
                searchUsersListElem.appendChild(li);
            }

            for(let user of users){
                let li = document.createElement("li");
                li.classList.add("p-2", "border-bottom", "modal-list-item-hover");
                
                let action = '';
                if(isSignedIn){
                    action = `
                        <form method="post" action="/FriendRequests/New/${user.userId}">
                            <button class="btn button-modal" title="Add Friend" type="submit"><i class="bi bi-person-fill-add"></i></button>
                        </form>`
                    if(user.isFriend && user.sentRequest){
                        action = '<p><i>Me</i></p>';
                    } else if(user.isFriend) {
                        action = '<p><i>Already Friends</i></p>';
                    } else if(user.sentRequest){
                        action = '<p><i>Friend Request Sent</i></p>';
                    }
                } 
                li.innerHTML = `
                    <div class="d-flex justify-content-between">
                        <div class="d-flex flex-row">
                            <div class="pt-1">
                                <p class="fw-bold mb-0">
                                    <a class="text-decoration-none username-link" href="/Users/Show/${user.userName}">${user.userName}</a>&nbsp;
                                    <span class="small text-muted">${user.firstName} ${user.lastName}</span>
                                </p>
                            </div>
                        </div>
                        <div class="d-flex flex-row align-items-end">${action}</div>
                    </div>`;
                searchUsersListElem.appendChild(li);
            }
            searchUserModal.show();
        });
    }
    return searchUser;
}
