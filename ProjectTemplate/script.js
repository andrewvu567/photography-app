/* 
script for photography-app
Version 1.0.0
Authors: Big Data Dynasty group CIS 440
TEST TEST 
git push and commit test
var firstName = null
var lastName = null
var email = null
var username = null
var password = null
var photoStyle = null
var photoType = null
var custAvailability = null
var custBudget = null
var 
function parseSurvey(){
    
 }
 function sendEmail(){
 }
 */

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