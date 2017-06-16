/*
** //product like counter update added by Md. Minul Islam sohel Bs-23
    
*/


var ProductLike = {
    loadWaiting: false,
    usepopupnotifications: false,
    like_unlike_selector: '',

    init: function (usepopupnotifications, like_unlike_selector) {
        this.loadWaiting = false;
        this.usepopupnotifications = usepopupnotifications;
        this.like_unlike_selector = like_unlike_selector;

    },
    displayLike: function () {
        $('.ajax-loading-block-window').hide('fast');
        setTimeout(function () { $('.like-block-window').show(); }, 150);
        setTimeout(function () { $('.like-block-window').hide('fast'); }, 650);
    },
    displayUnLike: function () {
        $('.ajax-loading-block-window').hide('fast');
        setTimeout(function () { $('.un-like-block-window').show(); }, 150);
        setTimeout(function () { $('.un-like-block-window').hide('fast'); }, 650);
       
    },
    setLoadWaiting: function (display) {
        displayAjaxLoading(display);
        this.loadWaiting = display;
    },

    //add a product to the cart/wishlist from the catalog pages
    like: function (urllike, like_unlike_selector) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        this.init(false, like_unlike_selector);
        $.ajax({
            cache: false,
            url: urllike,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },


    unlike: function (urlunlike, like_unlike_selector) {
        if (this.loadWaiting != false) {
            return;
        }
        this.setLoadWaiting(true);
        this.init(false, like_unlike_selector);
        $.ajax({
            cache: false,
            url: urlunlike,
            type: 'post',
            success: this.success_process,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },
    guestlike: function () {
        displayPopupNotification("Please <a href=\"/login\" class=\"ico-login\" style=\"color:red;text-decoration: underline;\"> Log in</a> to like the products", 'Alert', true);

    },


    success_process: function (response) {
        if (response.message) {
            //display notification
            if (response.success == true) {
                //success
                if (response.productlikecounthtml) {
                    $(ProductLike.like_unlike_selector).html(response.productlikecounthtml);
                    $('.liked-qty').html(response.likecounthtml);
                    $(ProductLike.like_unlike_selector).toggle();
                }

                if (response.likeproperty == true) {
                    ProductLike.displayLike();
                }
                else
                    ProductLike.displayUnLike();
            }
            else {
                //error
                if (ProductLike.usepopupnotifications == true) {
                    displayPopupNotification(response.message, 'error', true);
                }
                else {
                    //no timeout for errors
                    displayBarNotification(response.message, 'error', 0);
                }
            }
            return false;
        }
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    },

    resetLoadWaiting: function () {
        ProductLike.setLoadWaiting(false);
    },

    ajaxFailure: function () {
        alert('Failed to like or unlike the product. Please refresh the page and try one more time.');
    }
};