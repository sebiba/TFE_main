function ViewFile(li) {
    $("#detail").html("<iframe src='\\Data\\" + li.getAttribute("data-value") + "' width='800' height='1200'></iframe>")
}

function Post(url, input) {
    $.ajax({
        method: "POST",
        url: url,
        data: input
    })
        .done(function (msg) {
            alert("Data Saved: " + msg);
        });
}