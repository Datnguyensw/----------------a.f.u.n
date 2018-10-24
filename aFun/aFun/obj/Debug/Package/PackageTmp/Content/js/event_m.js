$(function () {
    //slide
    $("#slides").slidesjs({
        width: 480,
        height: 240,
        play: {
            auto: true,
            swap: true,
        }
    });
    //$('#dg-container').gallery({
    //    autoplay: true
    //});
    //end slide
    //up top
    $(window).scroll(function () {
        if ($(this).scrollTop() > 800) {
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
    //star
    var $bien = jQuery.noConflict();

    $('.star4').rating({
        callback: function (value) {
            if (value > 0) {
                AddStar(value);
            }
        }
    });

    $('.starnologin').rating({
        callback: function (value) {
            alert("Quý khách vui lòng đăng nhập.Để đánh giá cuốn sách. Xin cảm ơn!");
            document.location.href = "/dang-nhap.html";
        }
    });
    //end star
});
