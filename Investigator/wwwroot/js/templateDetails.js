let templateId = document.getElementById("templateId").innerText;

var connectionTemplateDetails = new signalR.HubConnectionBuilder().withUrl("/hubs/templateDetails").build();

function getLikes() {
    connectionTemplateDetails.send("JoinTemplate",templateId).then((value))
}
function fullfilled() {
    //do something on start
    console.log("Connection is succesfully established");
    getLikes();
}

function rejected() {
    //rejected logs
    console.log("Connection has failed");
}
connectionTemplateDetails.start().then(fullfilled, rejected);