
/* Revolution Slider
========================================================*/
$(document).ready(function() {
 jQuery('.tp-banner').show().revolution({
  dottedOverlay:"none",
  delay:16000,
  startwidth:1170,
  startheight:650,
  hideThumbs:200,
  thumbWidth: 100,
  thumbHeight: 50,
  thumbAmount: 5,
  navigationType: "bullet",
  navigationArrows: "solo",
  navigationStyle: "preview4",
  touchenabled: "on",
  onHoverStop: "on",
  swipe_velocity: 0.7,
  swipe_min_touches: 1,
  swipe_max_touches: 1,
  drag_block_vertical: false,
  parallax: "mouse",
  parallaxBgFreeze: "on",
  parallaxLevels: [7, 4, 3, 2, 5, 4, 3, 2, 1, 0],
  keyboardNavigation: "off",
  navigationHAlign: "center",
  navigationVAlign: "bottom",
  navigationHOffset: 0,
  navigationVOffset: 20,
  soloArrowLeftHalign: "left",
  soloArrowLeftValign: "center",
  soloArrowLeftHOffset: 20,
  soloArrowLeftVOffset: 0,
  soloArrowRightHalign: "right",
  soloArrowRightValign: "center",
  soloArrowRightHOffset: 20,
  soloArrowRightVOffset: 0,
  shadow: 0,
  fullWidth: "on",
  fullScreen: "off",
  spinner: "spinner1",
  stopLoop: "off",
  stopAfterLoops: -1,
  stopAtSlide: -1,
  shuffle: "off",
  autoHeight: "off",
  forceFullWidth: "off",
  hideThumbsOnMobile: "off",
  hideNavDelayOnMobile: 1500,
  hideBulletsOnMobile: "off",
  hideArrowsOnMobile: "off",
  hideThumbsUnderResolution: 0,
  hideSliderAtLimit: 0,
  hideCaptionAtLimit: 0,
  hideAllCaptionAtLilmit: 0,
  startWithSlide: 0,
  fullScreenOffsetContainer: ""
  });
});

$(window).load(function() {
  $(".loader").fadeOut(2000);
});

/* Slicknav Mobile Menu
========================================================*/
  $(document).ready(function(){
    $('.wpb-mobile-menu').slicknav({
      prependTo: '.navbar-header',
      parentTag: 'liner',
      allowParentLinks: true,
      duplicate: true,
      label: '',
      closedSymbol: '<i class="fa fa-angle-right"></i>',
      openedSymbol: '<i class="fa fa-angle-down"></i>',
    });
  });

/* Count Time
   ========================================================================== */
   	jQuery('#clock').countdown('2017/9/16',function(event){
		var $this=jQuery(this).html(event.strftime(''
		+'<div class="time-entry days"><span>%-D</span> Days</div> '
		+'<div class="time-entry hours"><span>%H</span> Hours</div> '
		+'<div class="time-entry minutes"><span>%M</span> Minutes</div> '
		+'<div class="time-entry seconds"><span>%S</span> Seconds</div> '));
	});

/* Dropdown Menu
   ========================================================================== */
	$(".dropdown").hover(
		function () {
		$(this).addClass('open');
		}, 
		function () {
		$(this).removeClass('open');
		}
	);

/* search toogle
   ========================================================================== */
	var openSearch = $('.open-search'),
    SearchForm = $('.full-search'),
    closeSearch = $('.close-search');

    openSearch.on('click', function(event){
      event.preventDefault();
      if (!SearchForm.hasClass('active')) {
        SearchForm.fadeIn(300, function(){
          SearchForm.addClass('active');
        });
      }
    });

    closeSearch.on('click', function(event){
      event.preventDefault();

      SearchForm.fadeOut(300, function(){
        SearchForm.removeClass('active');
        $(this).find('input').val('');
      });
    });

/* Bootsrap slider carousel
   ========================================================================== */
	$('#carousel-slider').carousel();

	$('a[data-slide="prev"]').click(function () {
	    $('#carousel-slider').carousel('prev');
	});

	$('a[data-slide="next"]').click(function () {
	    $('#carousel-slider').carousel('next');
	});

/* Testimonials Carousel
========================================================*/
$(".testimonials-carousel").owlCarousel({
  navigation: false,
  pagination: true,
  slideSpeed: 1000,
  stopOnHover: true,
  autoPlay: true,
  items: 2,
  itemsDesktopSmall: [1024, 2],
  itemsTablet: [600, 1],
  itemsMobile: [479, 1]
});

/* Touch Owl Carousel
========================================================*/
$(".touch-slider").owlCarousel({
    navigation: true,
    pagination: false,
    slideSpeed: 1000,
    stopOnHover: true,
    autoPlay: true,
    items: 1,
    itemsDesktopSmall: [1024, 1],
    itemsTablet: [600, 1],
    itemsMobile: [479, 1]
});

$('.touch-slider').find('.owl-prev').html('<i class="fa fa-angle-left"></i>');
$('.touch-slider').find('.owl-next').html('<i class="fa fa-angle-right"></i>');

// MixitUp
$(function(){
  $('#portfolio-list').mixItUp();
});

// Nivo Lightbox  
  $('.lightbox').nivoLightbox({
    effect: 'fadeScale',
    keyboardNav: true,
    errorMessage: 'The requested content cannot be loaded. Please try again later.'
  });


/*Back Top Link
========================================================*/
var offset = 200;
var duration = 500;
$(window).scroll(function() {
  if ($(this).scrollTop() > offset) {
    $('.back-to-top').fadeIn(400);
  } else {
    $('.back-to-top').fadeOut(400);
  }
});
$('.back-to-top').click(function(event) {
  event.preventDefault();
  $('html, body').animate({
    scrollTop: 0
  }, 600);
  return false;
})
