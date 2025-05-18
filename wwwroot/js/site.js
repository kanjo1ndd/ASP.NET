document.addEventListener('submit', e => {
    const form = e.target;
    if (form.id == "auth-model-form") {
        e.preventDefault();
        const login = form.querySelector('[name="AuthLogin"]').value;
        const password = form.querySelector('[name="AuthPassword"]').value;
        const errorField = document.getElementById("auth-error");

        errorField.textContent = "";

        if (!login || !password) {
            errorField.textContent = "Будь ласка, введіть логін і пароль.";
            return;
        }

        const credentials = btoa(login + ':' + password);
        fetch("/User/Signin", {
            method: 'GET',
            headers: {
                'Authorization': 'Basic ' + credentials
            }
        }).then(r => r.json())
            .then(j => {
                if (j.status == 200) {
                    window.location.reload();
                }
                else {
                    errorField.textContent = j.message;
                }
            })
            .catch (error => {
            errorField.textContent = "Помилка з'єднання з сервером.";
            console.error(error);
        });
        //console.log("Submit stopped");
    }

    if (form.id == "admin-category-form") {
        e.preventDefault();

        const name = form.querySelector('[name="category-name"]').value.trim();
        const description = form.querySelector('[name="category-description"]').value.trim();
        const slug = form.querySelector('[name="category-slug"]').value.trim();
        const image = form.querySelector('[name="category-image"]').files[0];

        document.getElementById("error-name").textContent = "";
        document.getElementById("error-description").textContent = "";
        document.getElementById("error-slug").textContent = "";
        document.getElementById("error-image").textContent = "";
        document.getElementById("admin-error").textContent = "";

        let hasErrors = false;

        if (!name) {
            document.getElementById("error-name").textContent = "Назва не може бути порожньою.";
            hasErrors = true;
        }
        if (!description) {
            document.getElementById("error-description").textContent = "Опис не може бути порожнім.";
            hasErrors = true;
        }
        if (!slug) {
            document.getElementById("error-slug").textContent = "Адреса (Slug) не може бути порожньою.";
            hasErrors = true;
        }
        if (!image) {
            document.getElementById("error-image").textContent = "Оберіть зображення.";
            hasErrors = true;
        }

        if (hasErrors) return;

        fetch("/Admin/AddCategory", {
            method: 'POST',
            body: new FormData(form)
        })
            .then(r => r.json())
            .then(j => {
                if (j.error) {
                    if (j.error.includes("Slug")) {
                        document.getElementById("error-slug").textContent = j.error;
                    } else {
                        document.getElementById("admin-error").textContent = j.error;
                    }
                } else {
                    console.log(j.message);
                    form.reset();
                }
            })
            .catch(error => {
                document.getElementById("admin-error").textContent = "Помилка з'єднання з сервером.";
                console.error(error);
            });
    }

    if (form.id == "admin-product-form") {
        e.preventDefault();

        const name = form.querySelector('[name="product-name"]').value.trim();
        const description = form.querySelector('[name="product-description"]').value.trim();
        const slug = form.querySelector('[name="product-slug"]').value.trim();
        const image = form.querySelector('[name="product-image"]').files[0];
        const price = form.querySelector('[name="product-price"]').value.trim();
        const stock = form.querySelector('[name="product-stock"]').value.trim();
        const category = form.querySelector('[name="category-id"]').value.trim();

        form.querySelector(".error-name").textContent = "";
        form.querySelector(".error-description").textContent = "";
        form.querySelector(".error-slug").textContent = "";
        form.querySelector(".error-image").textContent = "";
        form.querySelector(".error-price").textContent = "";
        form.querySelector(".error-stock").textContent = "";
        form.querySelector(".error-category").textContent = "";
        form.querySelector(".admin-error").textContent = "";

        let hasErrors = false;

        if (!name) {
            form.querySelector(".error-name").textContent = "Назва не може бути порожньою.";
            hasErrors = true;
        }
        if (!description) {
            form.querySelector(".error-description").textContent = "Опис не може бути порожнім.";
            hasErrors = true;
        }
        if (!slug) {
            form.querySelector(".error-slug").textContent = "Артикул/штрихкод не може бути порожнім.";
            hasErrors = true;
        }
        if (!image) {
            form.querySelector(".error-image").textContent = "Оберіть зображення.";
            hasErrors = true;
        }
        if (!price || parseFloat(price) <= 0) {
            form.querySelector(".error-price").textContent = "Ціна має бути більше 0.";
            hasErrors = true;
        }
        if (!stock || parseInt(stock) < 1) {
            form.querySelector(".error-stock").textContent = "Кількість має бути більше 0.";
            hasErrors = true;
        }
        if (!category) {
            form.querySelector(".error-category").textContent = "Будь ласка, виберіть категорію.";
            hasErrors = true;
        }

        if (hasErrors) return;

        fetch("/Admin/AddProduct", {
            method: "POST",
            body: new FormData(form)
        })
            .then(r => r.json())
            .then(j => {
                if (j.error) {
                    form.querySelector(".admin-error").textContent = j.error;
                } else {
                    console.log(j.message);
                    form.reset();
                }
            })
            .catch(error => {
                form.querySelector(".admin-error").textContent = "Помилка з'єднання з сервером.";
                console.error(error);
            });
    }
});

