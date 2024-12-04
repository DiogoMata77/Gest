var btn = document.getElementById("loginBtn");

btn.addEventListener("click", function () {
    var email = document.querySelector(".em").value;
    var password = document.querySelector(".pw").value;
    if (email != null && password != null && email != "" && password != "") {
        $.ajax({
            type: "POST",
            data: { nome: email, senha: password },
            url: "/ValidateLogIn",
            success: function (response) {
                if (response.success) {
                    window.location.href = response.url;
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: response.url,
                        showConfirmButton: true,
                        timer: 1200
                    });
                }
            },
            error: function () {
                alert("erro");
            }
        });
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Preencha os campos corretamente!',
            showConfirmButton: true,
            timer: 950
        });
    }
});



//Create Acount
 function CreateA() {
    var nome = document.getElementById("nome").value;
    var email = document.getElementById("email").value;
    var senha = document.getElementById("senha").value;
    var senhaConfirm = document.getElementById("senhaConf").value;

    $.ajax({
        type: "POST",
        url: "/FunctionCreate",
        data: { nome: nome, email: email, senha: senha, senhaConfirm: senhaConfirm },
        success: function (response) {
            if (response.success) {
                window.location.href = response.url;
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: response.url,
                    showConfirmButton: true,
                    timer: 800
                });
            }
        }
    });
}