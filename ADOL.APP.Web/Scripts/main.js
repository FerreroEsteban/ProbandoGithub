
$(document).ready(function () {
    var s = $('ul.mtree');

    s.find('button:first').addClass('active');
    s.find('.csl').on('click.mtree-close-same-level', function () {
        $(this).toggleClass('active');
    });




});