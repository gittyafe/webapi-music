const uri = '/User';
let users = [];

function redirectIfNeeded(){
    if(localStorage.getItem("userToken")==null){
        const currentUrl = window.location.href; // מקבל את ה-URL הנוכחי
        const newUrl = currentUrl.substring(0, currentUrl.lastIndexOf('/')); // מסיר את הקטע האחרון
        window.location.href = newUrl+'/login.html'; // מבצע את ה-redirect
    }
    else{
        console.log("the user has token");
        getItems();
    }
}


function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');
    const addPasswdTextbox = document.getElementById('add-passwd');
    const addTypeTextbox = document.getElementById('add-type');


    const item = {
        name: addNameTextbox.value.trim(),
        passwd: addPasswdTextbox.value.trim(),
        type: addTypeTextbox.value.trim()
    };

    fetch(uri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .then(response => response.json())
        .then(() => {
            getItems();
            addNameTextbox.value = '';
            addPasswdTextbox.value = '';
            addTypeTextbox.value = '';

        })
        .catch(error => console.error('Unable to add item.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
            method: 'DELETE'
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
    const item = users.find(item => item.id === id);
    
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-type').value = item.type;
    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-passwd').value = item.passwd;

    // document.getElementById('edit-isAccompanying').checked = item.isAaccompanying;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        Id: parseInt(itemId, 10),
        Name: document.getElementById('edit-name').value.trim(),    
        Passwd: document.getElementById('edit-passwd').value.trim(),
        Type: document.getElementById('edit-type').value.trim(),
      };

    fetch(`${uri}/${itemId}`, {
            method: 'PUT',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to update item.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'user' : 'users';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('usrs');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {

        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let tr = tBody.insertRow();

       let td0 = tr.insertCell(0);
        let textNode0 = document.createTextNode(item.id);
        td0.appendChild(textNode0);

        let td1 = tr.insertCell(1);
        let textNode = document.createTextNode(item.name);
        td1.appendChild(textNode);


        let td2 = tr.insertCell(2);
        let textNode1 = document.createTextNode(item.passwd);
        td2.appendChild(textNode1);

        let td3 = tr.insertCell(3);
        let textNode2 = document.createTextNode(item.type);
        td3.appendChild(textNode2);


        let td4 = tr.insertCell(4);
        td4.appendChild(editButton);

        let td5 = tr.insertCell(5);
        td5.appendChild(deleteButton);

    });

    users = data;
}