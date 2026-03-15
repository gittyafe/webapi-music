const uri = '/Music';
let instruments = [];
const token = localStorage.getItem("userToken");

function parseJwt(token) {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
        atob(base64)
            .split('')
            .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
    );

    return JSON.parse(jsonPayload);
}


function displayUserDetailes(){
    const userData = parseJwt(token);
    console.log(userData);
    const userId = userData.userid;

    fetch(`User/${userId}`, {
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token,
            "Accept": "application/json"
        }
    })
    .then(response => response.json())
    .then(data => {console.log(data)
        displayUser(data)})
    .catch(error => console.error("Unable to get user.", error));
}

function displayUser(user) {
    // הכנסת נתונים ל-inputs
    document.getElementById("contentName").innerText = user.name || "";
    document.getElementById("contentPwd").innerText = user.passwd || "";
    closeInput2();
    // אם הוא אדמין – מציגים קישור
    if (user.type === "Admin") {
        const adminLink = document.getElementById("users-link");
        if (adminLink.querySelector("a")) 
            return;
        const link = document.createElement("a");
        link.href= "./users.html";
        link.textContent ="to go to users";
        adminLink.append(link);
        adminLink.style.display = "inline-block";
    }
}

function displayEditFormUser() {
    ///
const userData = parseJwt(token);
    console.log(userData);
    const userId = userData.userid;

    fetch(`User/${userId}`, {
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token,
            "Accept": "application/json"
        }
    })
    .then(response => response.json())
    .then( userData=> {
////////
    document.getElementById('edit2-id').value = userData.id;
    document.getElementById('edit2-name').value = userData.name;
    document.getElementById('edit2-passwd').value = userData.passwd;

    // document.getElementById('edit-isAccompanying').checked = item.isAaccompanying;
    document.getElementById('editForm2').style.display = 'block';
    })
        .catch(error => console.error("Unable to get user.", error));

}

function updateUser() {
    
    const userId = document.getElementById('edit2-id').value;
    const user = {
        Id: parseInt(userId, 10),
        Name: document.getElementById('edit2-name').value.trim(),    
        Passwd: document.getElementById('edit2-passwd').value.trim(),
        Type: parseJwt(token).type
      };

    fetch(`User/${userId}`, {
            method: 'PUT',
            headers: {
                'Authorization': "Bearer " + token,
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        })
        .then(() => displayUserDetailes())
        .catch(error => console.error('Unable to update item.', error));

    closeInput2();

    return false;
}

function closeInput2() {
    document.getElementById('editForm2').style.display = 'none';
}























function redirectIfNeeded(){
    if(localStorage.getItem("userToken")==null){
        const currentUrl = window.location.href; // מקבל את ה-URL הנוכחי
        const newUrl = currentUrl.substring(0, currentUrl.lastIndexOf('/')); // מסיר את הקטע האחרון
        console.log(newUrl+'/login.html');
        window.location.href = newUrl+'/login.html'; // מבצע את ה-redirect  
    }
    getItems();
    displayUserDetailes();

}


function getItems() {

    fetch(uri, {
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token,
            "Accept": "application/json"
        }
    })
    .then(response => response.json())
    .then(data => _displayItems(data))
    .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');

    const item = {
        isAccompanying: false,
        name: addNameTextbox.value.trim()
    };

    fetch(uri, {
            method: 'POST',
            headers: {
                "Authorization": "Bearer " + token,
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .then(response => response.json())
        .then(() => {
            addNameTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
            method: 'DELETE',
            headers: {
                  "Authorization": "Bearer " + token,
            },
        })
        .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
    const item = instruments.find(item => item.id === id);

    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-isAccompanying').checked = item.isAaccompanying;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        isAccompanying: document.getElementById('edit-isAccompanying').checked,
        name: document.getElementById('edit-name').value.trim()
    };

    fetch(`${uri}/${itemId}`, {
            method: 'PUT',
            headers: {
                  "Authorization": "Bearer " + token,
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .catch(error => console.error('Unable to update item.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'music' : 'instruments';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('musics');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        let isAccompanyingCheckbox = document.createElement('input');
        isAccompanyingCheckbox.type = 'checkbox';
        isAccompanyingCheckbox.disabled = true;
        isAccompanyingCheckbox.checked = item.isAccompanying;

        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        td1.appendChild(isAccompanyingCheckbox);

        let td2 = tr.insertCell(1);
        let textNode = document.createTextNode(item.name);
        td2.appendChild(textNode);

        let td3 = tr.insertCell(2);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(3);
        td4.appendChild(deleteButton);
    });

    instruments = data;
}


//לבינתיים...
function initSignalR() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/activityHub", {
                accessTokenFactory: () => token
            })
                .build();
    connection.on("ReceiveActivity", function (username, action, pizzaName) {
        getItems();
        const activityList = document.getElementById("activityList");
        const li = document.createElement("li");
        li.textContent = `${username} ${action} '${pizzaName}'`;
        activityList.insertBefore(li, activityList.firstChild);

        // Keep only last 10 activities
        while (activityList.children.length > 10) {
            activityList.removeChild(activityList.lastChild);
        }
    });
    connection.start()
        .then(() => console.log("SignalR connected"))
        .catch(err => console.error("SignalR connection error:", err));
}
