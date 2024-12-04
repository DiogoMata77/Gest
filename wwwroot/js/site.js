var ca = document.getElementById("catalgo-artigos");
var cy = document.getElementById("cryptos");
var lojaURL = document.getElementById("lojaURL");
var logout = document.querySelector(".logout");
var estatisticas = document.getElementById("estatisticas");


estatisticas.addEventListener("click", function () {
    window.location.href = "/Home/Estatisticas";
});

logout.addEventListener("click", function () {
    alert(1);
    $.ajax({
        type: "GET",
        url: "/LogOut",
        success: function (data) {
            window.location.href = data;
        }
    });
});

ca.addEventListener("click", function () {
    window.location.href = "/";
});

cy.addEventListener("click", function () {
    window.location.href = "/Crypto/Index";
});
lojaURL.addEventListener("click", function () {
    window.open("https://www.vinted.pt/member/198606685-diogomaattaa");
});

//Para identificar qual a tecla clicada neste caso o enter
$("#campo-de-pesquisa").on('keyup', function (e) {
    if (e.key === 'Enter' || e.keyCode === 13) {
        var valorCampoPesquisa = document.getElementById("campo-de-pesquisa").value;
        var action = sessionStorage.getItem("action");
        if (action == "V" || action == "" || action == null) {
            action = 0;
        }
        else if (action == "C") {
            action = 1;
        }

        $.ajax({
            type: "POST",
            data: { state: action, pesquisa: valorCampoPesquisa },
            url: "/GenerateProducts",
            success: function (data) {
                document.getElementById("tableContent").innerHTML = data;
            }
        });
    }
});

function GerarProdutos(funcao) {
    if (funcao == 0) {
        sessionStorage.setItem("action", 0);
        var valorCampoPesquisa = document.getElementById("campo-de-pesquisa").value;

        //Comando para o controlador exemplo post
        $.ajax({
            type: "POST",
            data: { state: 0, pesquisa: valorCampoPesquisa },
            url: "/GenerateProducts",
            success: function (data) {
                document.getElementById("tableContent").innerHTML = data;
            },
            error: function () {
                alert("Erro, por favor tente mais tarde.");
            }
        });
    }
    else if (funcao == 1) {
        sessionStorage.setItem("action", 1);
        var valorCampoPesquisa = document.getElementById("campo-de-pesquisa").value;

        $.ajax({
            type: "POST",
            data: { state: 1, pesquisa: valorCampoPesquisa },
            url: "/GenerateProducts",
            success: function (data) {
                document.getElementById("tableContent").innerHTML = data;
            }
        });
    }
    else {
        Swal.fire({
            icon: 'error',
            title: 'Erro a executar esta ação!',
            showConfirmButton: true,
            timer: 800
        });
    }
    
}


function NomeImage(id) {
    if (document.querySelector(".contentImg").innerHTML == "") {
        document.querySelector(".contentImg").innerHTML = "<input id='img' class='inputAdd' placeholder='Digite o nome ou o caminho da imagem'/>";
        getValues(id);
    }
    else {
        document.querySelector(".contentImg").innerHTML = "";
    }
    
}

