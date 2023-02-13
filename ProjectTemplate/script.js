
// script for photography-app
// Version 1.0.0
// Authors: Big Data Dynasty group CIS 440

var userType = null
var firstName = null
var lastName = null
var email = null
var username = null
var password = null
var photoStyle = null
var photoType = null
var expPreference = null //Preferred photographer experience
var custBudget = null
var custAvailability = null

function parseSurvey(){
    username = $("#user").val()
    console.log(username)
    password = $("#pass").val()
    userType = $(".radioButtons1:checked").val()
    photoStyle = $(".radioButtons2:checked").val()
    photoType = $(".radioButtons3:checked").val()
    expPreference = $(".radioButtons4:checked").val()
    firstName = $("#firstName").val()
    lastName = $("#lastName").val()
    email = $("#emailAddress").val()
    console.log(email)
    custAvailability = $("#freeText").val()

    
    // $("#tester").append("User Information:")
    redirectToIndex()

    $("#userInfo").html("<p id='finalUserInfo'>Welcome" + username + "!" + " This is our photography appointment system. Below is your information" + "\n" + userType + "</p>");

    


 }



function TestButtonHandler() {
    var webMethod = "ProjectServices.asmx/TestConnection";
    var parameters = "{}";

    //jQuery ajax method
    $.ajax({
        type: "POST",
        url: webMethod,
        data: parameters,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var responseFromServer = msg.d;
            alert(responseFromServer);
        },
        error: function (e) {
            alert("this code will only execute if javascript is unable to access the webservice");
        }
    });
}

function redirectToIndex() {
    window.location.href='index.html';
}