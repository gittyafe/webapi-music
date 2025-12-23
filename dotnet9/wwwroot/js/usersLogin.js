const uri = '/User/Login';
let users = [];

// function getItems() {
//     fetch(uri)
//         .then(response => response.json())
//         .then(data => _displayItems(data))
//         .catch(error => console.error('Unable to get items.', error));
// }
const form = document.getElementById('form');

// 2. הגדרת הפונקציה שתרוץ ב-submit
function handleFormSubmit(event) {
  // מונע את רענון הדף הסטנדרטי של הטופס!
  event.preventDefault();


    const addNameTextbox = document.getElementById('passwd');
    const addPasswdTextbox = document.getElementById('name');


    const item = {
        name: addNameTextbox.value.trim(),
        passwd: addPasswdTextbox.value.trim()

    };
  
    fetch(`${uri}`, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        })
        .then(res=>res.json())
        .then(data=>{
            localStorage.setItem("userToken",data)

             // addNameTextbox.value = '';
            // addPasswdTextbox.value = '';
        })
        
        .catch(error => console.error('Unable to add item.', error));
}

form.addEventListener('submit', handleFormSubmit);

