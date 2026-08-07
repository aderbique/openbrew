
// -------------------------------------------------------------------------------------------------------
// Recipe Instance Class
// -------------------------------------------------------------------------------------------------------
function Recipe(obj) {

    // initialize
    this.RecipeId = 0;
    this.OriginalRecipeId = null;
    this.UnitType = null,
    this.Name = null;
    this.Description = null;
    this.BatchSize = 0;
    this.BoilSize = 0;
    this.BoilTime = 0;
    this.Efficiency = 0;
    this.Og = 1.0;
    this.Fg = 1.0;
    this.Srm = 0.0;
    this.Ibu = 0.0;
    this.BgGu = 0.0;
    this.Abv = 0.00;
    this.Calories = 0;
    this.StyleId = null;
    this.StyleCatalog = 'legacy-bjcp';
    this.StyleName = null;
    this.IbuFormula = null;
    this.Visibility = 'public';

    // Arrays
    this.Fermentables = [], this.Hops = [], this.Yeasts = [], this.Others = [], this.MashSteps = [], this.Steps = [];

    /// Gets the Recipe as Json
    this.getJSON = function () {
        return JSON.stringify(this);
    };

    // calculates the recipe
    this.calculate = function () {

        // Reset Values
        this.Og = 1.00, this.Fg = 1.00, this.Abv = 0, this.Srm = 0, this.Calories = 0, this.Ibu = 0, this.BgGu = 0;

        // Constants
        var steepEff = .50;

        // gravities and color
        var totalGp = 0, earlyGp = 0, earlyOg = 0, colorUnits = 0.0;
        var batch = this.UnitType === 's' ? Number(this.BatchSize) : util.l_To_gal(Number(this.BatchSize));
        var boil = this.UnitType === 's' ? Number(this.BoilSize) : util.l_To_gal(Number(this.BoilSize));

        if(batch == 0) {
            return;
        }

        // Determine Attenuation
        // If multiple yeasts, average the attenuation
        var atten = 0;
        if (this.Yeasts.length > 0) {
            for (var i = 0; i < this.Yeasts.length; i++) {
                atten += Number(this.Yeasts[i].Atten);
            }
            atten = (atten / this.Yeasts.length);
        } else {
            // Default Value if No yeast selected
            atten = 0.75;
        }

        // Iterate Fermentables
        for (var i = 0; i < this.Fermentables.length; i++) {
            var f = this.Fermentables[i];

            if (f.Amt <= 0) {
                continue;
            }

            // Determine Gravity Points and Srm for this fermentable
            var eff = (f.Use === 'Extract' || f.Use === 'Late') ? 1.00 : f.Use === 'Steep' ? steepEff : (Number(this.Efficiency));
            var amt = this.UnitType === 's' ? Number(f.Amt) : util.kg_To_lb(Number(f.Amt));
            var gp = (amt * f.Ppg * eff) / batch;
            totalGp += gp;
            colorUnits += (amt * f.L) / batch;

            if(f.Use !== "late") {
                earlyGp += gp;
            }
        }

        // Set Values (note these are not rounded ... we will round for display only)
        this.Og = totalGp === 0 ? 1.000 : 1 + (totalGp / 1000);
        this.Fg = totalGp === 0 ? 1.000 : 1 + (totalGp * (1 - atten)) / 1000;
        this.Abv = totalGp === 0 ? 0 : (totalGp - (totalGp * (1 - atten))) * 0.129;
        this.Srm = colorUnits <= 0 ? 0 : 1.49 * Math.pow(colorUnits, 0.69);
        this.Calories = Math.round(this.Og <= 1.000 ? 0 : ((1881.22 * this.Fg * (this.Og - this.Fg)) /
            ((1.775 - this.Og)) + (3550.0 * this.Fg * ((0.1808 * this.Og) + (0.8192 * this.Fg) - 1.0004))));

        // Set Ibu / BGGU
        this.Ibu = 0;
        earlyOg = earlyGp === 0 ? 1.000 : 1 + (earlyGp / 1000);

        for (var i = 0; i < this.Hops.length; i++) {
            var h = this.Hops[i];
            h.Ibu = 0;

            if (h.Use != 'Boil' && h.Use != 'FirstWort') {
                continue;
            }

            var amtInOunces = (this.UnitType === 's' ? Number(h.Amt) : util.g_To_oz(Number(h.Amt)));

            var time = h.Min;

            var utilization = 0;
            var utilizationFactor = (h.Type === "Pellet" ? 1.1 : 1);

            // Formulas taken from: http://realbeer.com/hops/FAQ.html#units
            if (this.IbuFormula === "t") {
                // Tinseth
                utilization = (1.65 * Math.pow(0.000125, (earlyOg - 1)) * (1 - Math.exp(-0.04 * time)) / 4.15) * utilizationFactor;
                h.Ibu = (((h.AA / 100) * amtInOunces * 7490 / batch) * utilization);
            } else if (this.IbuFormula === "r") {
                // Rager
                utilization = 18.11 + ( 13.86 * util.tanh((time - 31.32) / 18.27));
                h.Ibu = ((amtInOunces) * (utilization / 100) * (h.AA / 100) * 7462) / (batch * (1 + Math.max(0, (earlyOg - 1.050) / 0.2))) * utilizationFactor;
            } else if (this.IbuFormula === "b") {
                // openbrew formula
                utilization = (1.65 * Math.pow(0.000125, (batch / boil) * (this.Og - 1)) * (1 - Math.pow(2.718281828459045235, (-0.04 * h.Min))) / 4.15) * utilizationFactor;
                h.Ibu = (h.AA * amtInOunces) * utilization * 74.90 / batch;
            }

            if(isNaN(h.Ibu)) {
                this.Ibu += 0;
            } else {
                this.Ibu += h.Ibu;
            }
        }

        this.BgGu = (this.Og <= 1 && this.Ibu > 0) ? 1.0 : (this.Og === 0 || this.Ibu === 0) ? 0
            : this.Ibu / ((this.Og - 1) * 1000);
    };

    // Instantiate from passed in object
    if(obj != null) {
        for(var prop in obj) {
            this[prop] = obj[prop];
        }
        this.calculate();
    }
}