function Add() {
    Swal.fire({
        title: 'Adicionar um Artigo',
        icon: 'info',
        html: '<div style="margin-bottom: 20px; justify-content: space-between; "><p>Imagem</p><div style="justify-content: space-evenly;display: flex; "><button class="btn add" onclick="NomeImage()">Adicionar Imagem</button></div></div>'
            +'<div class="contentImg" style="margin-bottom:10px;"></div>'
            +'<div style="display:flex;" > '
            + '<div style="width=50%; margin-right: 20px;">'
            +'<p>Nome</p><input id="name" class="inputAdd"/>'
            + '<p>Destino</p><select id="destino" class="inputAdd"><option value="0">Stock</option><option value="1">Vendido</option> </select>'
            + '<p>Link do Produto</p><input id="link" class="inputAdd"/>'
            +'</div>'
            +'<div style="width=50%;">'
            + '<p>Quantidade</p><input min="0" type="number" id="quantity" class="inputAdd"/>'
            + '<p>Preço de Compra</p><input min="0" type="number" id="buing" class="inputAdd"/>'
            + '<p>Preço de Venda</p><input min="0" type="number" id="selling" class="inputAdd"/></div>'
            + '</div>',
            //+ '<script>'
            //+ 'if(sessionStorage.getItem("action") == "C" ){document.getElementById("destino").value = 0;}'
            //+ 'else if(sessionStorage.getItem("action") == "V"){document.getElementById("destino").value = 1;}'
            //+ '</script>',
        showCloseButton: true,
        showCancelButton: true,
        confirmButtonText: 'Adicionar',
    }).then((result) => {
        if (result.isConfirmed) {
            var image = document.getElementById("img");
            if (image == null) {
                image = "";
            }
            else {
                image = document.getElementById("img").value;
            }
            $.ajax({
                type: "POST",
                url: "/Add",
                data: { name: document.getElementById("name").value, mine: document.getElementById("destino").value, productLink: document.getElementById("link").value, quantity: document.getElementById("quantity").value, Img: image, buingPrice: document.getElementById("buing").value, sellingPrice: document.getElementById("selling").value },
                success: function (data) {
                    if (data == "error") {
                        location.reload();
                    }
                    else {
                        document.getElementById("tableContent").innerHTML = data;
                        Swal.fire({
                            icon: 'success',
                            title: 'Artigo adicionado!',
                            showConfirmButton: false,
                            timer: 800
                        })
                    }
                },
            });
        } 
    }) 
}

function confirm(id) {
    $.ajax({
        type: "POST",
        url: "/UpdateMine",
        data: { id: id.id },
        success: function (data) {
            document.getElementById("tableContent").innerHTML = data; 
            Swal.fire({
                icon: 'success',
                title: 'Artigo adicionado!',
                showConfirmButton: false,
                timer: 800
            });
            setTimeout(() => {
                location.reload();
            }, 800);
        },
        error: function (data) { alert(data); }
    });
}

function getValues(id) {
    $.ajax({
        type: "POST",
        url: "/getProductValues",
        data: { id: id, },
        success: function (data) {
            var Nome = data.Nome;
            var Destino = data.Destino;
            var Link = data.Link;
            var Quantidade = data.Quantidade;
            var Compra = data.Compra;
            var Venda = data.Venda;
            var Image = data.Imagem;

            document.getElementById("name").value = Nome;
            document.getElementById("destino").value = Destino;
            document.getElementById("link").value = Link;
            document.getElementById("quantity").value = Quantidade;
            document.getElementById("buing").value = Compra;
            document.getElementById("selling").value = Venda;
            if (document.querySelector(".contentImg").innerHTML != "") {
                document.getElementById("img").value = Image;
            }
        },
        error: function () { alert("erro"); }
        });
}

function edit(id) {
    id = id.id;
    getValues(id);

    Swal.fire({
        title: 'Editar Artigo',
        icon: 'info',
        html: '<div style="margin-bottom: 20px; justify-content: space-between;"><p>Imagem</p><div style="justify-content: space-evenly;display: flex; "><button class="btn add" onclick="NomeImage('+ id +')">Nome</button></div></div>'
            + '<div class="contentImg" style="margin-bottom:10px;"></div>'
            + '<div style="display:flex;" > '
            + '<div style="width=50%; margin-right: 20px;">'
            + '<p>Nome</p><input id="name" class="inputAdd"/>'
            + '<p>Destino</p><select id="destino" class="inputAdd"><option value="0">Stock</option><option value="1">Vendido</option> </select>'
            + '<p>Link do Produto</p><input id="link" class="inputAdd"/>'
            + '</div>'
            + '<div style="width=50%;">'
            + '<p>Quantidade</p><input min="0" type="number" id="quantity" class="inputAdd"/>'
            + '<p>Preço de Compra</p><input min="0" type="number" id="buing" class="inputAdd"/>'
            + '<p>Preço de Venda</p><input min="0" type="number" id="selling" class="inputAdd"/></div>'
            + '</div>',
        showCloseButton: true,
        showCancelButton: true,
        confirmButtonText: 'Edit',
    }).then((result) => {
        if (result.isConfirmed) {
            var Imagem = document.getElementById("img");
            if (document.getElementById("img") != null && document.getElementById("img") !== undefined && document.getElementById("img") !== '') {
                Imagem = document.getElementById("img").value;
            }
            else {
                Imagem = "";
            }

            $.ajax({
                type: "POST",
                url: "/UpdateData",
                data: {id:id, name: document.getElementById("name").value, mine: document.getElementById("destino").value, productLink: document.getElementById("link").value, quantity: document.getElementById("quantity").value, Img: Imagem, buingPrice: document.getElementById("buing").value, sellingPrice: document.getElementById("selling").value, function: sessionStorage.getItem("action") },
                success: function (data) {
                    if (data == "error") {
                        location.reload();
                    }
                    else {
                        document.getElementById("tableContent").innerHTML = data;
                        Swal.fire({
                            icon: 'success',
                            title: 'Artigo adicionado!',
                            showConfirmButton: false,
                            timer: 800
                        })

                    }
                },
            });
        }
    }) 
}

