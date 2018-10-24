function androidPlayer(url){
	document.write('<video id="video123" autoplay="autoplay" controls="controls" src="'+ url +'" type="video/mp4" width="100%"></video>');
}
function reLoad_mb(url){
	video=document.getElementById('video123');
	video.src = url;
	video.load();
    video.play();
}