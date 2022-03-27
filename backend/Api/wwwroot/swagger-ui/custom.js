﻿// This custom JS file allows us to do minor tweaks to how Swagger UI behaves.

(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            configureFavicon();
        });

        /**
         * Configures Swagger to use our custom favicon for the browser.
         */
        function configureFavicon() {
            // Set 16x16 favicon
            var linkIcon16 = document.createElement('link');
            linkIcon16.type = 'image/png';
            linkIcon16.rel = 'icon';
            linkIcon16.href = '/swagger-ui/favicon-16x16.png';
            linkIcon16.sizes = '16x16';
            document.getElementsByTagName('head')[0].appendChild(linkIcon16);

            // Set 32x32 favicon
            var linkIcon32 = document.createElement('link');
            linkIcon32.type = 'image/png';
            linkIcon32.rel = 'icon';
            linkIcon32.href = '/swagger-ui/favicon-32x32.png';
            linkIcon32.sizes = '32x32';
            document.getElementsByTagName('head')[0].appendChild(linkIcon32);
        }
    });
})();
