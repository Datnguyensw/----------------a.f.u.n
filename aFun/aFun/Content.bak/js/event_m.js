$(function () {
    //up top
    $(window).scroll(function () {
        if ($(this).scrollTop() > 400) {
            $('.scrollup').fadeIn();
        } else {
            $('.scrollup').fadeOut();
        }
    });

    $('.scrollup').click(function () {
        $("html, body").animate({ scrollTop: 0 }, 800);
        return false;
    });

    //end uptop
    //refesh captcha
    $('.refeshcaptcha').click(function () {
       $('.captcha_img').load('/Captcha/Show');
       $('.captcha_img').attr('src', '/Captcha/Show');
    });
    //end refesh captcha
});

