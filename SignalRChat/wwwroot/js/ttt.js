"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

if (localStorage) {
    if (localStorage.getItem("username") === null) {
        const overlay = document.createElement("div");
        overlay.id = "henk";
        overlay.className = "fixed w-screen h-screen z-[1000] bg-black/50 backdrop-blur flex top-0 left-0 justify-center items-center";
        overlay.innerHTML = `
        <div class="flex justify-center items-center flex-col rounded-xl space-y-4 h-96 w-[35rem] bg-gradient-to-br from-gray-500 to-gray-900">
            <h1 class="text-2xl text-white font-semibold pb-6">Enter Username</h1>
            <input id="username" type="text" class="text-gray-900 outline-none rounded-xl w-1/2 p-2" placeholder="Username"/>
            <button class="text-white p-2 w-48 rounded-xl bg-blue-600" id="continue">Ga door</button>
        </div>
        `;
        document.querySelector("body").appendChild(overlay);

        document.querySelector("#continue").addEventListener("click", (e) => {
            const inputvalue = document.querySelector("#username").value;

            updateUsername(inputvalue);
            document.querySelector("#henk").remove();
        });
    }
}

function updateUsername(username) {
    localStorage.setItem("username", username);
    document.querySelector("#welcome-user").innerHTML = `Welcome ${localStorage.getItem("username") ?? "User"}`;
}
connection.start().then(function () {
    updateSessionList();

    const username = localStorage.getItem("username");

    console.log('USNR')
    if (username === null) {
        return;
    }

    console.log("REEEE")
    connection.invoke("getSessionFromUser", username).catch(function (err) {
        return console.error(err.toString());
    }).then((e) => {
        const session = e;
        console.log("reee", session);
        if (e == null) {
            return;
        }

        if (session.player_1 == username || session.player_2 == username) {
            window.location.href = "/game/" + session.name;
        }
    })
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("sessionUpdated", function () {
    updateSessionList();
});

connection.on("matchStart", function (args) {
    const session = args.session;

    const username = localStorage.getItem("username");

    if (username === null) {
        createAlert("You are not signed in.");
        return;
    }

    if (session.player_1 == username || session.player_2 == username) {
        window.location.href = "/game/" + session.name;
    }
});

function updateSessionList() {
    connection.invoke("getSessions").then(function (sessions) {
        const sessionContainer = document.querySelector("#session_list");
        sessionContainer.innerHTML = '';

        sessions.forEach(session => {
            const child = document.createElement("div");
            child.className = "px-2 py-3 rounded-xl bg-stone-100 max-w-96";
            child.innerHTML = `<p>Session: ${session.name}, Status: ${session.player_2 ? 'In-Game' : 'Waiting...'}</p>`;
            sessionContainer.appendChild(child);
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

document.getElementById("join_session").addEventListener("click", function (event) {
    var session = prompt("Enter session name to join:");

    if (session) {
        const username = localStorage.getItem("username");

        if (username === null) {
            return;
        }

        connection.invoke("joinSession", session, username).catch(function (err) {
            return console.error(err.toString());
        }).then((e) => {
            createAlert(e.response);

            document.getElementById("leave_session").disabled = false;
        })
    }
});

document.getElementById("leave_session").addEventListener("click", function (event) {
    console.log("trying to leave ")
    const username = localStorage.getItem("username");

    if (username === null) {
        return;
    }
    
    connection.invoke("getSessionFromUser", username).catch(function (err) {
        return console.error(err.toString());
    }).then((e) => {
        console.log("getSess", e)
        if (e == null) {
            createAlert("You are not in a session.");
            return;
        }

        connection.invoke("leaveSession", e.name, username).catch(function (err) {
            return console.error(err.toString());
        }).then((e) => {
            console.log("Leave", e)
            createAlert("Session left.")
        })
    })
});

document.getElementById("create_session").addEventListener("click", function (event) {
    var name = prompt("Enter your name to create a session:");
    if (name) {
        const username = localStorage.getItem("username");
        
        if (username === null) {
            return;
        }
        
        connection.invoke("createSession", name, username).catch(function (err) {
            return console.error(err.toString());
        }).then((e) => {
            createAlert(e.response);

            document.getElementById("leave_session").disabled = false;
        })
    }
});

function createAlert(message) {
    let alerts = document.getElementById("alert-container");

    if (alerts.childElementCount < 2) {
        let alertBox = document.createElement("div");
        alertBox.className = "absolute bg-stone-100 rounded-lg min-w-80 border border-gray-300 p-3.5 absolute bottom-6 right-6 shadow-md z-50";

        let alertMsg = document.createTextNode(message);
        alertBox.appendChild(alertMsg);

        alerts.insertBefore(alertBox, alerts.childNodes[0]);

        setTimeout(function () {
            alerts.removeChild(alerts.lastChild);
        }, 3500);
    }
}
