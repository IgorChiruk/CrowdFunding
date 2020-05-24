$(function () {

    new WOW().init();

    IsLogin();

    $('#ThisUserLogout').click(function () {
        $.post("/Home/LogOut",
            null,
            function (data) {
                if (data) {
                    $('#AuthenticatedUser').hide();
                    $('#AnonimousUser').show();
                    $('#ThisUserName').text("Hello anonimous!");
                    $('#UsersLink').addClass("text-hide");
                    $('#MainPageHome').trigger("click"); 
                }        
            }, "json");         
    });

    $(document).on("ajaxSend", function (event, xhr, options) {
        if ((options.url == "/Home/Register") || (options.url == "/Home/Login")) { return false; }
        else
        {
            $('#MainDiv').removeClass("lightSpeedIn");
            $('#MainDiv').addClass("lightSpeedOut");
        }
    }).on("ajaxComplete", function (event, xhr, options) {
        if ((options.url == "/Home/Register") || (options.url == "/Home/Login")) { return false; }
        else
        { 
            $('#MainDiv').removeClass("lightSpeedOut");            
            $('#MainDiv').addClass("lightSpeedIn");
        }
    });

    $('#MainDiv').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', (function () {
        $('#MainDiv').removeClass("lightSpeedOut");
        $('#MainDiv').removeClass("lightSpeedIn"); }));
  
    $('#LoginInput').click(function () {  
        
        if (($('#Login').val()) == "") {
            $('#Login').addClass("is-invalid");
            $('#Password').removeClass("is-invalid");
            $('#LoginAlert').removeClass("text-hide");
            $('#LoginAlert').text("Enter Login");
            return false;
        }
        else if (($('#Password').val()) == "") {
            $('#Login').removeClass("is-invalid");
            $('#Password').addClass("is-invalid");
            $('#LoginAlert').removeClass("text-hide");
            $('#LoginAlert').text("Enter Password");
            return false;
        }

        var formData = $("#Form1").serializeArray();
        $.post("/Home/Login",
            formData,
            function (data) {
                if (data) {
                    $('#CloseLogin').trigger("click");            
                    IsLogin();
                };
                if (!data) {
                    $('#Login').addClass("is-invalid");
                    $('#Password').addClass("is-invalid");
                    $('#LoginAlert').removeClass("text-hide");
                    $('#LoginAlert').text("Wrong Login or Password");                                 
                }
            }, "json");      
    });

    $('#RegisterInput').click(function () {    
       
        if (($('#RegisterLogin').val()) == "") {
            $('#RegisterLogin').addClass("is-invalid");
            $('#RegisterPassword').removeClass("is-invalid");
            $('#ConfirmRegisterPassword').removeClass("is-invalid");
            $('#RegisterAlert').removeClass("text-hide");
            $('#RegisterAlert').text("Enter Login");
            return false;
        }
        else if (($('#RegisterPassword').val()) == "") {
            $('#RegisterLogin').removeClass("is-invalid");
            $('#RegisterPassword').addClass("is-invalid");
            $('#ConfirmRegisterPassword').removeClass("is-invalid");
            $('#RegisterAlert').removeClass("text-hide");
            $('#RegisterAlert').text("Enter Password");
            return false;
        } else if (($('#RegisterPassword').val()) != ($('#ConfirmRegisterPassword').val())) {
            $('#RegisterLogin').removeClass("is-invalid");
            $('#RegisterPassword').addClass("is-invalid");
            $('#ConfirmRegisterPassword').addClass("is-invalid");
            $('#RegisterAlert').removeClass("text-hide");
            $('#RegisterAlert').text("Passwords dont match");
            return false;
        }

        var formData = $("#Form2").serializeArray();
        $.post("/Home/Register",
            formData,
            function (data) {
                if (data) {
                    $('#CloseRegister').trigger("click");
                    IsLogin();
                };
                if (!data) {
                    $('#RegisterLogin').addClass("is-invalid");
                    $('#RegisterPassword').removeClass("is-invalid");
                    $('#ConfirmRegisterPassword').removeClass("is-invalid");
                    $('#RegisterAlert').removeClass("text-hide");
                    $('#RegisterAlert').text("Login already taken");
                }
            }, "json");
    });

});

function IsLogin() {
    $.post("/Home/GetUserName",
        null,
        function (data) {
            if (!data) {
                $('#AuthenticatedUser').hide();
                $('#AnonimousUser').show();
                $('#ThisUserName').text("Hello anonimous!");
            }
            else {
                $('#AnonimousUser').hide();
                $('#AuthenticatedUser').show();
                $('#ThisUserName').text("Hello " + data.UserName + "!");
                if (data.IsAdmin=="True") {
                    $('#UsersLink').removeClass("text-hide");
                }
            }
        }, "json");
       
}

function DeleteUseraaaaa(Id) {
    var formdata = new FormData();
    formdata.append('id', Id);
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Home/DeleteUser');
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var $UsersUpdate = $('#UsersUpdate'); 
            $UsersUpdate.trigger("click");
        }
    }
}

function AdminChange(Id) {
    var formdata = new FormData();
    formdata.append('id', Id);
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Home/AdminChange');
    xhr.send(formdata);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var $UsersUpdate = $('#UsersUpdate');
            $UsersUpdate.trigger("click");
        }
    }

}