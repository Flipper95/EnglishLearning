$("#notification-close").on("click", function () {
    $(this).parent(".alert").remove();
    document.cookie = 'Notification=;Path=/;expires=Thu, 01 Jan 1970 00:00:01 GMT;';
});