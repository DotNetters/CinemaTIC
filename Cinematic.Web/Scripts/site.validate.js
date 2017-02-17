$(function () {
    $.validator.methods.date = function (value, element) {
        // Globalize.culture("es-ES");
        // you can alternatively pass the culture to parseDate instead of
        // setting the culture above, like so:
        // parseDate(value, null, "es-ES")
        return (
            this.optional(element) ||
            Globalize.parseDate(value, 'dd/MM/yyyy') !== null ||
            Globalize.parseDate(value, 'dd/MM/yyyy HH:mm') !== null ||
            Globalize.parseDate(value, 'dd/MM/yyyy HH:mm:ss') !== null);
    }
});