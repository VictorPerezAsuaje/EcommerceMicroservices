function toggleMegaMenu(name) {
    const activeMenu = document.querySelector(".megamenu:not([hidden])");
    if (activeMenu?.id === name) {
        activeMenu.setAttribute("hidden", "");
        return;
    }

    const menus = document.querySelectorAll(".megamenu");
    menus.forEach(menu => {
        if (menu.id === name)
            menu.removeAttribute("hidden");
        else
            menu.setAttribute("hidden", "");
    });
}

function openCartMenu() {
    const cart = document.getElementById("cart");
    cart.classList.remove("-right-3/4")
    cart.classList.remove("md:-right-1/2")
    cart.classList.remove("lg:-right-1/3")
    cart.classList.add("-right-0")
}

function closeCartMenu() {
    const cart = document.getElementById("cart");
    cart.classList.add("-right-3/4")
    cart.classList.add("md:-right-1/2")
    cart.classList.add("lg:-right-1/3")
    cart.classList.remove("-right-0")
}