var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/userStatusHub").build();

connection.on("ReceiveStatusUpdate", function (userId, isActive) {
    if ($("#userdatanew").length) {
        // Find the row with the corresponding user ID and update the status
        var statusCell = $("#userdatanew tr[data-id='" + userId + "']").find("td:eq(7) span");
        if (isActive) {
            statusCell.removeClass("bg-lightred").addClass("bg-lightgreen").text("Active");
        } else {
            statusCell.removeClass("bg-lightgreen").addClass("bg-lightred").text("Inactive");
        }
    }
});
function newWindowLoadedOnClient() {
    connection.send("NotifyStatusChange");
}
function fullfilled() {
    console.log("Connection to user hub establish successfully.");
    newWindowLoadedOnClient();
}
function rejected() {

}
//connection.start().catch(function (err) {
//    return console.error(err.toString());
//});
connection.start().then(fullfilled, rejected);