
// use this to redirect from "Login Page" only in order to save the state on server side session
// because blazor's NavigateTo() won't perform a postback request. The function below performs a postback request
//and runs above code to save data in server side session.
window.clientJsMethods = {
    RedirectTo: function (path) {
        window.location = path;
    }
};

