$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

/* Menu handler */
function setActiveMenuItem(item) {
    $('#' + item).addClass('active');
};

/* Venta de entradas */
function toggleSeat(row, seat) {
    var selector = "#" + row + "_" + seat;
    if ($(selector).hasClass("btn-success")) {
        /* Selección de butaca */
        $(selector).removeClass("btn-success");
        $(selector).addClass("btn-danger");
        /* Control del contador */
        var sCount = parseInt($("#seatCount").html());
        sCount = sCount + 1;
        $("#seatCount").html(sCount);
        /* Control de la selección escondida */
        var sSeats = [];
        if ($("#selectedSeats").val() != "") {
            var sSeats = $("#selectedSeats").val().split(",");
        };
        sSeats.push(row + "_" + seat);
        $("#selectedSeats").val(sSeats.join());
    }
    else {
        /* Selección de butaca */
        $(selector).addClass("btn-success");
        $(selector).removeClass("btn-danger");
        /* Control del contador */
        var sCount = parseInt($("#seatCount").html());
        sCount = sCount - 1;
        $("#seatCount").html(sCount);
        /* Control de la selección escondida */
        var sSeats = [];
        if ($("#selectedSeats").val() != "") {
            var sSeats = $("#selectedSeats").val().split(",");
        };
        var idx = sSeats.indexOf(row + "_" + seat);
        sSeats.splice(idx, 1);
        $("#selectedSeats").val(sSeats.join());
    }
}