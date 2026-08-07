// -------------------------------------------------------------------------------------------------------
// Layout Static Class
// -------------------------------------------------------------------------------------------------------
var Layout =
{
    /// Shows a Status Modal
    statusModal : function(message, onComplete) {
        var html = '<div class="working-dialog"><img src="/img/colorbox/loading.gif" width="32" height="32" /> <h3>' + message + '</h3></div>';
        $.colorbox({ html: html, speed: 100, opacity: .35, height: 75, overlayClose: false, closeButton: false, escKey: false, scrolling: false, onComplete: onComplete});
        $("#cboxLoadedContent").height(38);        
    }
};

var Message =
{
    /// Clears the message list
    clear: function () {
        $('div.messages').hide();
        $('div.messages li').remove();
    },

    /// Sets the target parent for the message container
    setParent: function(target) {
        $(target).prepend($('div.messages'));
    },

    /// Change the message element to use the smaller version
    useSmall: function() {
        $('div.messages').addClass('small');
    },

    /// Adds a message
    add: function (message, type) {
        // type: info, warn, error, success
        var cssClass = type;
        $('div.messages ul').append('<li class="' + type + '">' + message + '</li>');
        if (!$('.messages').is(':visible')) {
            $('.messages').show();
        }
    },
    
    // Adds an error message
    info : function(message) {
        Message.add(message, 'info');
    },

    // Adds an error message
    success : function(message) {
        Message.add(message, 'success');
    },

    // Adds an error message
    warn : function(message) {
        Message.add(message, 'warn');
    },

    // Adds an error message
    error : function(message) {
        Message.add(message, 'error');
    }
};
