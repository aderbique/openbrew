$(function() {
    
    // Default messaging settings
    Message.useSmall();
    Message.setParent('.message-wrapper');

    // Partner General Settings Form
    $('#view-partnersettings form, #view-sendtoshopsettings form').ajaxForm({
        preSubmitCallback: function (theForm) {
            Message.clear();
        },
        successCallback: function (theForm, data) {
            Message.success("Your changes have been saved");
            window.scrollTo(0, 0);
        },
        errorCallback: function (theForm) {
            Message.error("Oops, something went wrong.  Please try again.");
            window.scrollTo(0, 0);
        }
    });

    // Send to shop Ings Ajax Form
    $('#view-sendtoshopings form').ajaxForm({
        preSubmitCallback: function (theForm) {
            Message.clear();

            var pid = $('#PartnerId').val();
            var ings = [];

            $('[data-fid]:checked').each(function (i, e) { ings.push({ PartnerId: pid, IngredientTypeId: 10, IngredientId: $(e).attr('data-fid') }); });
            $('[data-hid]:checked').each(function (i, e) { ings.push({ PartnerId: pid, IngredientTypeId: 20, IngredientId: $(e).attr('data-hid') }); });
            $('[data-yid]:checked').each(function (i, e) { ings.push({ PartnerId: pid, IngredientTypeId: 30, IngredientId: $(e).attr('data-yid') }); });
            $('[data-aid]:checked').each(function (i, e) { ings.push({ PartnerId: pid, IngredientTypeId: 40, IngredientId: $(e).attr('data-aid') }); });

            $('#IngredientJson').val(JSON.stringify(ings));
        },
        successCallback: function (theForm, data) {
            Message.success("Your selections have been saved");
            window.scrollTo(0, 0);
        },
        errorCallback: function (theForm) {
            Message.error("Oops, something went wrong.  Please try again.");
            window.scrollTo(0, 0);
        }
    });

    // Selector links on Send to Shop Ing Lists
    $('#view-sendtoshopings .selector').each(function (i, e) {
        $(e).find('a.all').click(function () { $(this).parents('.ingGroup').find('input[type=checkbox]').attr('checked', 'checked'); return false; });
        $(e).find('a.none').click(function() { $(this).parents('.ingGroup').find('input[type=checkbox]').removeAttr('checked'); return false; });
    });

    // Partner Service Status Enable/Disable links
    $('.compliment').on('click', 'a.service-status-link', function () {
        var data =
        {
            PartnerId: $(this).attr('data-partnerid'),
            PartnerServiceType: $(this).attr('data-servicetype'),
            IsEnabled: $(this).attr('data-enabled')
        };

        $.ajax({
            url: '/partner/SetPartnerServiceStatus/',
            data: data,
            method: 'post',
            success: function(t) {
                $('#service-status').replaceWith(t);
            },
            error: function () {
                Message.error("An unknown error has occurred, please try again.");
            }
        });
    });

    var toggleTimeFrameElements = function(ele) {
        if ($(ele).val() == "true") {
            // enable restrictions
            $('#DayStart, #DayEnd, #HourStart, #HourEnd').attr('disabled', 'disabled').css('color', '#ccc');
        } else {
            // disable restrictions
            $('#DayStart, #DayEnd, #HourStart, #HourEnd').removeAttr('disabled').css('color', '#333');
        }
    };

    toggleTimeFrameElements($('#AllowOutOfRangeOrders'));

    // Partner Send-To-Shop timeframe Drop Down
    $('#AllowOutOfRangeOrders').change(function() {
        toggleTimeFrameElements($(this));
    });


});