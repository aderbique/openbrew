// -------------------------------------------------------------------------------------------------------
// Style Chart Class
// -------------------------------------------------------------------------------------------------------
var StyleChart = {

    // CONSTANTS - used for creating the chart and the similar styles
    bjcpStyleChartValues: {
        og_low_graph: 0.995, og_high_graph: 1.120,
        fg_low_graph: 0.995, fg_high_graph: 1.120,
        ibu_low_graph: 0, ibu_high_graph: 120,
        srm_low_graph: 0, srm_high_graph: 40,
        abv_low_graph: 0, abv_high_graph: 14,
        bggu_low_graph: .00, bggu_high_graph: 2.00
    },

    allStyles: [{ "SubCategoryID": "0A", "SubCategoryName": "Water", "og_low": 1.000, "og_high": 1.000, "fg_low": 1.000, "fg_high": 1.000, "ibu_low": 0, "ibu_high": 0.01, "srm_low": 0, "srm_high": 0.01, "abv_low": 0.0, "abv_high": 0.01 }, { "SubCategoryID": "1A", "SubCategoryName": "Lite American Lager", "og_low": 1.028, "og_high": 1.04, "fg_low": 0.998, "fg_high": 1.008, "ibu_low": 8, "ibu_high": 12, "srm_low": 2, "srm_high": 3, "abv_low": 2.8, "abv_high": 4.2 }, { "SubCategoryID": "1B", "SubCategoryName": "Standard American Lager", "og_low": 1.04, "og_high": 1.05, "fg_low": 1.004, "fg_high": 1.01, "ibu_low": 8, "ibu_high": 15, "srm_low": 2, "srm_high": 4, "abv_low": 4.2, "abv_high": 5.3 }, { "SubCategoryID": "1C", "SubCategoryName": "Premium American Lager", "og_low": 1.046, "og_high": 1.056, "fg_low": 1.008, "fg_high": 1.012, "ibu_low": 15, "ibu_high": 25, "srm_low": 2, "srm_high": 6, "abv_low": 4.6, "abv_high": 6 }, { "SubCategoryID": "1D", "SubCategoryName": "Munich Helles", "og_low": 1.045, "og_high": 1.051, "fg_low": 1.008, "fg_high": 1.012, "ibu_low": 16, "ibu_high": 22, "srm_low": 3, "srm_high": 5, "abv_low": 4.7, "abv_high": 5.4 }, { "SubCategoryID": "1E", "SubCategoryName": "Dortmunder Export", "og_low": 1.048, "og_high": 1.056, "fg_low": 1.01, "fg_high": 1.015, "ibu_low": 23, "ibu_high": 30, "srm_low": 4, "srm_high": 6, "abv_low": 4.8, "abv_high": 6 }, { "SubCategoryID": "2A", "SubCategoryName": "German Pilsner (Pils)", "og_low": 1.044, "og_high": 1.05, "fg_low": 1.008, "fg_high": 1.013, "ibu_low": 25, "ibu_high": 45, "srm_low": 2, "srm_high": 5, "abv_low": 4.4, "abv_high": 5.2 }, { "SubCategoryID": "2B", "SubCategoryName": "Bohemian Pilsener", "og_low": 1.044, "og_high": 1.056, "fg_low": 1.013, "fg_high": 1.017, "ibu_low": 35, "ibu_high": 45, "srm_low": 3.5, "srm_high": 6, "abv_low": 4.2, "abv_high": 5.4 }, { "SubCategoryID": "2C", "SubCategoryName": "Classic American Pilsner", "og_low": 1.044, "og_high": 1.06, "fg_low": 1.01, "fg_high": 1.015, "ibu_low": 25, "ibu_high": 40, "srm_low": 3, "srm_high": 6, "abv_low": 4.5, "abv_high": 6 }, { "SubCategoryID": "3A", "SubCategoryName": "Vienna Lager", "og_low": 1.046, "og_high": 1.052, "fg_low": 1.01, "fg_high": 1.014, "ibu_low": 18, "ibu_high": 30, "srm_low": 10, "srm_high": 16, "abv_low": 4.5, "abv_high": 5.5 }, { "SubCategoryID": "3B", "SubCategoryName": "Oktoberfest/Märzen", "og_low": 1.05, "og_high": 1.057, "fg_low": 1.012, "fg_high": 1.016, "ibu_low": 20, "ibu_high": 28, "srm_low": 7, "srm_high": 14, "abv_low": 4.8, "abv_high": 5.7 }, { "SubCategoryID": "4A", "SubCategoryName": "Dark American Lager", "og_low": 1.044, "og_high": 1.056, "fg_low": 1.008, "fg_high": 1.012, "ibu_low": 8, "ibu_high": 20, "srm_low": 14, "srm_high": 22, "abv_low": 4.2, "abv_high": 6 }, { "SubCategoryID": "4B", "SubCategoryName": "Munich Dunkel", "og_low": 1.048, "og_high": 1.056, "fg_low": 1.01, "fg_high": 1.016, "ibu_low": 18, "ibu_high": 28, "srm_low": 14, "srm_high": 28, "abv_low": 4.5, "abv_high": 5.6 }, { "SubCategoryID": "4C", "SubCategoryName": "Schwarzbier (Black Beer)", "og_low": 1.046, "og_high": 1.052, "fg_low": 1.01, "fg_high": 1.016, "ibu_low": 22, "ibu_high": 32, "srm_low": 17, "srm_high": 30, "abv_low": 4.4, "abv_high": 5.4 }, { "SubCategoryID": "5A", "SubCategoryName": "Maibock/Helles Bock", "og_low": 1.064, "og_high": 1.072, "fg_low": 1.011, "fg_high": 1.018, "ibu_low": 23, "ibu_high": 35, "srm_low": 6, "srm_high": 11, "abv_low": 6.3, "abv_high": 7.4 }, { "SubCategoryID": "5B", "SubCategoryName": "Traditional Bock", "og_low": 1.064, "og_high": 1.072, "fg_low": 1.013, "fg_high": 1.019, "ibu_low": 20, "ibu_high": 27, "srm_low": 14, "srm_high": 22, "abv_low": 6.3, "abv_high": 7.2 }, { "SubCategoryID": "5C", "SubCategoryName": "Doppelbock", "og_low": 1.072, "og_high": 1.112, "fg_low": 1.016, "fg_high": 1.024, "ibu_low": 16, "ibu_high": 26, "srm_low": 6, "srm_high": 25, "abv_low": 7, "abv_high": 10 }, { "SubCategoryID": "5D", "SubCategoryName": "Eisbock", "og_low": 1.078, "og_high": 1.12, "fg_low": 1.02, "fg_high": 1.035, "ibu_low": 25, "ibu_high": 35, "srm_low": 18, "srm_high": 30, "abv_low": 9, "abv_high": 14 }, { "SubCategoryID": "6A", "SubCategoryName": "Cream Ale", "og_low": 1.042, "og_high": 1.055, "fg_low": 1.006, "fg_high": 1.012, "ibu_low": 15, "ibu_high": 20, "srm_low": 3, "srm_high": 5, "abv_low": 4.2, "abv_high": 5.6 }, { "SubCategoryID": "6B", "SubCategoryName": "Blonde Ale", "og_low": 1.038, "og_high": 1.054, "fg_low": 1.008, "fg_high": 1.013, "ibu_low": 15, "ibu_high": 28, "srm_low": 3, "srm_high": 6, "abv_low": 3.8, "abv_high": 5.5 }, { "SubCategoryID": "6C", "SubCategoryName": "Kölsch", "og_low": 1.044, "og_high": 1.05, "fg_low": 1.007, "fg_high": 1.011, "ibu_low": 20, "ibu_high": 30, "srm_low": 4, "srm_high": 5, "abv_low": 4.4, "abv_high": 5.2 }, { "SubCategoryID": "6D", "SubCategoryName": "American Wheat or Rye Beer", "og_low": 1.04, "og_high": 1.055, "fg_low": 1.008, "fg_high": 1.013, "ibu_low": 15, "ibu_high": 30, "srm_low": 3, "srm_high": 6, "abv_low": 4, "abv_high": 5.5 }, { "SubCategoryID": "7A", "SubCategoryName": "Northern German Altbier", "og_low": 1.046, "og_high": 1.054, "fg_low": 1.01, "fg_high": 1.015, "ibu_low": 25, "ibu_high": 40, "srm_low": 13, "srm_high": 19, "abv_low": 4.5, "abv_high": 5.2 }, { "SubCategoryID": "7B", "SubCategoryName": "California Common Beer", "og_low": 1.048, "og_high": 1.054, "fg_low": 1.011, "fg_high": 1.014, "ibu_low": 30, "ibu_high": 45, "srm_low": 10, "srm_high": 14, "abv_low": 4.5, "abv_high": 5.5 }, { "SubCategoryID": "7C", "SubCategoryName": "Düsseldorf Altbier", "og_low": 1.046, "og_high": 1.054, "fg_low": 1.01, "fg_high": 1.015, "ibu_low": 35, "ibu_high": 50, "srm_low": 11, "srm_high": 17, "abv_low": 4.5, "abv_high": 5.2 }, { "SubCategoryID": "8A", "SubCategoryName": "Standard/Ordinary Bitter", "og_low": 1.032, "og_high": 1.04, "fg_low": 1.007, "fg_high": 1.011, "ibu_low": 25, "ibu_high": 35, "srm_low": 4, "srm_high": 14, "abv_low": 3.2, "abv_high": 3.8 }, { "SubCategoryID": "8B", "SubCategoryName": "Special/Best/Premium Bitter", "og_low": 1.04, "og_high": 1.048, "fg_low": 1.008, "fg_high": 1.012, "ibu_low": 25, "ibu_high": 40, "srm_low": 5, "srm_high": 16, "abv_low": 3.8, "abv_high": 4.6 }, { "SubCategoryID": "8C", "SubCategoryName": "Extra Special/Strong Bitter (English Pale Ale)", "og_low": 1.048, "og_high": 1.06, "fg_low": 1.01, "fg_high": 1.016, "ibu_low": 30, "ibu_high": 50, "srm_low": 6, "srm_high": 18, "abv_low": 4.6, "abv_high": 6.2 }, { "SubCategoryID": "9A", "SubCategoryName": "Scottish Light 60/-", "og_low": 1.03, "og_high": 1.035, "fg_low": 1.01, "fg_high": 1.013, "ibu_low": 10, "ibu_high": 20, "srm_low": 9, "srm_high": 17, "abv_low": 2.5, "abv_high": 3.2 }, { "SubCategoryID": "9B", "SubCategoryName": "Scottish Heavy 70/-", "og_low": 1.035, "og_high": 1.04, "fg_low": 1.01, "fg_high": 1.015, "ibu_low": 10, "ibu_high": 25, "srm_low": 9, "srm_high": 17, "abv_low": 3.2, "abv_high": 3.9 }, { "SubCategoryID": "9C", "SubCategoryName": "Scottish Export 80/-", "og_low": 1.04, "og_high": 1.054, "fg_low": 1.01, "fg_high": 1.016, "ibu_low": 15, "ibu_high": 30, "srm_low": 9, "srm_high": 17, "abv_low": 3.9, "abv_high": 5 }, { "SubCategoryID": "9D", "SubCategoryName": "Irish Red Ale", "og_low": 1.044, "og_high": 1.06, "fg_low": 1.01, "fg_high": 1.014, "ibu_low": 17, "ibu_high": 28, "srm_low": 9, "srm_high": 18, "abv_low": 4, "abv_high": 6 }, { "SubCategoryID": "9E", "SubCategoryName": "Strong Scotch Ale", "og_low": 1.07, "og_high": 1.13, "fg_low": 1.018, "fg_high": 1.056, "ibu_low": 17, "ibu_high": 35, "srm_low": 14, "srm_high": 25, "abv_low": 6.5, "abv_high": 10 }, { "SubCategoryID": "10A", "SubCategoryName": "American Pale Ale", "og_low": 1.045, "og_high": 1.06, "fg_low": 1.01, "fg_high": 1.015, "ibu_low": 30, "ibu_high": 45, "srm_low": 5, "srm_high": 14, "abv_low": 4.5, "abv_high": 6.2 }, { "SubCategoryID": "10B", "SubCategoryName": "American Amber Ale", "og_low": 1.045, "og_high": 1.06, "fg_low": 1.01, "fg_high": 1.015, "ibu_low": 25, "ibu_high": 40, "srm_low": 10, "srm_high": 17, "abv_low": 4.5, "abv_high": 6.2 }, { "SubCategoryID": "10C", "SubCategoryName": "American Brown Ale", "og_low": 1.045, "og_high": 1.06, "fg_low": 1.01, "fg_high": 1.016, "ibu_low": 20, "ibu_high": 40, "srm_low": 18, "srm_high": 35, "abv_low": 4.3, "abv_high": 6.2 }, { "SubCategoryID": "11A", "SubCategoryName": "Mild", "og_low": 1.03, "og_high": 1.038, "fg_low": 1.008, "fg_high": 1.013, "ibu_low": 10, "ibu_high": 25, "srm_low": 12, "srm_high": 25, "abv_low": 2.8, "abv_high": 4.5 }, { "SubCategoryID": "11B", "SubCategoryName": "Southern English Brown", "og_low": 1.033, "og_high": 1.042, "fg_low": 1.011, "fg_high": 1.014, "ibu_low": 12, "ibu_high": 20, "srm_low": 19, "srm_high": 35, "abv_low": 2.8, "abv_high": 4.1 }, { "SubCategoryID": "11C", "SubCategoryName": "Northern English Brown Ale", "og_low": 1.04, "og_high": 1.052, "fg_low": 1.008, "fg_high": 1.014, "ibu_low": 20, "ibu_high": 30, "srm_low": 12, "srm_high": 22, "abv_low": 4.2, "abv_high": 5.4 }, { "SubCategoryID": "12A", "SubCategoryName": "Brown Porter", "og_low": 1.04, "og_high": 1.052, "fg_low": 1.008, "fg_high": 1.014, "ibu_low": 18, "ibu_high": 35, "srm_low": 20, "srm_high": 30, "abv_low": 4, "abv_high": 5.4 }, { "SubCategoryID": "12B", "SubCategoryName": "Robust Porter", "og_low": 1.048, "og_high": 1.065, "fg_low": 1.012, "fg_high": 1.016, "ibu_low": 25, "ibu_high": 50, "srm_low": 22, "srm_high": 35, "abv_low": 4.8, "abv_high": 6.5 }, { "SubCategoryID": "12C", "SubCategoryName": "Baltic Porter", "og_low": 1.06, "og_high": 1.09, "fg_low": 1.016, "fg_high": 1.024, "ibu_low": 20, "ibu_high": 40, "srm_low": 17, "srm_high": 30, "abv_low": 5.5, "abv_high": 9.5 }, { "SubCategoryID": "13A", "SubCategoryName": "Dry Stout", "og_low": 1.036, "og_high": 1.05, "fg_low": 1.007, "fg_high": 1.011, "ibu_low": 30, "ibu_high": 45, "srm_low": 25, "srm_high": 40, "abv_low": 4, "abv_high": 5 }, { "SubCategoryID": "13B", "SubCategoryName": "Sweet Stout", "og_low": 1.044, "og_high": 1.06, "fg_low": 1.012, "fg_high": 1.024, "ibu_low": 20, "ibu_high": 40, "srm_low": 30, "srm_high": 40, "abv_low": 4, "abv_high": 6 }, { "SubCategoryID": "13C", "SubCategoryName": "Oatmeal Stout", "og_low": 1.048, "og_high": 1.065, "fg_low": 1.01, "fg_high": 1.018, "ibu_low": 25, "ibu_high": 40, "srm_low": 22, "srm_high": 40, "abv_low": 4.2, "abv_high": 5.9 }, { "SubCategoryID": "13D", "SubCategoryName": "Foreign Extra Stout", "og_low": 1.056, "og_high": 1.075, "fg_low": 1.01, "fg_high": 1.018, "ibu_low": 30, "ibu_high": 70, "srm_low": 30, "srm_high": 40, "abv_low": 5.5, "abv_high": 8 }, { "SubCategoryID": "13E", "SubCategoryName": "American Stout", "og_low": 1.05, "og_high": 1.075, "fg_low": 1.01, "fg_high": 1.022, "ibu_low": 35, "ibu_high": 75, "srm_low": 30, "srm_high": 40, "abv_low": 5, "abv_high": 7 }, { "SubCategoryID": "13F", "SubCategoryName": "Russian Imperial Stout", "og_low": 1.075, "og_high": 1.115, "fg_low": 1.018, "fg_high": 1.03, "ibu_low": 50, "ibu_high": 90, "srm_low": 30, "srm_high": 40, "abv_low": 8, "abv_high": 12 }, { "SubCategoryID": "14A", "SubCategoryName": "English IPA", "og_low": 1.05, "og_high": 1.075, "fg_low": 1.01, "fg_high": 1.018, "ibu_low": 40, "ibu_high": 60, "srm_low": 8, "srm_high": 14, "abv_low": 5, "abv_high": 7.5 }, { "SubCategoryID": "14B", "SubCategoryName": "American IPA", "og_low": 1.056, "og_high": 1.075, "fg_low": 1.01, "fg_high": 1.018, "ibu_low": 40, "ibu_high": 70, "srm_low": 6, "srm_high": 15, "abv_low": 5.5, "abv_high": 7.5 }, { "SubCategoryID": "14C", "SubCategoryName": "Imperial IPA", "og_low": 1.07, "og_high": 1.09, "fg_low": 1.01, "fg_high": 1.02, "ibu_low": 60, "ibu_high": 120, "srm_low": 8, "srm_high": 15, "abv_low": 7.5, "abv_high": 10 }, { "SubCategoryID": "15A", "SubCategoryName": "Weizen/Weissbier", "og_low": 1.044, "og_high": 1.052, "fg_low": 1.01, "fg_high": 1.014, "ibu_low": 8, "ibu_high": 15, "srm_low": 2, "srm_high": 8, "abv_low": 4.3, "abv_high": 5.6 }, { "SubCategoryID": "15B", "SubCategoryName": "Dunkelweizen", "og_low": 1.044, "og_high": 1.056, "fg_low": 1.01, "fg_high": 1.014, "ibu_low": 10, "ibu_high": 18, "srm_low": 14, "srm_high": 23, "abv_low": 4.3, "abv_high": 5.6 }, { "SubCategoryID": "15C", "SubCategoryName": "Weizenbock", "og_low": 1.064, "og_high": 1.09, "fg_low": 1.015, "fg_high": 1.022, "ibu_low": 15, "ibu_high": 30, "srm_low": 12, "srm_high": 25, "abv_low": 6.5, "abv_high": 8 }, { "SubCategoryID": "15D", "SubCategoryName": "Roggenbier (German Rye Beer)", "og_low": 1.046, "og_high": 1.056, "fg_low": 1.01, "fg_high": 1.014, "ibu_low": 10, "ibu_high": 20, "srm_low": 14, "srm_high": 19, "abv_low": 4.5, "abv_high": 6 }, { "SubCategoryID": "16A", "SubCategoryName": "Witbier", "og_low": 1.044, "og_high": 1.052, "fg_low": 1.008, "fg_high": 1.012, "ibu_low": 10, "ibu_high": 20, "srm_low": 2, "srm_high": 4, "abv_low": 4.5, "abv_high": 5.5 }, { "SubCategoryID": "16B", "SubCategoryName": "Belgian Pale Ale", "og_low": 1.048, "og_high": 1.054, "fg_low": 1.01, "fg_high": 1.014, "ibu_low": 20, "ibu_high": 30, "srm_low": 8, "srm_high": 14, "abv_low": 4.8, "abv_high": 5.5 }, { "SubCategoryID": "16C", "SubCategoryName": "Saison", "og_low": 1.048, "og_high": 1.065, "fg_low": 1.002, "fg_high": 1.012, "ibu_low": 20, "ibu_high": 35, "srm_low": 5, "srm_high": 14, "abv_low": 5, "abv_high": 7 }, { "SubCategoryID": "16D", "SubCategoryName": "Bière de Garde", "og_low": 1.06, "og_high": 1.08, "fg_low": 1.008, "fg_high": 1.016, "ibu_low": 18, "ibu_high": 28, "srm_low": 6, "srm_high": 19, "abv_low": 6, "abv_high": 8.5 }, { "SubCategoryID": "16E", "SubCategoryName": "Belgian Specialty Ale", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "17A", "SubCategoryName": "Berliner Weisse", "og_low": 1.028, "og_high": 1.032, "fg_low": 1.003, "fg_high": 1.006, "ibu_low": 3, "ibu_high": 8, "srm_low": 2, "srm_high": 3, "abv_low": 2.8, "abv_high": 3.8 }, { "SubCategoryID": "17B", "SubCategoryName": "Flanders Red Ale", "og_low": 1.048, "og_high": 1.057, "fg_low": 1.002, "fg_high": 1.012, "ibu_low": 10, "ibu_high": 25, "srm_low": 10, "srm_high": 16, "abv_low": 4.6, "abv_high": 6.5 }, { "SubCategoryID": "17C", "SubCategoryName": "Flanders Brown Ale/Oud Bruin", "og_low": 1.04, "og_high": 1.074, "fg_low": 1.008, "fg_high": 1.012, "ibu_low": 20, "ibu_high": 25, "srm_low": 15, "srm_high": 22, "abv_low": 4, "abv_high": 8 }, { "SubCategoryID": "17D", "SubCategoryName": "Straight (Unblended) Lambic", "og_low": 1.04, "og_high": 1.054, "fg_low": 1.001, "fg_high": 1.01, "ibu_low": 0, "ibu_high": 0, "srm_low": 3, "srm_high": 7, "abv_low": 5, "abv_high": 6.5 }, { "SubCategoryID": "17E", "SubCategoryName": "Gueuze", "og_low": 1.04, "og_high": 1.06, "fg_low": 1, "fg_high": 1.006, "ibu_low": 0, "ibu_high": 0, "srm_low": 3, "srm_high": 7, "abv_low": 5, "abv_high": 8 }, { "SubCategoryID": "17F", "SubCategoryName": "Fruit Lambic", "og_low": 1.04, "og_high": 1.06, "fg_low": 1, "fg_high": 1.01, "ibu_low": 0, "ibu_high": 0, "srm_low": 3, "srm_high": 7, "abv_low": 5, "abv_high": 7 }, { "SubCategoryID": "18A", "SubCategoryName": "Belgian Blond Ale", "og_low": 1.062, "og_high": 1.075, "fg_low": 1.008, "fg_high": 1.018, "ibu_low": 15, "ibu_high": 30, "srm_low": 4, "srm_high": 7, "abv_low": 6, "abv_high": 7.5 }, { "SubCategoryID": "18B", "SubCategoryName": "Belgian Dubbel", "og_low": 1.062, "og_high": 1.075, "fg_low": 1.008, "fg_high": 1.018, "ibu_low": 15, "ibu_high": 25, "srm_low": 10, "srm_high": 17, "abv_low": 6.3, "abv_high": 7.6 }, { "SubCategoryID": "18C", "SubCategoryName": "Belgian Tripel", "og_low": 1.075, "og_high": 1.085, "fg_low": 1.008, "fg_high": 1.014, "ibu_low": 20, "ibu_high": 40, "srm_low": 4.5, "srm_high": 7, "abv_low": 7.5, "abv_high": 9.5 }, { "SubCategoryID": "18D", "SubCategoryName": "Belgian Golden Strong Ale", "og_low": 1.07, "og_high": 1.095, "fg_low": 1.005, "fg_high": 1.016, "ibu_low": 22, "ibu_high": 35, "srm_low": 3, "srm_high": 6, "abv_low": 7.5, "abv_high": 10.5 }, { "SubCategoryID": "18E", "SubCategoryName": "Belgian Dark Strong Ale", "og_low": 1.075, "og_high": 1.11, "fg_low": 1.01, "fg_high": 1.024, "ibu_low": 20, "ibu_high": 30, "srm_low": 12, "srm_high": 22, "abv_low": 8, "abv_high": 11 }, { "SubCategoryID": "19A", "SubCategoryName": "Old Ale", "og_low": 1.06, "og_high": 1.09, "fg_low": 1.015, "fg_high": 1.022, "ibu_low": 30, "ibu_high": 60, "srm_low": 10, "srm_high": 22, "abv_low": 6, "abv_high": 9 }, { "SubCategoryID": "19B", "SubCategoryName": "English Barleywine", "og_low": 1.08, "og_high": 1.12, "fg_low": 1.018, "fg_high": 1.03, "ibu_low": 35, "ibu_high": 70, "srm_low": 8, "srm_high": 22, "abv_low": 8, "abv_high": 12 }, { "SubCategoryID": "19C", "SubCategoryName": "American Barleywine", "og_low": 1.08, "og_high": 1.12, "fg_low": 1.016, "fg_high": 1.03, "ibu_low": 50, "ibu_high": 120, "srm_low": 10, "srm_high": 19, "abv_low": 8, "abv_high": 12 }, { "SubCategoryID": "20A", "SubCategoryName": "FRUIT BEER", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "21A", "SubCategoryName": "Spice, Herb, or Vegetable Beer", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "21B", "SubCategoryName": "Christmas/Winter Specialty Spiced Beer", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "22A", "SubCategoryName": "Classic Rauchbier", "og_low": 1.05, "og_high": 1.057, "fg_low": 1.012, "fg_high": 1.016, "ibu_low": 20, "ibu_high": 30, "srm_low": 12, "srm_high": 22, "abv_low": 4.8, "abv_high": 6 }, { "SubCategoryID": "22B", "SubCategoryName": "Other Smoked Beer", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "22C", "SubCategoryName": "Wood-Aged Beer", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "23A", "SubCategoryName": "Specialty Beer", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "24A", "SubCategoryName": "Dry Mead", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "24B", "SubCategoryName": "Semi-sweet Mead", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "24C", "SubCategoryName": "Sweet Mead", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "25A", "SubCategoryName": "Cyser", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "25B", "SubCategoryName": "Pyment", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "25C", "SubCategoryName": "Other Fruit Melomel", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "26A", "SubCategoryName": "Metheglin", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "26B", "SubCategoryName": "Braggot", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "26C", "SubCategoryName": "Open Category Mead", "og_low": 0, "og_high": 0, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 0, "abv_high": 0 }, { "SubCategoryID": "27A", "SubCategoryName": "Common Cider ", "og_low": 1.045, "og_high": 1.065, "fg_low": 1, "fg_high": 1.02, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 5, "abv_high": 8 }, { "SubCategoryID": "27B", "SubCategoryName": "English Cider ", "og_low": 1.05, "og_high": 1.075, "fg_low": 0.995, "fg_high": 1.01, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 6, "abv_high": 9 }, { "SubCategoryID": "27C", "SubCategoryName": "French Cider", "og_low": 1.05, "og_high": 1.065, "fg_low": 1.01, "fg_high": 1.02, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 3, "abv_high": 6 }, { "SubCategoryID": "27D", "SubCategoryName": "Common Perry", "og_low": 1.05, "og_high": 1.06, "fg_low": 1, "fg_high": 1.02, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 5, "abv_high": 7.2 }, { "SubCategoryID": "27E", "SubCategoryName": "Traditional Perry ", "og_low": 1.05, "og_high": 1.07, "fg_low": 1, "fg_high": 1.02, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 5, "abv_high": 9 }, { "SubCategoryID": "28A", "SubCategoryName": "New England Cider", "og_low": 1.06, "og_high": 1.1, "fg_low": 0.995, "fg_high": 1.01, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 7, "abv_high": 13 }, { "SubCategoryID": "28B", "SubCategoryName": "Fruit Cider", "og_low": 1.045, "og_high": 1.07, "fg_low": 0.995, "fg_high": 1.01, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 5, "abv_high": 9 }, { "SubCategoryID": "28C", "SubCategoryName": "Applewine", "og_low": 1.07, "og_high": 1.1, "fg_low": 0.995, "fg_high": 1.01, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 9, "abv_high": 12 }, { "SubCategoryID": "28D", "SubCategoryName": "Other Specialty Cider/Perry", "og_low": 1.045, "og_high": 1.1, "fg_low": 0, "fg_high": 0, "ibu_low": 0, "ibu_high": 0, "srm_low": 0, "srm_high": 0, "abv_low": 5, "abv_high": 12 }],

    // Calculates the percent based on the charts low and high values
    CalculatePercentForChart: function (low, middle, high) {
        if (middle <= low)
            return 0;

        if (middle >= high)
            return 100;

        return ((middle - low) / (high - low)) * 100;
    },

    // Checks if a value is inbetween two values
    CalculateMatch: function (low, current, high) {
        if (low == 0 && high == 0) {
            return true;
        }
        return ((current >= low) && (current <= high));
    },

    // finds the midpoint between two numbers
    CalculateMidPoint: function (low, high) {
        return (((high - low) / 2) + low);
    },

    // returns the difference between two numbers
    Difference: function (numberA, numberB) {
        return Math.abs(numberA - numberB);
    },

    // used for sorting 
    CompareTotalPoints: function (a, b) {
        if (a.totalPoints < b.totalPoints) {
            return -1;
        }
        if (a.totalPoints > b.totalPoints) {
            return 1;
        }
        return 0;
    },

    isWater: function (recipe) {
        if (recipe == null)
            return true;

        if (recipe.Og <= 1.000 &&
        recipe.Fg <= 1.000 &&
        recipe.Ibu <= 0)
            return true;
        else
            return false;
    },

    // Creates the chart using the template and appends to the passed in object
    create: function (bjcpStyle, recipe, objectToAddItTo) {



        if (bjcpStyle == null || recipe == null)
            return;

        var og_startpercent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.og_low_graph, bjcpStyle.og_low, StyleChart.bjcpStyleChartValues.og_high_graph);
        var og_endpercent = (StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.og_low_graph, bjcpStyle.og_high, StyleChart.bjcpStyleChartValues.og_high_graph) - og_startpercent);
        if (bjcpStyle.og_low == 0 && bjcpStyle.og_high == 0) { og_endpercent = 100; }
        var og_currentbrew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.og_low_graph, recipe.Og, StyleChart.bjcpStyleChartValues.og_high_graph);
        var og_match = (StyleChart.CalculateMatch(bjcpStyle.og_low, recipe.Og, bjcpStyle.og_high)) ? "block" : "none";

        var fg_startpercent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.fg_low_graph, bjcpStyle.fg_low, StyleChart.bjcpStyleChartValues.fg_high_graph);
        var fg_endpercent = (StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.fg_low_graph, bjcpStyle.fg_high, StyleChart.bjcpStyleChartValues.fg_high_graph) - fg_startpercent);
        if (bjcpStyle.fg_low == 0 && bjcpStyle.fg_high == 0) { fg_endpercent = 100; }
        var fg_currentbrew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.fg_low_graph, recipe.Fg, StyleChart.bjcpStyleChartValues.fg_high_graph);
        var fg_match = (StyleChart.CalculateMatch(bjcpStyle.fg_low, recipe.Fg, bjcpStyle.fg_high)) ? "block" : "none";

        var ibu_startpercent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.ibu_low_graph, bjcpStyle.ibu_low, StyleChart.bjcpStyleChartValues.ibu_high_graph);
        var ibu_endpercent = (StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.ibu_low_graph, bjcpStyle.ibu_high, StyleChart.bjcpStyleChartValues.ibu_high_graph) - ibu_startpercent);
        if (bjcpStyle.ibu_low == 0 && bjcpStyle.ibu_high == 0) { ibu_endpercent = 100; }
        var ibu_currentbrew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.ibu_low_graph, recipe.Ibu, StyleChart.bjcpStyleChartValues.ibu_high_graph);
        var ibu_match = (StyleChart.CalculateMatch(bjcpStyle.ibu_low, recipe.Ibu, bjcpStyle.ibu_high)) ? "block" : "none";

        var srm_startpercent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.srm_low_graph, bjcpStyle.srm_low, StyleChart.bjcpStyleChartValues.srm_high_graph);
        var srm_endpercent = (StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.srm_low_graph, bjcpStyle.srm_high, StyleChart.bjcpStyleChartValues.srm_high_graph) - srm_startpercent);
        if (bjcpStyle.srm_low == 0 && bjcpStyle.srm_high == 0) { srm_endpercent = 100; }
        var srm_currentbrew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.srm_low_graph, recipe.Srm, StyleChart.bjcpStyleChartValues.srm_high_graph);
        var srm_match = (StyleChart.CalculateMatch(bjcpStyle.srm_low, recipe.Srm, bjcpStyle.srm_high)) ? "block" : "none";

        var abv_startpercent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.abv_low_graph, bjcpStyle.abv_low, StyleChart.bjcpStyleChartValues.abv_high_graph);
        var abv_endpercent = (StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.abv_low_graph, bjcpStyle.abv_high, StyleChart.bjcpStyleChartValues.abv_high_graph) - abv_startpercent);
        if (bjcpStyle.abv_low == 0 && bjcpStyle.abv_high == 0) { abv_endpercent = 100; }
        var abv_currentbrew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.abv_low_graph, recipe.Abv.toFixed(1), StyleChart.bjcpStyleChartValues.abv_high_graph);
        var abv_match = (StyleChart.CalculateMatch(bjcpStyle.abv_low, recipe.Abv, bjcpStyle.abv_high)) ? "block" : "none";

        var lowBGGUStyle = StyleChart.CalculateBUGURatio(bjcpStyle.og_high, bjcpStyle.ibu_low);
        var highBGGUStyle = StyleChart.CalculateBUGURatio(bjcpStyle.og_low, bjcpStyle.ibu_high);
        var bggu_startpercent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.bggu_low_graph, lowBGGUStyle, StyleChart.bjcpStyleChartValues.bggu_high_graph);
        var bggu_endpercent = (StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.bggu_low_graph, highBGGUStyle, StyleChart.bjcpStyleChartValues.bggu_high_graph) - bggu_startpercent);
        if (lowBGGUStyle == 0 && highBGGUStyle == 0) { bggu_endpercent = 100; } else if (bjcpStyle.SubCategoryID == "0A") { bggu_endpercent = 0; }
        var bggu_currentbrew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.bggu_low_graph, recipe.BgGu, StyleChart.bjcpStyleChartValues.bggu_high_graph);
        var bggu_match = (StyleChart.CalculateMatch(lowBGGUStyle, recipe.BgGu, highBGGUStyle)) ? "block" : "none";

        var bjcpStyleConstants = [
        { styleElement: "OG", currentBrewValue: recipe.Og.toFixed(3), styleStart: bjcpStyle.og_low.toFixed(3), styleEnd: bjcpStyle.og_high.toFixed(3), styleElementStartValue: StyleChart.bjcpStyleChartValues.og_low_graph.toFixed(3), styleElementEndValue: StyleChart.bjcpStyleChartValues.og_high_graph.toFixed(3), styleElementStartPercent: og_startpercent, styleElementEndPercent: og_endpercent, styleMarkerPercent: og_currentbrew, styleMatch: og_match },
        { styleElement: "FG", currentBrewValue: recipe.Fg.toFixed(3), styleStart: bjcpStyle.fg_low.toFixed(3), styleEnd: bjcpStyle.fg_high.toFixed(3), styleElementStartValue: StyleChart.bjcpStyleChartValues.fg_low_graph.toFixed(3), styleElementEndValue: StyleChart.bjcpStyleChartValues.fg_high_graph.toFixed(3), styleElementStartPercent: fg_startpercent, styleElementEndPercent: fg_endpercent, styleMarkerPercent: fg_currentbrew, styleMatch: fg_match },
        { styleElement: "IBU", currentBrewValue: recipe.Ibu.toFixed(0), styleStart: bjcpStyle.ibu_low, styleEnd: bjcpStyle.ibu_high, styleElementStartValue: StyleChart.bjcpStyleChartValues.ibu_low_graph, styleElementEndValue: StyleChart.bjcpStyleChartValues.ibu_high_graph, styleElementStartPercent: ibu_startpercent, styleElementEndPercent: ibu_endpercent, styleMarkerPercent: ibu_currentbrew, styleMatch: ibu_match },
        { styleElement: "SRM", currentBrewValue: recipe.Srm.toFixed(1), styleStart: bjcpStyle.srm_low, styleEnd: bjcpStyle.srm_high, styleElementStartValue: StyleChart.bjcpStyleChartValues.srm_low_graph, styleElementEndValue: StyleChart.bjcpStyleChartValues.srm_high_graph, styleElementStartPercent: srm_startpercent, styleElementEndPercent: srm_endpercent, styleMarkerPercent: srm_currentbrew, styleMatch: srm_match },
        { styleElement: "ABV", currentBrewValue: recipe.Abv.toFixed(1), styleStart: bjcpStyle.abv_low, styleEnd: bjcpStyle.abv_high, styleElementStartValue: StyleChart.bjcpStyleChartValues.abv_low_graph, styleElementEndValue: StyleChart.bjcpStyleChartValues.abv_high_graph, styleElementStartPercent: abv_startpercent, styleElementEndPercent: abv_endpercent, styleMarkerPercent: abv_currentbrew, styleMatch: abv_match },
        { styleElement: "BG:GU", currentBrewValue: recipe.BgGu.toFixed(2), styleStart: lowBGGUStyle.toFixed(2), styleEnd: highBGGUStyle.toFixed(2), styleElementStartValue: StyleChart.bjcpStyleChartValues.bggu_low_graph, styleElementEndValue: StyleChart.bjcpStyleChartValues.bggu_high_graph, styleElementStartPercent: bggu_startpercent, styleElementEndPercent: bggu_endpercent, styleMarkerPercent: bggu_currentbrew, styleMatch: bggu_match },
        ];

        $("#bjcpStyleTitle").tmpl(bjcpStyle).appendTo(objectToAddItTo);
        $("#bjcpStyleTemplate").tmpl(bjcpStyleConstants).appendTo(objectToAddItTo);

        // Add the tipsy mouse over flyout
        $('.bjcp-slider').tipsy();
    },

    // returns all styles sorted by closest match
    SortStylesByClosestMatch: function (recipe) {

        var styles = StyleChart.allStyles;

        for (var i = 0; i < styles.length; i++) {

            if (styles[i].og_low == 0 && styles[i].og_high == 0 && styles[i].fg_low == 0 && styles[i].fg_high == 0 && styles[i].ibu_low, styles[i].ibu_high == 0 && styles[i].srm_low == 0 && styles[i].srm_high == 0 && styles[i].abv_low == 0 && styles[i].abv_high == 0) {
                styles[i].totalPoints = 99999;
                continue;
            }

            og_midpoint = StyleChart.CalculateMidPoint(styles[i].og_low, styles[i].og_high);
            fg_midpoint = StyleChart.CalculateMidPoint(styles[i].fg_low, styles[i].fg_high);
            ibu_midpoint = StyleChart.CalculateMidPoint(styles[i].ibu_low, styles[i].ibu_high);
            srm_midpoint = StyleChart.CalculateMidPoint(styles[i].srm_low, styles[i].srm_high);
            abv_midpoint = StyleChart.CalculateMidPoint(styles[i].abv_low, styles[i].abv_high);

            og_percent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.og_low_graph, og_midpoint, StyleChart.bjcpStyleChartValues.og_high_graph);
            og_percent_brew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.og_low_graph, recipe.Og, StyleChart.bjcpStyleChartValues.og_high_graph);
            fg_percent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.fg_low_graph, fg_midpoint, StyleChart.bjcpStyleChartValues.fg_high_graph);
            fg_percent_brew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.fg_low_graph, recipe.Fg, StyleChart.bjcpStyleChartValues.fg_high_graph);
            ibu_percent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.ibu_low_graph, ibu_midpoint, StyleChart.bjcpStyleChartValues.ibu_high_graph);
            ibu_percent_brew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.ibu_low_graph, recipe.Ibu, StyleChart.bjcpStyleChartValues.ibu_high_graph);
            srm_percent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.srm_low_graph, srm_midpoint, StyleChart.bjcpStyleChartValues.srm_high_graph);
            srm_percent_brew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.srm_low_graph, recipe.Srm, StyleChart.bjcpStyleChartValues.srm_high_graph);
            abv_percent = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.abv_low_graph, abv_midpoint, StyleChart.bjcpStyleChartValues.abv_high_graph);
            abv_percent_brew = StyleChart.CalculatePercentForChart(StyleChart.bjcpStyleChartValues.abv_low_graph, recipe.Abv, StyleChart.bjcpStyleChartValues.abv_high_graph);

            og_diff = (recipe.Og > styles[i].og_low && recipe.Og < styles[i].og_high) ? 0 : StyleChart.Difference(og_percent, og_percent_brew);
            fg_diff = (recipe.Fg > styles[i].fg_low && recipe.Fg < styles[i].fg_high) ? 0 : StyleChart.Difference(fg_percent, fg_percent_brew);
            ibu_diff = (recipe.Ibu > styles[i].ibu_low && recipe.Ibu < styles[i].ibu_high) ? 0 : StyleChart.Difference(ibu_percent, ibu_percent_brew);
            srm_diff = (recipe.Srm > styles[i].srm_low && recipe.Srm < styles[i].srm_high) ? 0 : StyleChart.Difference(srm_percent, srm_percent_brew);
            abv_diff = (recipe.Abv > styles[i].abv_low && recipe.Abv < styles[i].abv_high) ? 0 : StyleChart.Difference(abv_percent, abv_percent_brew);

            totalPointsForThisStyle = og_diff + fg_diff + ibu_diff + srm_diff + abv_diff;

            styles[i].totalPoints = totalPointsForThisStyle;
        }

        return styles.sort(StyleChart.CompareTotalPoints);
    },

    // gets a style based on it's subCategoryID
    GetStyle: function (subCategoryID) {
        for (var i = 0; i < StyleChart.allStyles.length; i++) {
            if (StyleChart.allStyles[i].SubCategoryID == subCategoryID)
                return StyleChart.allStyles[i];
        }
        return null;
    },

    CalculateBUGURatio: function (og, ibu) {
        if (og <= 1 && ibu > 0)
            return 1.0;
        if (og == 0 || ibu == 0)
            return 0;
        return ibu / ((og - 1) * 1000);
    }
};