// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('youtube-form');
    const loadingSearch = document.getElementById('loadingSearch');

    if (searchForm && loadingSearch) {
        searchForm.addEventListener('submit', function () {
            loadingSearch.style.display = 'block';
        });
    }

    const downloadForms = document.querySelectorAll('.download-form');
    const loadingDownload = document.getElementById('loadingDownload');

    if (downloadForms.length && loadingDownload) {
        downloadForms.forEach(form => {
            form.addEventListener('submit', function () {
                loadingDownload.style.display = 'block';
            });
        });
    }
});