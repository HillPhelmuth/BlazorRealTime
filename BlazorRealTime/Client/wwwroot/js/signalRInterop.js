//import 'https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/3.1.7/signalr.min.js';

let connection;
let subject;
let screenCastTimer;
let isStreaming = false;
let streaming = false;
const framepersecond = 10;
const screenWidth = 1280;
const screenHeight = 800;
const video = document.getElementById("video");
const canvas = document.getElementById('screenCanvas');
let filter = 'sepia(1)';

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

    await connection.start().then(function () {
        console.log("connected");
    });
    startCapture();
    return connection;
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
    }, Math.round(1000 / framepersecond));
}
function captureScreen() {
    return new Promise(function(resolve, reject) {
        
        var context = canvas.getContext('2d');
        context.drawImage(video, 0, 0, screenWidth, screenHeight);
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
    var width = screenWidth;
    var height = screenHeight;
    try {
        await navigator.mediaDevices.getDisplayMedia(displayMediaOptions)
            .then(function (stream) {
                video.srcObject = stream;
            });
        video.addEventListener('canplay', function () {
            if (!streaming) {
                height = video.videoHeight / (video.videoWidth / width);
                if (isNaN(height)) {
                    height = width / (4 / 3);
                }
                video.setAttribute('width', width);
                video.setAttribute('height', height);
                canvas.setAttribute('width', width);
                canvas.setAttribute('height', height);
                streaming = true;
            }
        }, false);
    } catch (err) {
        console.error("Error: " + err);
    }
}

function stopCapture() {
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
    connection.send("AddScreenCastAgent", agentName);
}