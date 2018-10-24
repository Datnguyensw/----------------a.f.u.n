//Hide Top
window.addEventListener("load", function () {
    // Set a timeout...
    setTimeout(function () {
        // Hide the address bar!
        window.scrollTo(0, 1);
    }, 0);
});
// Side Bar
$(document).ready(function () {
    var is_expand = false;
    $('.wrapper').css('overflow-x', 'hidden');
    $('#sib_btn').click(function () {
        if (is_expand == false) {
            $('.left_menu').animate({ left: '0px' }, 100);
            $('.carea').animate({ left: $('.left_menu').width() + 'px' }, 100);
            $('.carea').height($('.left_menu').height());
            $('.carea').css('overflow', 'hidden');
            $('body').css('position', 'absolute');
            is_expand = true;

            // Expand Header
            $('.header').animate({ left: $('.sidebar').width() + 'px' }, 100);
            $('.header').css('position', 'fixed');
            $('.header_menu').css('margin-top', $('.header').height() + 'px');
        }
        else {
            $('.left_menu').animate({ left: '-' + $('.left_menu').width() + 'px' }, 100);
            $('.carea').animate({ left: '0' }, 100);
            is_expand = false;
            $('body').height($('.carea').height() + 'px');
            $('body').css('position', 'relative');
            $('.carea').height('auto');
        }
        return false;
    });

});
//tab
$(document).ready(function(e) {
	$('.tab-service a').click(function(){
		$('.tab1').removeClass('active1');
		$(this).addClass('active1');
		$('.content-service').hide();
		var ct1 = $(this).attr('rel');
		$('#'+ct1).show();
		return false;
	});
	$('.tab2-video a').click(function(){
		$('.tab3').removeClass('active3');
		$(this).addClass('active3');
		$('.content-video').hide();
		var ct2 = $(this).attr('rel');
		$('#'+ct2).show();
		return false;
	});
	$('.tab3-book a').click(function(){
		$('.tab4').removeClass('active4');
		$(this).addClass('active4');
		$('.content-book').hide();
		var ct3 = $(this).attr('rel');
		$('#'+ct3).show();
		return false;
	});
	$('.tab5-news a').click(function(){
		$('.tab6').removeClass('active6');
		$(this).addClass('active6');
		$('.content-news').hide();
		var ct4 = $(this).attr('rel');
		$('#'+ct4).show();
		return false;
	});
	$('.tab7-quiz a').click(function(){
		$('.tab8').removeClass('active8');
		$(this).addClass('active8');
		$('.content-quiz').hide();
		var ct6 = $(this).attr('rel');
		$('#'+ct6).show();
		return false;
	});
	$('.tab_more a').click(function () {
	    $('.tab9').removeClass('active');
	    $(this).addClass('active');
	    $('.content-book').hide();
	    var ct7= $(this).attr('rel');
	    $('#' + ct7).show();
	    return false;
	});
	
});
