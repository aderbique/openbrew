$(function() {
    $('a[data-userfeedbackid]').click(function () {
        var anchor = $(this);
        $.ajax({
            url: '/admin/resolvefeedback/' + $(this).attr('data-userfeedbackid'),
            success: function() {
                anchor.parents('tr').remove();
            },
            error: function() {
                alert('Uh oh, something went wrong.');
            }
        });

        return false;
    });
    
    $('a[data-promoteurl]').click(function () {
        var ele = $(this);

        var ingId = ele.attr('data-id');
        var category = ele.siblings('[name=category]').val();

        $.ajax({
            url: ele.attr('data-promoteurl'),
            type: 'post',
            data: { IngredientId: ingId, Category: category },
            success: function (t) {
                ele.parents('tr').remove();
            },
            error: function() {
                alert('Uh oh, something happened.');
            }
        });

        return false;
    });

    $('#cache-items li a').each(function (i, e) {
        $(e).click(function () {
            $.ajax({
                url: 'RemoveCacheItem' + '/' + $(e).attr('data-key'),
                success: function (t) {
                    $(e).parent('li').remove();
                }
            });
            return false;
        });
    });
});