function apagar(id) {
    id = id.id;

    Swal.fire({
        title: 'Apagar um Artigo',
        icon: 'info',
        text: 'Tem a certeza que quer apagar este artigo?',
        showCloseButton: true,
        showCancelButton: true,
        confirmButtonText: 'Apagar',
    }).then((result) => {
        if (result.isConfirmed) {
            var action = "";
            if (!sessionStorage.getItem("action")) {
                action = "V";
            }
            else {
                action = sessionStorage.getItem("action");
            }
            $.ajax({
                type: "POST",
                url: "/Delete",
                data: { ids: "," + id, funcao: action, },
                success: function (data) {
                    if (data == "error") {
                        Swal.fire({
                            icon: 'error',
                            title: 'Ocurreu um erro!',
                            showConfirmButton: false,
                            timer: 1500
                        })
                    }
                    else {
                        document.getElementById("tableContent").innerHTML = data;
                        sessionStorage.removeItem("products");
                        Swal.fire({
                            icon: 'success',
                            title: 'Artigo apagado!',
                            showConfirmButton: false,
                            timer: 1500
                        })
                    }
                },
            });
        }
    })
}

function Delete() {
    if (!sessionStorage.getItem("products")) {
        Swal.fire({
            title: 'Erro',
            icon: 'error',
            text: 'Selecione um artigo para que ele possa ser apagado.',
            showCloseButton: true,
            showCancelButton: true,
            confirmButtonText: 'OK',
        });
    }
    else {
        Swal.fire({
            title: 'Apagar um Artigo',
            icon: 'info',
            text: 'Tem a certeza que quer apagar este artigo?',
            showCloseButton: true,
            showCancelButton: true,
            confirmButtonText: 'Adicionar',
        }).then((result) => {
            if (result.isConfirmed) {
                var action = "";
                if (!sessionStorage.getItem("action")) {
                    action = "V";
                }
                else {
                    action = sessionStorage.getItem("action");
                }
                $.ajax({
                    type: "POST",
                    url: "/Delete",
                    data: { ids: sessionStorage.getItem("products"), funcao: action, },
                    success: function (data) {
                        if (data == "error") {
                            Swal.fire({
                                icon: 'error',
                                title: 'Ocurreu um erro!',
                                showConfirmButton: false,
                                timer: 1500
                            })
                        }
                        else {
                            document.getElementById("tableContent").innerHTML = data;
                            sessionStorage.removeItem("products");
                            Swal.fire({
                                icon: 'success',
                                title: 'Artigo apagado!',
                                showConfirmButton: false,
                                timer: 1500
                            })
                        }
                    },
                });
            }
        })
    }
    
}

function selectProduct(id) {
    id = id.id;
    if (document.getElementById(id).checked == true) {
        if (sessionStorage.getItem("products") != null) {
            sessionStorage.setItem("products", sessionStorage.getItem("products") + "," + id);
        }
        else {
            sessionStorage.setItem("products", "," + id);
        }
        
        $.ajax({
            type:"POST",
            url: "/GetValues",
            data: { ids: sessionStorage.getItem("products"), },
            success: function (data) {
                document.getElementById("vVenda").innerHTML = "Valor de venda : " + data[0] + "€";
                document.getElementById("vCompra").innerHTML = "Valor de compra : " + data[1] + "€";
                document.getElementById("total").innerHTML = "Total : " + data[2] + "€";
            },
            error: function () {
                alert(1);
                document.getElementById("vVenda").innerHTML = "Valor de venda : 0€";
                document.getElementById("vCompra").innerHTML = "Valor de compra : 0€" ;
                document.getElementById("total").innerHTML = "Total : 0€" ;
            },
        });
    }
    else {
        sessionStorage.setItem("products", sessionStorage.getItem("products").replace("," + id, ""));
        $.ajax({
            type: "POST",
            url: "/GetValues",
            data: { ids: sessionStorage.getItem("products"), },
            success: function (data) {
                document.getElementById("vVenda").innerHTML = "Valor de venda : " + data[0] + "€";
                document.getElementById("vCompra").innerHTML = "Valor de compra : " + data[1] + "€";
                document.getElementById("total").innerHTML = "Total : " + data[2] + "€";
            },
            error: function () {
                document.getElementById("vVenda").innerHTML = "Valor de venda : 0€";
                document.getElementById("vCompra").innerHTML = "Valor de compra : 0€";
                document.getElementById("total").innerHTML = "Total : 0€";
            },
        });
    }
    
}

