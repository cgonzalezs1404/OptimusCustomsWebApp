
// use this to redirect from "Login Page" only in order to save the state on server side session
// because blazor's NavigateTo() won't perform a postback request. The function below performs a postback request
//and runs above code to save data in server side session.
window.clientJsMethods = {
    RedirectTo: function (path) {
        window.location = path;
    }
};

window.clientCookiesMethods = {

    /**
     * SetCookie(cName, cValue, exDays)
     * Funcion para agregar una cookie al navegador
     * 
     * */
    SetCookie: function (cName, cVale, exDays) {
        const d = new Date();
        d.setTime(d.getTime() + (exDays * 24 * 60 * 60 * 1000));
        let expires = "expires=" + d.toUTCString();
        document.cookie = cName + "=" + cVale + ";" + expires + "path/";
        console.log("SetCookie: " + cName + "=" + cVale + ";" + expires + "path/");
    },

    /**
     * DeleteCookie(cName)
     * Funcion para borrar una cookie al navegador
     * 
     * */
    DeleteCookie: function (cName) {
        document.cookie = cName + "=;Expires=Thu, 01 Jan 1970 00:00:01 GMT;path=/";
        console.log("DeleteCookie: " + cName);
    },

    /**
     * GetCookie(cName)
     * Funcion para obtener una cookie al navegador
     * returns cookie
     * */
    GetCookie: function (cName) {
        var cookie;
        let name = cName + "=";
        let decodedCookie = decodeURIComponent(document.cookie);
        let ca = decodedCookie.split(";");
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                cookie = c.substring(name.length, c.length);
            }
        }
        console.log("GetCookie: " + cName + " " + cookie);
        return cookie;
    }
};

