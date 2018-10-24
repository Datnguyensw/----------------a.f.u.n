function sizing() {
	var w_site	= $('body').width();
	var boxplayer	= $('.boxplayer').width();
	if($('.box_screen').css('display') != 'none'){ 
		if(w_site>979 ){
			$('.box_screen').css({'margin-left' : '10px'});
			$('.box_screen').width(320);
			$('#videoWarp').width(boxplayer-330);
			var heightplayer = $('#videoWarp').height();
			$('.box_screen').height(heightplayer);
		}else{
			$('.box_screen').css({'margin-left' : '0'});
			$('.box_screen').width(boxplayer);
			$('#videoWarp').width(boxplayer);
		}
	}
}
////////////////////////////////////////////////////////////
$(document).ready(sizing);
$(window).resize(sizing);