function AddStar(value) {
    var bookId = $('#bookId').val();
    $.post("/Book/AddStar", { bookId: bookId, star: value },
function (data) {
    if (data == "0") {
        alert('Cám ơn quý khách đã đánh giá.')
    }
    else {
        alert('Đánh giá thất bại.')
    }
});
}
function addlikebook(value, isFa) {
    $.post("/Book/addlikebook", { bookId: value, isFa: isFa },
function (data) {
    if (data == "1") {
        alert('Cám ơn quý khách đã yêu thích.');
    } else if (data == "2") {
        alert('Cám ơn quý khách. Sách này quý khách đã yêu thích.');
    }
    else {
        alert('Cám ơn quý khách đã yêu thích.Hệ thống đang bận.');
    }
});
}
function makeslideshow(id, stype) {
    $.post("/Home/makeslideshow", { id: id, stype: stype },
function (data) {
    location.href = data.linkurl;
});
}
function makelinkurl(txurl) {
    location.href = txurl;
}
