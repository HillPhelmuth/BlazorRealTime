import "https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/3.1.7/signalr.min.js";

let connection;
let subject;
let screenCastTimer;
let isStreaming = false;
let streaming = false;
const frameRate = 250;
const maxWidth = 960;
const maxHeight = 600;
const video = document.getElementById("video");
const canvas = document.getElementById("screenCanvas");
let filter = "none";

export async function initializeSignalR(hubUrl, agentName) {
    connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("NewViewer", function () {
        if (isStreaming === false)
            startStreamCast(agentName);
    });

    connection.on("NoViewer", function () {
        if (isStreaming === true)
            stopStreamCast();
    });
    connection.on("ReceiveMessage",
        function(chatMessage) {
            DotNet.invokeMethodAsync('BlazorRealTime.Client', 'ReceiveChat', chatMessage);
        });
    await connection.start().then(function () {
        console.log("connected");
    });
    
    return connection;
}
export function sendChat(agentName, chatMessage) {
    connection.send("SendChat", agentName, chatMessage);
}
async function startStreamCast(agentName) {
    
    isStreaming = true;
    subject = new signalR.Subject();
    connection.send("StreamCastData", subject, agentName);
    screenCastTimer = setInterval(function () {
        try {
            captureScreen().then(function (data) {
                subject.next(data);
            });

        } catch (e) {
            console.log(e);
        }
    }, Math.round(1000 / frameRate));
}
function captureScreen() {
    return new Promise(function(resolve) {
        var context = canvas.getContext("2d");
        context.drawImage(video, 0, 0, 960, 600);
        context.filter = filter;
        resolve(canvas.toDataURL());
    });
}
async function startCapture() {
   
    var displayMediaOptions = {
        video: {
            cursor: "always",

        },
        audio: false
    };
    var width = maxWidth;
    var height = maxHeight;
    try {
        await navigator.mediaDevices.getDisplayMedia(displayMediaOptions)
            .then(function (stream) {
                video.srcObject = stream;
            });
        video.addEventListener("canplay", function () {
            if (!streaming) {
                height = video.videoHeight / (video.videoWidth / width);
                if (isNaN(height)) {
                    height = width / (4 / 3);
                }
                video.setAttribute("width", width);
                video.setAttribute("height", height);
                canvas.setAttribute("width", width);
                canvas.setAttribute("height", height);
                streaming = true;
            }
        }, false);
    } catch (err) {
        console.error("Error: " + err);
    }
}

function stopCapture() {

    if (video.srcObject == null) {
        return;
    }
    let tracks = video.srcObject.getTracks();
    
    tracks.forEach(track => track.stop());
    video.srcObject = null;
}
export function stopStreamCast() {
    stopCapture();
    if (isStreaming === true) {
        clearInterval(screenCastTimer);
        subject.complete();
        isStreaming = false;
    }
}
export function startCast(agentName) {
    startCapture();
    connection.send("AddScreenCastAgent", agentName);
}