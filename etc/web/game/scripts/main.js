document.addEventListener('DOMContentLoaded',function(){
    logOut();
})

function logOut() {
    const logOutBtn = document.querySelector('.log-out-btn');
    logOutBtn.addEventListener('click',function(){
        localStorage.clear();
        window.location.href = 'https://motionjam2021.momu.co/main/';
    })
