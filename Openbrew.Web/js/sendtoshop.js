$(function () {

    // Force Step 1 to load upon refresh
    if (window.location.toString().toLowerCase().indexOf("#step-1") == -1) {
        if (window.location.toString().toLowerCase().indexOf("#step-") > -1) {
            window.location = window.location.toString().split("#")[0] + "#step-1";
        } else {
            window.location = window.location.toString() + "#step-1";
        }
    }

    // Hash Change Event - To Support Browser Nav of Steps
    $(window).on('hashchange', function () {
        goToStep(getStep());
    });

    // Navigation of Steps
    $('a[data-nav]').click(function () {
        Message.clear();
        var target = $(this).attr('href').replace('#step-', '');
        var currentStep = getStep();

        window.scrollTo(0, 0);

        // Allow Backward Nav
        if (parseInt(target) < parseInt(currentStep)) {
            return true;
        }

        return validate();
    });

    // Show Terms Trigger
    $('#terms-link').click(function () {
        $('#sendtoshop-terms').show();
        return false;
    });

    // Row Level Checkboxes (enable/disable)
    $('input.enabled').change(function() {
        var tr = $(this).parents('tr');
        if(!this.checked) {
            tr.find('input[type]').attr('disabled', 'disabled');
            tr.find('td').css('color', '#ccc').css('background-color', '#ddd;');
            tr.find('td input[type=text]').css('color', '#aaa').css('background-color', '#eee;');
        } else {
            tr.find('input[type]').removeAttr('disabled');
            tr.find('td').css('color', '').css('background-color', '');
            tr.find('td input[type=text]').css('color', '').css('background-color', '');
        }

        $(this).removeAttr('disabled');
    });

    // Send Recipe Button
    $('#send-recipe').click(function () {
        if (!validate()) {
            window.scrollTo(0, 0);
            return false;
        }

        // Build Order Object
        var order =
        {
            RecipeId: $('#RecipeId').val(),
            Name: $('#Name').val(),
            EmailAddress: $('#EmailAddress').val(),
            PhoneNumber: $('#PhoneNumber').val(),
            Comments: $('#Comments').val(),
            Items:
                $('[data-ingtypeid]').map(function (i, e) {
                    if($(this).parents('tr').find('input.enabled:checked').length > 0) {
                        return {
                            IngredientTypeId: $(e).attr('data-ingtypeid'),
                            IngredientId: $(e).attr('data-ingid'),
                            Quantity: $(e).val(),
                            Unit: $(e).attr('data-unittype'),
                            Instructions: $(e).parents('tr').find('.instruct input:checked').map(function(ii, ee) { return ee.value; }).get().join(', ')
                        };
                    }
                }).get()
        };

        var modalComplete = function () {
            $.ajax({
                url: "/sendtoshop/createorder",
                data: JSON.stringify(order),
                method: "post",
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (t) {
                    switch (t.toString()) {
                        case "-1": // general failure
                            Message.error('There was a problem sending your recipe.  Please try again');
                            break;
                        case "0": // validation error
                            Message.error('Did you leave something blank?  Please check your entries and try again.');
                            break;
                        case "1": // success
                            window.location = "/sendtoshop/confirmation";
                            break;
                    }
                },
                error: function (t) {
                    Message.error('There was a problem saving your recipe.  Please try again.');
                },
                complete: function () {
                    $.colorbox.close();
                    window.scrollTo(0, 0);
                }
            });
        };

        // Submit via Ajax
        window.scrollTo(0, 0);
        Layout.statusModal('Sending Recipe...', modalComplete);
        return false;
    }); 
});

/// Goes to a specific step
function goToStep(step) {
    $('.step:visible').hide();
    $('#step-' + step).show();
}

/// Gets the Step
function getStep() {
    var step = 1;
    if (window.location.toString().toLowerCase().indexOf("#step-") > -1) {
        step = window.location.toString().toLowerCase().split("#step-")[1];
    }
    return step;
}

/// Validates the Step
function validate() {

    Message.clear();

    var message = "Uh oh, something needs your attention. Please check the highlighted entries below.";
    var step = getStep();
    var isValid = true;

    // Verify Text boxes are not blank
    $('#step-' + step + ' [data-val=true]').each(function (i, e) {
        if (!$(e).valid()) {
            isValid = false;
        }
    });

    // Verify Not Zero
    if($('#step-3').is(':visible')) {
        $('#step-3 input[data-ingtypeid]').each(function(i, e) {
            if($(e).parents('tr').find('input.enabled:checked').length > 0) {
                if(parseFloat($(e).val()) <= 0) {
                    isValid = false;
                    $(e).addClass('input-validation-error');
                }
            }
        });
    }

    // Verify One or more Checked Rows
    if($('#step-3').is(':visible')) {
        if($('#step-3 input.enabled:checked').length == 0) {
            isValid = false;
            message = "You must select one or more ingredients for your order";
        }
    }

    if (isValid) {
        if ($('#step-3').is(':visible')) {
            if (!$('#terms-agree').is(':checked')) {
                isValid = false;
                message = "You must agree to openbrew's Send-To-Shop Terms Of Use";
            }
        }
    }

    if (isValid) {
        $('.step:visible').hide();
        $('#step-' + step).show();
    } else {
        Message.error(message);
    }

    return isValid;
}
