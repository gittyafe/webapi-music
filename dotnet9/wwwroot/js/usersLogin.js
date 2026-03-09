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
async function handleFormSubmit(event) {
    // מונע את רענון הדף הסטנדרטי של הטופס!
    event.preventDefault();


    const addNameTextbox = document.getElementById('name');
    const addPasswdTextbox = document.getElementById('passwd');
    const name = addNameTextbox.value.trim();
    const passwd = addPasswdTextbox.value.trim();
    let item, item2;
    await fetch("/User")
        .then(res => res.json())
        .then(json => {
            console.log(json);
            item = json.find(x => x.name == name && x.passwd == passwd);
            console.log(item);
            debugger;
            item2 = {
                Id: item.id,
                Name: item.name,
                Passwd: item.passwd,
                Type: item.type
            }
            if (item == null) {
                console.log("could not find user");
                return;
            }
            console.log(item);
        });



    await fetch(`${uri}`, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item2)
    })
        .then(res => res.text())
        .then(data => {
            console.log(data);
            const cleanToken = data.replace(/"/g, ""); // מסיר מרכאות
            localStorage.setItem("userToken", cleanToken);

            // addNameTextbox.value = '';
            // addPasswdTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));


    const currentUrl = window.location.href; // מקבל את ה-URL הנוכחי
    const newUrl = currentUrl.substring(0, currentUrl.lastIndexOf('/')); // מסיר את הקטע האחרון
    console.log(newUrl + '/login.html');
    window.location.href = newUrl + '/index.html'; // מבצע את ה-redirect

}


form.addEventListener('submit', handleFormSubmit);

