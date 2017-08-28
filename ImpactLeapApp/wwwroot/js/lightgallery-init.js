$(document).ready(function() {

  // Init galleries
  $('.lightgallery-images').lightGallery({
    share: false,
    thumbnail: false,
    autoplay: false, 
    progressBar: false,
    autoplayControls: false,
    fullScreen: false, 
    zoom: false, 
    hash: false,
    download: false
  });

  // Associate gallery with button clicked
  $('.lightgallery-trigger').on('click', (function() {
    $(this).closest('.lightgallery').find('.lightgallery-images li:first-child').trigger('click');
  }));


});