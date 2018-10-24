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