// -------------------------------------------------------------------------------------------------------
// Recipe Builder Static Class
// -------------------------------------------------------------------------------------------------------
var Builder =
{
    doCalc: true,

    /// initializes the builder
    initialize: function () {
        // Prep Templates
        Builder.prepTemplates();

        // Wire Controls
        Builder.wireControlsAndEvents();

        // Description Autosize
        $('textarea[data-name=r_Description]').autosize();

        // Initial Calc and Update
        Builder.calcAndUpdate();
    },

    // Gets the Recipe Id
    getRecipeId : function() {
        return Number($('[data-name=r_RecipeId]').val());
    },

    /// Determines if it is a new recipe
    isNewRecipe : function() {
        return Builder.getRecipeId() === 0;
    },

    /// Gets the Recipe instance from UI Elements
    getRecipe: function () {

        var recipe = new Recipe();

        // Get Recipe Data
        $('.recipedata [data-name]').each(function () {
            var name = $(this).attr('data-name').replace('r_', '');
            recipe[name] = $(this).val();
        });

        // Get Ingredients and Steps
        $('.dataTable').each(function () {
            var type = $(this).attr('data-name').split('_')[0];

            // Determine List
            var list = type === 'f' ? recipe.Fermentables : type === 'h' ? recipe.Hops : type === 'y' ? recipe.Yeasts : type === 'o' ? recipe.Others : type === 'm' ? recipe.MashSteps : recipe.Steps;

            $(this).find('[data-datarow]').each(function () {
                var data = {};
                $(this).find('[data-track]').each(function () {
                    var name = $(this).attr('data-name').replace(type + '_', '');
                    data[name] = $(this).val();
                });
                list.push(data);
            });
        });

        recipe.calculate();
        return recipe;
    },

    /// Pauses Auto Calculations
    pauseCalc: function () {
        Builder.doCalc = false;
    },

    /// Resumes Auto Calculations
    resumeCalc: function (force) {
        Builder.doCalc = true;
        if (force) {
            Builder.calcAndUpdate();
        }
    },

    /// Wires general controls
    wireControlsAndEvents: function () {
        // Setup Fancy DropDowns
        $('[data-trigger],[data-name=r_StyleId],[data-name=r_StyleGuideEdition]').chosen();
        $('.builder').on('chosen:showing_dropdown', '[data-trigger],[data-name=r_StyleId],[data-name=r_StyleGuideEdition]', function () {
            var $groupbox = $(this).closest('.groupbox');
            var $container = $(this).next('.chosen-container');
            var closeTimer = $container.data('chosen-close-timer');

            if (closeTimer) {
                window.clearTimeout(closeTimer);
                $container.removeData('chosen-close-timer');
            }

            var menuTop = $container.offset().top;
            var viewportTop = $(window).scrollTop();
            var spaceAbove = menuTop - viewportTop;
            var spaceBelow = viewportTop + $(window).height() - menuTop - $container.outerHeight();
            var openUpward = spaceBelow < 250 && spaceAbove > spaceBelow;
            var availableSpace = openUpward ? spaceAbove : spaceBelow;
            var resultHeight = Math.max(160, Math.min(420, availableSpace - 56));

            $('.builder .groupbox.dropdown-open').not($groupbox).removeClass('dropdown-open');
            $groupbox.addClass('dropdown-open');
            $container
                .toggleClass('chosen-drop-up', openUpward)
                .addClass('chosen-open')
                .find('.chosen-results').css('max-height', resultHeight + 'px');
        });
        $('.builder').on('chosen:hiding_dropdown', '[data-trigger],[data-name=r_StyleId],[data-name=r_StyleGuideEdition]', function () {
            var $container = $(this).next('.chosen-container');
            var $groupbox = $(this).closest('.groupbox');
            var closeTimer = window.setTimeout(function () {
                // Chosen closes the menu before it raises the select's change event.
                // Keep this panel above its neighbors until that click/change cycle has
                // completed, otherwise a scrolled result can lose its mouse interaction.
                if (!$container.hasClass('chosen-with-drop')) {
                    $container.removeClass('chosen-open chosen-drop-up');
                    if (!$groupbox.find('.chosen-container.chosen-with-drop').length) {
                        $groupbox.removeClass('dropdown-open');
                    }
                }
                $container.removeData('chosen-close-timer');
            }, 150);
            $container.data('chosen-close-timer', closeTimer);
        });

        // Wire validated field class removal
        $('.builder').on('keyup', '[data-validate]', function () {
            if($(this).val().trim().length > 0) {
                $(this).removeClass('field-error');
            } else {
                $(this).addClass('field-error');
            }
        });

        // Wire Save Button
        $('#SaveRecipeButton').click(function () {

            // So the "You have unsaved changes" doesn't show
            $('.builder').attr('data-formchanged', 'false');

            // Clear Messages
            Message.clear();

            // Validation
            if(!Builder.validate()) {
                Message.error('Uh oh, something needs your attention.  Please check the highlighted entries below.');

                window.scrollTo(0, 1);

                return false;
            }

            // If not logged in, add the isanon attribute
            if (!$('.builder').attr('data-isanon')) {
                if (!util.userIsLoggedIn()) {
                    $('.builder').attr('data-isanon', '1');
                }
            }

            // Not Logged in, Show Login/Register dialog
            if ($('.builder').attr('data-isanon')) {
                var loginFrameurl = "/loginviadialog" + (parseInt($('[data-name=r_RecipeId').val()) > 0 ? "?editMode=1" : "");
                $.colorbox({ href: loginFrameurl, iframe: true, width: 800, height: 525, opacity: .35, overlayClose: false, closeButton: false, escKey: false, scrolling: false });
            } else {

                var recipe = Builder.getRecipe();

                if (recipe.RecipeId == 0) {
                    // New Recipes Saved via Form POST
                    Layout.statusModal('Saving Recipe...');
                    $('.builder').removeAttr('data-formchanged');
                    $('#RecipeJson').val(escape(recipe.getJSON()));
                    $('#RecipeForm').submit();
                } else {
                    // Existing Recipes Saved via Ajax
                    if(recipe.RecipeId > 0) {
                        // We execute the AJAX request after the modal to ensure
                        // that the "Saving Recipe" message is displayed first
                        var modalComplete = function() {
                            $.ajax({
                                url: "/SaveRecipe",
                                data: { RecipeJson: Builder.getRecipe().getJSON() },
                                method: "post",
                                success: function (t) {
                                    switch (t) {
                                        case "-1": // general failure
                                            Message.error('There was a problem saving your recipe.  Please try again');
                                            break;
                                        case "0": // validation error
                                            Message.error('Did you leave something blank?  Please check your entries and try again.');
                                            break;
                                        case "1": // success
                                            Message.success('Your Recipe has been saved');
                                            $('.builder').removeAttr('data-formchanged');

                                            // Hide All Custom Name Fields (show span after saving)
                                            // This is done to prevent editing the name after save
                                            // And causing a second custom ingredient to get created
                                            $('.customname:visible').each(function(i, e) {
                                                $(e).siblings('.ingName').text($(e).val()).show();
                                                $(e).hide();
                                            });

                                            break;
                                    }
                                },
                                error: function (t) {
                                    Message.error('There was a problem saving your recipe.  Please try again.');
                                },
                                complete: function () {
                                    $.colorbox.close();

                                    // Prevent the Facts from Stepping on the Message
                                    $('.compliment').css('position', '').css('margin-left', '').css('top', '');
                                    window.scrollTo(0, 0);
                                }
                            });
                        };

                        Layout.statusModal('Saving Recipe...', modalComplete);
                    }
                }
            }

            return false;
        });

        // Scale Dialog
        $('#ScalePopupTrigger').click(function () {

            var recipe = Builder.getRecipe();
            var max = recipe.UnitType === "s" ? 100 : 400;

            $('#ScaleSlider').slider({
                animate: false,
                value: recipe.BatchSize,
                min: 1,
                max: max,
                step: 1,
                slide: function (event, ui) {
                    $('#TargetVolume').val(ui.value.toFixed(2));
                }
            });

            $('#TargetVolume').val(Number(Builder.getRecipe().BatchSize).toFixed(2)).removeClass("input-validation-error");
            $.colorbox({ inline: true, href: '#ScaleDialog', opacity: .35, width: 600, height: 290, overlayClose: false, escKey: true, scrolling: false });
            return false;
        });

        // Apply Scale Button -- Enter Press
        $('#TargetVolume').keydown(function (e) {
            if (e.which == 13) {
                $('#ApplyScaleButton').click();
                return false;
            }
            return true;
        });

        // Boil Time Change -- To Affect First Wort Hopping
        $('[data-name=r_BoilTime]').change(function () {
            var val = $(this).val();
            $('[data-name=h_Use] option[value="FirstWort"]:selected').each(function(i, e) {
                $(e).parents('tr').find('[data-name=h_Min]').val(val);
            });
        });

        // Apply Scale Button Event
        $('#ApplyScaleButton').click(function () {
            if (isNaN($('#TargetVolume').val()) || $('#TargetVolume').val().trim().length == 0) {
                $('#TargetVolume').addClass("input-validation-error");
                return false;
            }

            Builder.scale(Number($('#TargetVolume').val()));
            $.colorbox.close();
            return false;
        });

        // Photo Dialog
        if (Builder.isNewRecipe()) {
            $('#PhotoPopupTrigger').click(function () {
                $.colorbox({ inline: true, href: '#PhotoDialog', opacity: .35, width: 600, height: 285, overlayClose: false, escKey: true, scrolling: false });
            });
            $('#SavePhotoButton').click(function () {
                $.colorbox.close();
            });
        } else {
            $('#PhotoPopupTrigger').click(function () {
                $.colorbox({ iframe: true, href: '/builderchangerecipephoto/' + Builder.getRecipeId(), opacity: .35, width: 600, height: 325, overlayClose: false, escKey: true, scrolling: false });
            });
        }


        // Setup Controls and Events
        Builder.wireCalcTriggers();
        Builder.wireIngAddTriggers();
        Builder.wireStepControls();
        Builder.wireSorting();
        Builder.wireStyleChart();

        // Unit Change Event
        $('[data-name=r_UnitType]').change(function () {
            Builder.useUnit($(this).val());
        });

        // Wire Hop IBU Formula Select
        $('[data-name=r_IbuFormula]').change(function() {
            Builder.calcAndUpdate();
        });

        // Auto-Focus Amt Fields with 0
        $('.dataTable').on('click', 'input[type=text]', function() {
            if ($(this).val().trim() == '0') {
                $(this).select();
            }
        });

        // Fermentable Row Events
        $('[data-name=f_table]').on('keyup', '[data-name=f_Amt]', function () {
            Builder.updateFVals();
        });
        $('[data-name=f_table]').on('click', '[data-name=remove]', function () {
            Builder.updateFVals();
        });

        // Hop Row Events
        $('[data-name=h_table]').on('change', '[data-name=h_Use]', function() {
            var row = $(this).parents('[data-datarow]');
            var val = $(this).val();

            var minEle = row.find('[data-name=h_Min]');
            var dayEle = row.find('[data-name=h_Day]');

            minEle.show().removeAttr('disabled');
            dayEle.hide();

            if(val == "DryHop") {
                minEle.hide();
                dayEle.show();
            }

            // First Wort Forces Boil Time
            if (val == "FirstWort") {
                minEle.val($('[data-name=r_BoilTime]').val()).attr('disabled', 'disabled');
            }

            // Flame Out forces 0 Minutes
            if(val == "FlameOut") {
                minEle.val("0").attr('disabled', 'disabled');
            }
        });
        $('[data-name=h_table]').on('keyup', '[data-name=h_Amt]', function () {
            Builder.updateHVals();
        });
        $('[data-name=h_table]').on('click', '[data-name=remove]', function () {
            Builder.updateHVals();
        });

        // Step Row Events
        $('[data-name=s_table]').on('change', '[data-name=s_Rank]', function () {
            $(this).closest('tr').find('[data-name=s_RankLabel]').text($(this).val() + '.');
        });

        // Update Stylename in Recipe Facts
        $('[data-name=r_StyleId]').change(function() {
            if($(this).val()) {
                $('[data-name=facts_stylename]').text($(this).find('option:selected').text().split('.')[1]);
            } else {
                $('[data-name=facts_stylename]').text('');
            }
        });
    },

    /// Wires cal triggers
    wireCalcTriggers: function () {
        $('.builder').on('keyup', 'input[data-track]', function() {
            Builder.calcAndUpdate();
        });
        $('.builder').on('change', 'select[data-track]', function () {
            Builder.calcAndUpdate();
        });
        $('.builder').on('click', 'img[data-name=remove]', function () {
            var table = $(this).parents('.dataTable');
            var type = table.attr('data-name').split('_')[0];
            var row = $(this).parents('[data-datarow]');

            row.remove();
            Builder.reRankTable(type);

            if(type != 's') {
                Builder.calcAndUpdate();
            }
        });
    },

    /// Wires Add Ing Triggers
    wireIngAddTriggers : function() {
        $('[data-trigger]').change(function () {
            var ingType = $(this).attr('data-trigger');
            var ingEle = $(this).find('option[value="' + $(this).val() + '"]');
            var ingId = Number(ingEle.val());
            var name = ingEle.text();
            var typeName = null;

            if (name == "Add Custom Ingredient") {
                name = null;
            }

            var ingText = null;

            // Add Appropriate Type
            if (ingType === 'f') {
                Builder.addF(0, ingId, 0, 0, name, Number(ingEle.attr('data-ppg')), Number(ingEle.attr('data-L')), ingEle.attr('data-use'));
                ingText = "Fermentables";
            } else if(ingType === 'h') {
                Builder.addH(0, ingId, 0, 0, name, 'Pellet', 'Boil', 60, 0, Number(ingEle.attr('data-aa')).toFixed(1));
                ingText = "Hops";
            } else if (ingType === 'y') {
                Builder.addY(0, ingId, 0, name, (Number(ingEle.attr('data-atten'))));
                ingText = "Yeast";
            } else if (ingType === 'o') {
                Builder.addO(0, ingId, 0, 0, 'each', name, ingEle.attr('data-use'));
                ingText = "Ingredients";
            } else if (ingType === 'm') {
                Builder.addM(0, ingId, 0, name, 'Decoction', (Builder.getRecipe().UnitType == "s" ? '150' : '66'), '60');
                ingText = "Mash Steps";
            }

            // Reset Select to No Val using custom function added to chosen
            $(this).val('').trigger('chosen:brewgr_custom_reset', "Add " + ingText +"...");
        });
    },

    /// Wires step controls
    wireStepControls : function() {
        $('[data-name=s_add]').click(function () { Builder.addS(0, 0, null).find('input').focus(); return false; });

        // Override Tab Functionality inside Brewing Steps
        $('[data-name=s_table] tbody').keydown(function (event) {
            var tabIndex = $(this).attr('tabindex');
            var code = event.keyCode || event.which;

            // Regular Tab
            if (!event.shiftKey && code == '9') {
                var lastStep = $('[data-name=s_table] tr input:last');
                if (lastStep[0] == document.activeElement) {
                    Builder.addS(0, 0, null).find('input').focus();
                    return false;
                } else {
                    return true;
                }
            }
                // Shift Tab
            else if (event.shiftKey && code == '9') {
                if (tabIndex > 1) {
                    $('[data-name=s_table] tr input[tabindex=' + (tabIndex - 1) + ']').trigger('focus');
                    return false;
                }
            }
            return true;
        });
    },

    // Wire Sorting
    wireSorting : function() {
        $('.dataTable').each(function () {
            var dataName = $(this).attr('data-name');
            $('[data-name=' + dataName + '] tbody').sortable({
                containment: '[data-name=' + dataName + ']',
                placeholder: 'placeholder',
                start: function (event, ui) {
                    ui.placeholder.html('<td colspan="10" class="placeholder">&nbsp;</td>');
                },
                update: function (event, ui) {
                    // Re-Rank Table
                    var type = $(ui.item).parents('table').attr('data-name').split('_')[0];
                    Builder.reRankTable(type);
                    Builder.refreshTabIndices();
                }
            });
        });
    },

    /// Wires the Style Chart
    wireStyleChart : function() {
        // Style Guide
        $('[data-name=r_StyleId]').change(function () {
            if ($(this).val() == '') {
                $('#recipe-facts .style').text('');
                $('#selected-style-chart').empty();
            } else {
                $('#recipe-facts .style').text($(this[this.selectedIndex]).text().split('. ')[1]);

                StyleChart.createForRecipeStyle($(this).val(), $('[data-name=r_StyleCatalog]').val(), Builder.getRecipe(), $('#selected-style-chart'));
            }
        });
    },

    /// Re-Ranks a table
    reRankTable: function (type) {
        $('[data-name=' + type + '_table] tbody tr').each(function (index, ele) {
            $(ele).find('[data-name=' + type + '_Rank]').val(index + 1).change();
            $(ele).find('[data-name=' + type + '_RankLabel]').text((index + 1) + '.');
        });
    },

    // Pulls down ing row templates and adds them to the DOM
    // We do it this way to benefit from browser caching
    prepTemplates : function() {
        $.ajax({
            url: '/buildertemplates-v2',
            async: false,
            cache: true,
            success: function (t) {
                $(t).appendTo('body');
            }
        });
    },

    // Adds a Fermentable
    addF: function (id, ingId, rank, amt, name, ppg, l, use) {
        Builder.addData('f', { Id: id, IngId: ingId, Rank: rank, Amt: amt, Name: name, CustomName: null, Ppg: ppg, L: l, Use: use });

        // Update F Totals
        Builder.updateFVals();
    },

    /// Updates F Vals
    updateFVals: function () {
        var total = 0;

        $('[data-name=f_Amt]').each(function() {
            total += isNaN($(this).val()) ? 0 : Number($(this).val());
        });

        var values = [];
        var percent = [];

        $('[data-name=f_Amt]').each(function () {
            values.push(isNaN($(this).val()) ? 0 : Number($(this).val()));
        });

        var sum = 0;
        $.each(values, function () { sum += parseFloat(this) || 0; });
        var totalPercent = 100;
        for (var i = 0; i < values.length; i++) {
            var rawPercent = sum == 0 ? 0 : values[i] / sum * totalPercent;
            sum -= values[i];
            var roundedPercent = Math.round(rawPercent);
            totalPercent -= roundedPercent;
            percent.push(roundedPercent);
        }
        var index = 0;

        $('[data-name=f_Per]').each(function () {
            $(this).text((isNaN(percent[index]) ? '0' : percent[index]) + '%');
            index++;
        });

        $('[data-name=f_Total]').text(util.safeRound(total));
    },

    // Addss a Hop
    addH: function (id, ingId, rank, amt, name, type, use, min, day, aa) {
        Builder.addData('h', { Id: id, IngId: ingId, Rank: rank, Amt: amt, Name: name, Customname: null, Type: type, Use: use, Min: min, Day: day, AA: aa, Ibu: 0 });

        // Update H Totals
        Builder.updateHVals();
    },

    /// Updates H Vals
    updateHVals: function() {
        var total = 0;
        $('[data-name=h_Amt]').each(function () {
            var val = $(this).val().trim().length > 0 ? Number($(this).val()) : 0;
            total += val;
        });

        $('[data-name=h_Total]').text(util.safeRound(total));
    },

    // Adds a Yeast
    addY: function (id, ingId, rank, name, atten) {
        Builder.addData('y', { Id: id, IngId: ingId, Rank: rank, Name: name, CustomName: null, Atten: atten.toFixed(2) });
    },

    // Adds an Other Ingredient
    addO: function (id, ingId, rank, amt, unit, name, use) {
        var row = Builder.addData('o', { Id: id, IngId: ingId, Rank: rank, Amt: amt, Unit: unit, Name: name, CustomName: null, Use: use });

        // Show the Appropriate Unit Drop Down
        row.find('[data-name=o_Unit][data-unit=' + Builder.getRecipe().UnitType + ']').show().attr('data-track', '1');
        row.find('[data-name=o_Unit][data-unit=' + (Builder.getRecipe().UnitType == "s" ? "m" : "s") + ']').hide().removeAttr('data-track');
    },

    // Adds a MashStep
    addM: function (id, ingId, rank, stepType, heat, temp, time) {
        Builder.addData('m', { Id: id, IngId: ingId, Rank: rank, Name: stepType, Heat: heat, Temp: temp, Time: time });
    },

    // Appends a new Brewing Step
    addS: function (id, rank, text) {
        var row = Builder.addData('s', { Id: id, Rank: rank, Text: text });
        return row;
    },

    /// adds an ingredient
    addData: function (type, values) {

        if (values.Rank == null || values.Rank === 0) {
            values.Rank = $('[data-name=' + type + '_table] [data-datarow]').length + 1;
        }

        // Set Rownum
        values.rownum = ++Builder.rownum;

        // Create Row from Template
        var row = util.applyTemplate(type + '_Row', values);

        // Iterate over the values object, find matching elements, set values
        for (var prop in values) {
            if(values[prop] != null) {
                row.find('[data-name=' + type + '_' + prop + ']').val(values[prop].toString());
            }
        }

        // Set Data Filters
        row.find('[data-filter]').each(function (i, e) {
            util.filterInput($(e), $(e).attr('data-filter'));
        });

        // Handle Custom
        if(type !== 's') {
            if(values.IngId === 0) {
                row.find('[data-name=' + type + '_Name]').hide();
                row.find('[data-name=' + type + '_CustomName]').show().removeClass("hidden");
            } else {
                row.find('[data-name=' + type + '_Name]').show().removeClass("hidden");
                row.find('[data-name=' + type + '_CustomName]').hide();
            }
        }

        // Append the new Row (show header/footer, hide intro)
        var table = $('[data-name=' + type + '_table]');
        table.find('.introrow').remove();
        table.find('thead, tfoot').show();
        table.find('tbody').append(row);

        // Refresh Tab Indices
        Builder.refreshTabIndices();

        return row;
    },

    // Uses a unit type
    useUnit: function (unit) {
        if ($('[data-name=r_UnitType]') === unit) {
            return;
        }

        var prevUnit = unit === 'm' ? 's' : 'm';

        // Pause Calculations
        Builder.pauseCalc();

        // Toggle Labels
        $('[data-unit]').hide();
        $('[data-unit="' + unit + '"]').show();

        // Convert Sizes
        var infoConvFun = (unit === 'm' ? util.gal_To_l : util.l_To_gal);
        if($('[data-name=r_BatchSize]').val().length > 0) {
            $('[data-name=r_BatchSize]').val(util.safeRound(infoConvFun(Number($('[data-name=r_BatchSize]').val()))));
        }
        if($('[data-name=r_BoilSize]').val().length > 0) {
            $('[data-name=r_BoilSize]').val(util.safeRound(infoConvFun(Number($('[data-name=r_BoilSize]').val()))));
        }

        // Convert Fermentables
        var fConvFunc = (unit === 'm' ? util.lb_To_kg : util.kg_To_lb);
        $('[data-name=f_Amt]').each(function () {
            if($(this).val().length > 0) {
                $(this).val(util.safeRound(fConvFunc(Number($(this).val()))));
            }
        });

        // Convert Hops
        var hConvFunc = (unit == 'm' ? util.oz_To_g : util.g_To_oz);
        $('[data-name=h_Amt]').each(function () {
            if($(this).val().length > 0) {
                $(this).val(util.safeRound(hConvFunc(Number($(this).val()))));
            }
        });

        // Switch o-Ing Units and COnvert Amounts
        $('[data-name=o_Unit][data-track]').each(function (i, e) {

            var amtEle = $(this).parents('tr').find('[data-name=o_Amt]');
            var optEle = $(e).find('option:selected');

            // Set Other Unit Select value, add the data-track attribute
            $(e).siblings('select').val($(optEle).attr('data-aunit')).attr('data-track', '1');
            $(e).removeAttr('data-track');

            // Convert the Value
            if ($(e).val() != 'each') {
                var oConvFunc = window["util"][$(this).val() + "_To_" + optEle.attr('data-aunit')];
                amtEle.val(util.safeRound(oConvFunc(Number(amtEle.val()))));
            }
        });

        // Convert Mash Temperatures
        var mConvFunc = (unit === 'm' ? util.f_To_c : util.c_To_f);
        $('[data-name=m_Temp][data-track]').each(function(i,e) {
            if ($(this).val().length > 0) {
                $(this).val(Math.round(mConvFunc(Number($(this).val()))));
            }
        });

        // Resume Calculations
        Builder.resumeCalc();

        Builder.updateFVals();
        Builder.updateHVals();
        Builder.updateVisibilityStatus(recipe);
    },

    // Scales a Recipe
    scale: function (targetVol) {
        var recipe = Builder.getRecipe();

        var curIbu = recipe.Ibu;
        var curVol = Number($('[data-name=r_BatchSize]').val());
        var boilVol = Number($('[data-name=r_BoilSize]').val());

        if (isNaN(targetVol) || targetVol === curVol) {
            return;
        }

        // Pause Calculations
        Builder.pauseCalc();

        var factor = targetVol / curVol;

        $('[data-name=r_BatchSize]').val(util.safeRound(targetVol));
        $('[data-name=r_BoilSize]').val(util.safeRound(targetVol + (boilVol - curVol)));

        $('[data-scale]').each(function (i, e) {
            $(this).val(util.safeRound(Number($(this).val()) * factor));
        });

        // Resume Calculations
        Builder.resumeCalc(true);
        recipe = Builder.getRecipe();

        // Special Handling for Hops and IBU (we have to do this after re-calculate)
        // We increase the Boil Hops by the IBU growth rate
        var ibuGrowth = curIbu / recipe.Ibu;// : Builder.Recipe.Ibu / curIbu;
        $('[data-name=h_Amt]').each(function () {
            if ($(this).closest('tr').find('[data-name=h_Use]').val() == "Boil") {
                $(this).val((Number($(this).val()) * ibuGrowth).toFixed(4));
            }
        });
    },

    /// refreshes Tab Indices
    refreshTabIndices: function () {
        $('input[type=text],select,textarea').each(function (i, e) {
            $(this).attr("tabindex", i + 1);
        });
    },

    /// Validates the Recipe, adds field errors as necessary
    validate: function () {
        var isValid = true;
        var amtRegex = /^(?=.*\d)\d{0,6}(?:\.\d{1,})?$/;
        $('[data-validate]').filter(':visible').each(function (i, e) {
            if ($(e).val().trim().length == 0) {
                $(e).addClass('field-error');
                isValid = false;
            }
            if ($(e).attr('data-name').indexOf('_Amt') > -1) {
                if (!amtRegex.test($(e).val())) {
                    $(e).addClass('field-error');
                    isValid = false;
                }
            }
        });

        return isValid;
    },

    /// Calculates and Updates UI
    calcAndUpdate: function () {
        if(!Builder.doCalc) {
            return;
        }

        // Get Recipe from UI
        var recipe = Builder.getRecipe();

        // Calculate
        recipe.calculate();

        Builder.updateFVals();
        Builder.updateHVals();
        Builder.updateVisibilityStatus(recipe);

        // Set Hop IBUs
        $('[data-name=h_table] [data-datarow]').each(function() {
            var rank = Number($(this).find('[data-name=h_Rank]').val());
            for (var i = 0; i < recipe.Hops.length; i++) {
                if (Number(recipe.Hops[i].Rank) === rank) {
                    var ibu = Number(recipe.Hops[i].Ibu);
                    $(this).find('[data-name=h_Ibu]').text(!isNaN(ibu) ? ibu.toFixed(1) : 0);
                }
            }
        });

        // Set Recipe Facts
        $('[data-name=facts_og]').text(Number(recipe.Og).toFixed(3));
        $('[data-name=facts_fg]').text(Number(recipe.Fg).toFixed(3));
        $('[data-name=facts_srm]').text(Number(recipe.Srm).toFixed(1));
        $('[data-name=facts_abv]').text(Number(recipe.Abv).toFixed(1));
        $('[data-name=facts_cal]').text(Math.round(recipe.Calories));
        $('[data-name=facts_ibu]').text(recipe.Ibu.toFixed(1) + ' IBU');
        $('[data-name=facts_bggu]').text(Number(recipe.BgGu).toFixed(2));
        $('[data-name=facts_batchsize]').text(Number(recipe.BatchSize).toFixed(2));
        $('[data-name=facts_boilsize]').text(Number(recipe.BoilSize).toFixed(2));
        $('[data-name=facts_boiltime]').text(recipe.BoilTime);
        $('[data-name=facts_efficiency]').text((Number(recipe.Efficiency) * 100).toFixed(0) + '%');
        $('[data-name=facts_ibuformula]').html("&nbsp;(" + (recipe.IbuFormula === "r" ? "rager" : recipe.IbuFormula === "t" ? "tinseth" : "openbrew") + ")");

        $('.color').css('background-color', util.srm_To_hex(recipe.Srm));

        // Refresh the Style Chart after a calculation
        StyleChart.createForRecipeStyle($('[data-name=r_StyleId]').val(), $('[data-name=r_StyleCatalog]').val(), recipe, $('#selected-style-chart'));

        // if "similar styles" section is showing then we need to refresh them
        if ($('#style-hidemore').is(":visible")) {
            StyleChart.renderSimilarStyles($('[data-name=r_StyleId]').val(), $('[data-name=r_StyleCatalog]').val(), recipe, $('#similar-style-chart'));
            $('#style-showmore').hide();
            $('#style-hidemore').show();
        }
    },

    // Incomplete recipes are saved as private drafts by the server. Surface that
    // rule while editing so brewers know exactly what will happen on save.
    updateVisibilityStatus: function (recipe) {
        var isComplete = recipe.Fermentables.some(function (fermentable) {
            return Number(fermentable.Amt) > 0;
        }) && recipe.Yeasts.length > 0;
        var requestedVisibility = (recipe.Visibility || 'public').toLowerCase();
        var message = !isComplete
            ? 'Draft — add fermentables and yeast before this recipe can be published.'
            : requestedVisibility === 'private'
                ? 'Private — only you can see this recipe.'
                : 'Published — this recipe can appear in search and public feeds.';

        $('#RecipeVisibilityHint').text(message)
            .toggleClass('is-draft', !isComplete)
            .toggleClass('is-private', isComplete && requestedVisibility === 'private')
            .toggleClass('is-public', isComplete && requestedVisibility !== 'private');
    }
};