document.addEventListener('DOMContentLoaded', e => {
    for (let fab of document.querySelectorAll('[data-cart-product-id]')) {
        fab.addEventListener('click', addToCartClick);
    }
    for (let fab of document.querySelectorAll('[data-cart-decrement]')) {
        fab.addEventListener('click', decCartClick);
    }
    for (let fab of document.querySelectorAll('[data-cart-increment]')) {
        fab.addEventListener('click', incCartClick);
    }
    for (let fab of document.querySelectorAll('[data-cart-delete]')) {
        fab.addEventListener('click', deleteCartClick);
    }
});

function incCartClick(e) {
    const cartId = e.target.closest('[data-cart-increment]').getAttribute('data-cart-increment');
    console.log("++", cartId);
    modifyCartItem(cartId, 1);
}

function decCartClick(e) {
    const cartId = e.target.closest('[data-cart-decrement]').getAttribute('data-cart-decrement');
    console.log("--", cartId);
    modifyCartItem(cartId, -1);
}

function deleteCartClick(e) {
    const cartId = e.target.closest('[data-cart-delete]').getAttribute('data-cart-delete');
    const q = e.target.closest('.cart-item-row').querySelector('[data-cart-quantity]').innerText;
    console.log("xx", cartId);
    modifyCartItem(cartId, -q);
}

function modifyCartItem(cartId, delta) {
    fetch(`/Shop/ModifyCartItem?cartId=${cartId}&delta=${delta}`, {
        method: 'PUT'
    }).then(r => r.json())
        .then(j => {
            if (j.status == 200) {
                window.location.reload();
            }
            else if (j.status == 422) {
                alert("Недостатня кількість на складі");
            }
            else {
                console.log(j.message);
                alert("Помилка, повторіть пізніше");
            }
        });
}
function addToCartClick(e) {
    e.stopPropagation();
    e.preventDefault();
    const elem = document.querySelector('[data-auth-ua-id]');
    if (!elem) {
        alert('Увійдіть до системи для здійснення замовлень');
        return;
    }
    const uaId = elem.getAttribute('data-auth-ua-id');
    const productId = e.target.closest('[data-cart-product-id]').getAttribute('data-cart-product-id');
    console.log(productId, uaId);
    fetch('/Shop/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: `productId=${productId}&uaId=${uaId}`
    }).then(r => r.json()).then(j => {
        if (j.status == 200) {
            alert("Додано до кошику");
        }
        else {
            alert("Помилка додавання");
        }
    });
}