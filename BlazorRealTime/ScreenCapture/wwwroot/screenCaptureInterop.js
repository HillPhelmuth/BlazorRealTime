const videoElem = document.getElementById("video");
const logElem = document.getElementById("log");
const canvasCapture = document.getElementById("capture");
let width = 640;    // We will scale the photo width to this.
let height = 0;
let streaming = false;
// Options for getDisplayMedia()

//console.log = msg => logElem.innerHTML += `${msg}<br>`;
//console.error = msg => logElem.innerHTML += `<span class="error">${msg}</span><br>`;
//console.warn = msg => logElem.innerHTML += `<span class="warn">${msg}<span><br>`;
//console.info = msg => logElem.innerHTML += `<span class="info">${msg}</span><br>`;

export async function startCapture() {
    logElem.innerHTML = "";
    var displayMediaOptions = {
        video: {
            cursor: "always",
            
        },
        audio: false
    };
    try {
        await navigator.mediaDevices.getDisplayMedia(displayMediaOptions)
            .then(function (stream) {
                videoElem.srcObject = stream;
                console.log(stream);
                console.info(videoElem.scrObject);
                videoElem.play();
            });
        
        videoElem.addEventListener('canplay', function () {
            if (!streaming) {
                height = videoElem.videoHeight / (videoElem.videoWidth / width);
                if (isNaN(height)) {
                    height = width / (4 / 3);
                }
                videoElem.setAttribute('width', width);
                videoElem.setAttribute('height', height);
                
                streaming = true;
            }
        }, false);
        videoElem.addEventListener("play", function () {
            console.log('play');
            timercallback();
        }, false);
    } catch (err) {
        console.error("Error: " + err);
    }
}
export function stopCapture() {
    let tracks = videoElem.srcObject.getTracks();

    tracks.forEach(track => track.stop());
    videoElem.srcObject = null;
}
//function dumpOptionsInfo() {
//    const videoTrack = videoElem.srcObject.getVideoTracks()[0];

//    console.info("Track settings:");
//    console.info(JSON.stringify(videoTrack.getSettings(), null, 2));
//    console.info("Track constraints:");
//    console.info(JSON.stringify(videoTrack.getConstraints(), null, 2));
//}
function timercallback() {
    if (videoElem.paused || videoElem.ended) {
        return;
    }
    computeFrame();
    setTimeout(function () {
        timercallback();
    }, 0);
}
function computeFrame() {
    var video = document.getElementById('video');
    var canvas = document.createElement('canvas');
    canvas.setAttribute('width', 640);
    canvas.setAttribute('height', 480);
    var sendCanvas = document.getElementById('sent');
    var image = document.getElementById('smallImg');
    var context = canvas.getContext('2d');
    var sendContext = sendCanvas.getContext('2d');
    context.drawImage(videoElem, 0, 0, screen.width, screen.height);
    sendContext.drawImage(videoElem,0,0, video.videoWidth, video.videoHeight);
    var dataUrl = sendCanvas.toDataURL();//canvas.toDataURL('application/json');
    image.src = dataUrl;
    //let dataUrl = video.toDataURL('application/json');
    var data = dataUrl.split(',')[1];
    //var resultObj = JSON.stringify(dataUrl, null, 2);
    //console.log(resultObj);
    //var result = dataUrl.split(',')[1];
    DotNet.invokeMethodAsync('BlazorRealTime.Client', 'SendScreen', data);
}
function drawImageActualSize() {
    // Use the intrinsic size of image in CSS pixels for the canvas element
    canvas.width = this.naturalWidth;
    canvas.height = this.naturalHeight;

    // Will draw the image as 300x227, ignoring the custom size of 60x45
    // given in the constructor
    ctx.drawImage(this, 0, 0);

    // To use the custom size we'll have to specify the scale parameters 
    // using the element's width and height properties - lets draw one 
    // on top in the corner:
    ctx.drawImage(this, 0, 0, this.width, this.height);
}
export function setCapture(dataUrl) {
    //var capture = dataUrl;
    var canvas = document.getElementById('capture');
    img = document.createElement('img');
    img.height = 480;
    img.width = 640;
    img.src = dataUrl;
    var context = canvas.getContext('2d');
    context.drawImage(img, 0, 0);

    //var context = canvas.getContext('2d');
    //context.drawImage(dataUrl, 0, 0, 300, 225);
    //showStringifyResult(dataUrl);

}

//function showStringifyResult(target) {
//    let result = document.getElementById("result");
//    result.select();
//    result.setRangeText(JSON.stringify(stringify(target), null, ' '));
//}

//function stringify(element) {
//    let obj = {};
//    obj.name = element.localName;
//    obj.attributes = [];
//    obj.children = [];
//    Array.from(element.attributes).forEach(a => {
//        obj.attributes.push({ name: a.name, value: a.value });
//    });
//    Array.from(element.children).forEach(c => {
//        obj.children.push(stringify(c));
//    });

//    return obj;
//}