function selectAll() {
    var all = document.getElementById("all");
    var checkboxes = document.querySelectorAll(".centContent");

    if (all.checked == true) {
        checkboxes.forEach((checkbox) => {
            if (checkbox.checked == false) {
                if (sessionStorage.getItem("products") != null) {
                    sessionStorage.setItem("products", sessionStorage.getItem("products") + "," + checkbox.id);
                }
                else {
                    sessionStorage.setItem("products", "," + checkbox.id);
                }
            }
            checkbox.checked = true;
        });
        $.ajax({
            type: "POST",
            url: "/GetValues",
            data: { ids: sessionStorage.getItem("products"), },
            success: function (data) {
                document.getElementById("vVenda").innerHTML = "Valor de venda : " + data[0] + "€";
                document.getElementById("vCompra").innerHTML = "Valor de compra : " + data[1] + "€";
                document.getElementById("total").innerHTML = "Total : " + data[2] + "€";
            },
            error: function () {
                document.getElementById("vVenda").innerHTML = "Valor de venda : 0€";
                document.getElementById("vCompra").innerHTML = "Valor de compra : 0€";
                document.getElementById("total").innerHTML = "Total : 0€";
            },
        });
    }
    else {
        checkboxes.forEach((checkbox) => {
            checkbox.checked = false;
        });
        document.getElementById("vVenda").innerHTML = "Valor de venda : 0€";
        document.getElementById("vCompra").innerHTML = "Valor de compra : 0€";
        document.getElementById("total").innerHTML = "Total : 0€";
        sessionStorage.removeItem("products");
    }
}

function Vender() {
    alert("Funcionalidade em desenvolvimento");
}

// --------------------------------------------------------------------------------------------------- Despezas ---------------------------------------------------------------------------------------------------------
function AdicionarDespesa(value) {
    Swal.fire({
        title: 'Adicionar um Registo',
        icon: 'info',
        html: '<div > '
            + '<p>Descirção</p><input id="desc" class="inputAdd"/>'
            + '<p>Valor</p><input type="number" class="inputAdd" id="montante"/>'
            + '</div>',
        showCloseButton: true,
        showCancelButton: true,
        confirmButtonText: 'Adicionar',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/InsertDespeza",
                data: { valor: value, montante: document.getElementById("montante").value, descricao: document.getElementById("desc").value, },
                success: function () {
                    Swal.fire({
                        icon: 'success',
                        title: 'Artigo adicionado!',
                        showConfirmButton: false,
                        allowOutsideClick: false,
                        timer: 2000
                    });
                    setTimeout(() => {
                        window.location.reload();
                    }, 1500);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
}

function DeleteDespeza(id) {
    Swal.fire({
        title: 'Apagar Registo',
        icon: 'info',
        text: 'Tem a certeza que quer apagar este registo?',
        showCloseButton: true,
        showCancelButton: true,
        confirmButtonText: 'Delete',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/DeleteDespeza",
                data: { id: id, },
                success: function () {
                    Swal.fire({
                        icon: 'success',
                        title: 'Registo Apagado!',
                        showConfirmButton: false,
                        allowOutsideClick: false,
                        timer: 3000
                    });
                    setTimeout(() => {
                        window.location.reload();
                    }, 2000);
                },
                error: function (data) {
                    console.log(data);
                },
            });
        }
    });
}

// ------------------------------------------------------------------------------------------------------ Crypto ---------------------------------------------------------------------------------------------------

