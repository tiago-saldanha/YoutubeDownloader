window.downloadFile = (fileName, contentType, fileBytes) => {
    const blob = new Blob([fileBytes], { type: contentType });

    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.download = fileName;
    link.href = url;

    document.body.appendChild(link);
    link.click();

    document.body.removeChild(link);
    URL.revokeObjectURL(url);
};

window.showLoading = (message = "Carregando...") => {
    const loader = document.getElementById("globalLoading");
    const text = document.getElementById("loadingText");

    if (text) text.innerText = message;
    if (loader) loader.style.display = "flex";
};

window.hideLoading = () => {
    const loader = document.getElementById("globalLoading");
    const text = document.getElementById("loadingText");

    if (loader) loader.style.display = "none";
    if (text) text.innerText = "";
};
