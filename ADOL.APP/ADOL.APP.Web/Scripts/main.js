
$(document).ready(function () {
    var mtree = $('ul.mtree');


    s.find('button:first').addClass('active');
    s.find('.csl').on('click.mtree-close-same-level', function () {
        $(this).toggleClass('active');
    });




});