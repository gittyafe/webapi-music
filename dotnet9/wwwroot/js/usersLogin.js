
const form = document.getElementById('form');

async function handleFormSubmit(event) {
    event.preventDefault();

    const name = document.getElementById('name').value.trim();
    const passwd = document.getElementById('passwd').value.trim();
    const loginData = {
        Name: name,
        Passwd: passwd
    };

    const res = await fetch("/Login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(loginData)
    });

    if (!res.ok) {
        alert("שם משתמש או סיסמה שגויים");
        return;
    }

    const token = (await res.text()).replace(/"/g, "");
    localStorage.setItem("userToken", token);

    // מעבר לעמוד הראשי
    const currentUrl = window.location.href;
    const newUrl = currentUrl.substring(0, currentUrl.lastIndexOf('/'));
    window.location.href = newUrl + '/index.html';
}

form.addEventListener('submit', handleFormSubmit);

// Google Sign-In callback for SPA flow
async function handleGoogleCredentialResponse(response) {
    const idToken = response.credential;
    // שלח את ה-ID Token לשרת שלך
    const res = await fetch('Auth/google-login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ idToken })
    });
    if (!res.ok) {
        alert('Google login failed');
        return;
    }
    const token = (await res.text()).replace(/"/g, "");
    localStorage.setItem('userToken', token);
    // מעבר לעמוד הראשי
    const currentUrl = window.location.href;
    const newUrl = currentUrl.substring(0, currentUrl.lastIndexOf('/'));
    window.location.href = newUrl + '/index.html';
}