// -------------------------------------------------------------------------------------------------------
// Recipe Builder Static Class
// -------------------------------------------------------------------------------------------------------
var SessionBuilder =
{
    initialize: function() {
        $('[data-unit="' + $('[data-name=s_UnitType]').val() + '"]').addClass('active');

        // Data Unit Change Event
        $('[data-name=s_UnitType]').change(function() {
            SessionBuilder.useUnit($(this).val());
            WaterCalc.useUnit($(this).val());
        });

        // Notes Autosize
        $('textarea[data-name=s_Notes]').autosize();

        // Bottle/Keg Fields
        $('[data-name=s_ConditionType]').change(function() {
            if($(this).val() == "10") {
                $('.keg-field').hide();
                $('.bottle-field').show();
            } else if($(this).val() == "20") {
                $('.keg-field').show();
                $('.bottle-field').hide();
            } else {
                $('.keg-field').hide();
                $('.bottle-field').hide();
            }
        }).change();

        $('.builder').removeAttr('data-formchanged');

        // Wire Save Button
        $('#SaveBrewSessionButton').click(function(e) {
            e.preventDefault();
            SessionBuilder.save();
        });
    },

    // Gets the session as JSON
    getSession: function() {
        return {
            BrewSessionId: SessionBuilder.getValue('[data-name=s_BrewSessionId]'),
            RecipeId: SessionBuilder.getValue('[data-name=s_RecipeId]'),
            UnitTypeId: SessionBuilder.getValue('[data-name=s_UnitType]') == 's' ? 10 : 20,
            BrewDate: SessionBuilder.getValue('[data-name=s_BrewDate]'),
            Notes: SessionBuilder.getValue('[data-name=s_Notes]'),
            GrainWeight: SessionBuilder.getValue('[data-name=s_GrainWeight]'),
            GrainTemp: SessionBuilder.getValue('[data-name=s_GrainTemp]'),
            BoilTime: SessionBuilder.getValue('[data-name=s_BoilTime]'),
            BoilVolumeEst: SessionBuilder.getValue('[data-name=s_BoilVolumeEst]'),
            FermentVolumeEst: SessionBuilder.getValue('[data-name=s_FermentVolumeEst]'),
            TargetMashTemp: SessionBuilder.getValue('[data-name=s_TargetMashTemp]'),
            MashThickness: SessionBuilder.getValue('[data-name=s_MashThickness]'),
            TotalWaterNeeded: SessionBuilder.getValue('[data-name=s_TotalWaterNeeded]'),
            StrikeWaterTemp: SessionBuilder.getValue('[data-name=s_StrikeWaterTemp]'),
            StrikeWaterVolume: SessionBuilder.getValue('[data-name=s_StrikeWaterVolume]'),
            FirstRunningsVolume: SessionBuilder.getValue('[data-name=s_FirstRunningsVolume]'),
            SpargeWaterVolume: SessionBuilder.getValue('[data-name=s_SpargeWaterVolume]'),
            BrewKettleLoss: SessionBuilder.getValue('[data-name=s_BrewKettleLoss]'),
            WortShrinkage: SessionBuilder.getValue('[data-name=s_WortShrinkage]'),
            MashTunLoss: SessionBuilder.getValue('[data-name=s_MashTunLoss]'),
            BoilLoss: SessionBuilder.getValue('[data-name=s_BoilLoss]'),
            MashGrainAbsorption: SessionBuilder.getValue('[data-name=s_MashGrainAbsorption]'),
            SpargeGrainAbsorption: SessionBuilder.getValue('[data-name=s_SpargeGrainAbsorption]'),
            MashPH: SessionBuilder.getValue('[data-name=s_MashPH]'),
            MashStartTemp: SessionBuilder.getValue('[data-name=s_MashStartTemp]'),
            MashEndTemp: SessionBuilder.getValue('[data-name=s_MashEndTemp]'),
            MashTime: SessionBuilder.getValue('[data-name=s_MashTime]'),
            BoilVolumeActual: SessionBuilder.getValue('[data-name=s_BoilVolumeActual]'),
            PreBoilGravity: SessionBuilder.getValue('[data-name=s_PreBoilGravity]'),
            BoilTimeActual: SessionBuilder.getValue('[data-name=s_BoilTimeActual]'),
            PostBoilVolume: SessionBuilder.getValue('[data-name=s_PostBoilVolume]'),
            FermentVolumeActual: SessionBuilder.getValue('[data-name=s_FermentVolumeActual]'),
            OriginalGravity: SessionBuilder.getValue('[data-name=s_OriginalGravity]'),
            FinalGravity: SessionBuilder.getValue('[data-name=s_FinalGravity]'),
            ConditionDate: SessionBuilder.getValue('[data-name=s_ConditionDate]'),
            ConditionTypeId: SessionBuilder.getValue('[data-name=s_ConditionType]'),
            PrimingSugarType: SessionBuilder.getValue('[data-name=s_PrimingSugarType]'),
            PrimingSugarAmount: SessionBuilder.getValue('[data-name=s_PrimingSugarAmount]'),
            KegPSI: SessionBuilder.getValue('[data-name=s_KegPSI]')
        };
    },

    // Gets a value from a selector
    getValue: function(selector) {
        return $(selector).length ? $(selector).val() : null;
    },

    // Uses a unit type
    useUnit: function(unit) {
        if($('[data-name=s_UnitType]') === unit) {
            return;
        }

        var prevUnit = unit === 'm' ? 's' : 'm';

        // Toggle Labels
        $('[data-unit].active').removeClass('active');
        $('[data-unit="' + unit + '"]').addClass('active');

        util.convert('[data-name=s_MashStartTemp]', prevUnit, util.c_To_f, util.f_To_c);
        util.convert('[data-name=s_MashEndTemp]', prevUnit, util.c_To_f, util.f_To_c);

        util.convert('[data-name=s_BoilVolumeActual]', prevUnit, util.l_To_gal, util.gal_To_l);
        util.convert('[data-name=s_PostBoilVolume]', prevUnit, util.l_To_gal, util.gal_To_l);
        util.convert('[data-name=s_FermentVolumeActual]', prevUnit, util.l_To_gal, util.gal_To_l);
        util.convert('[data-name=s_PrimingSugarAmount]', prevUnit, util.g_To_oz, util.oz_To_g);

        util.convert('[data-name=p_BoilVolumeEst]', prevUnit, util.l_To_gal, util.gal_To_l);
    },

    /// Saves the Brew Session
    save: function() {

        // So the "You have unsaved changes" doesn't show
        $('.builder').attr('data-formchanged', 'false');

        // Clear Messages
        Message.clear();

        var session = SessionBuilder.getSession();

        // Validate
        $('.session .field-error').removeClass('field-error');
        if($('[data-name=s_BrewDate]').val() == null || $('[data-name=s_BrewDate]').val().length == 0) {
            $('[data-name=s_BrewDate]').addClass('field-error');
            Message.error('Uh oh, something needs your attention.  Please check the highlighted entries below.');
            window.scrollTo(0, 1);
            return false;
        }

        // Save
        if(session.BrewSessionId == 0) {
            // New Session Saved via Form POST
            Layout.statusModal('Saving Brew Session...');
            $('.builder').removeAttr('data-formchanged');
            $('#SessionJson').val(escape(JSON.stringify(session)));
            $('#SessionForm').submit();
        } else {
            // Existing Session Saved via Ajax
            // We execute the AJAX request after the modal to ensure
            // that the "Saving Brew Session" message is displayed first
            var modalComplete = function() {
                $.ajax({
                    url: "/brew/SaveSession",
                    data: { SessionJson: JSON.stringify(session) },
                    method: "post",
                    success: function(t) {
                        switch(t) {
                        case "-1": // general failure
                            Message.error('There was a problem saving your brew session.  Please try again');
                            break;
                        case "0": // validation error
                            Message.error('Did you leave something blank?  Please check your entries and try again.');
                            break;
                        case "1": // success
                            Message.success('Your Brew Session has been saved');
                            $('.builder').removeAttr('data-formchanged');
                            break;
                        }
                    },
                    error: function(t) {
                        Message.error('There was a problem saving your brew session.  Please try again.');
                    },
                    complete: function() {
                        $.colorbox.close();

                        // Prevent the Facts from Stepping on the Message
                        $('.compliment').css('position', '').css('margin-left', '').css('top', '');
                        window.scrollTo(0, 0);
                    }
                });
            };

            Layout.statusModal('Saving Brew Session...', modalComplete);
        }

        return false;
    }
};
