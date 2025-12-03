const uri = '/User';
let users = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');

    const item = {
        name: addNameTextbox.value.trim()
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
    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-age').value = item.age;

    // document.getElementById('edit-isAccompanying').checked = item.isAaccompanying;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId, 10),
        // isAaccompanying: document.getElementById('edit-isAccompanying').checked,
        age: document.getElementById('edit-age').value.trim(),
        name: document.getElementById('edit-name').value.trim()
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
        // let isAccompanyingCheckbox = document.createElement('input');
        // isAccompanyingCheckbox.type = 'checkbox';
        // isAccompanyingCheckbox.disabled = true;
        // isAccompanyingCheckbox.checked = item.isAccompanying;

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
        let textNode1 = document.createTextNode(item.age);
        td2.appendChild(textNode1);
        // td1.appendChild(isAccompanyingCheckbox);


        let td3 = tr.insertCell(3);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(4);
        td4.appendChild(deleteButton);
    });

    users = data;
}