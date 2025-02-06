// Made by MaxxTheLightning, 2025
const image = document.querySelector("img"),
input = document.querySelector("input");

input.addEventListener("change", () => {
    image.src = URL.createObjectURL(input.files[0]);
})