<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
//
include_once (getcwd().'/wp-content/themes/justlanded/PaymentSettings.php');

$customerEmail = null;
$defaultCount = 1;
$fullName = null;

if($_POST["email"]){
    debug_log($_POST["email"], true);
    $customerEmail = $_POST["email"];
}

if($_POST["full_name"]){
    debug_log($_POST["full_name"], true);
    $fullName = $_POST["full_name"];
}

// Client has successfully paid for the product
debug_log('Creating product Information to send.',true);

// This calls the function in PaymentSettings.php that
// creates the product keys and send them to the user
//if(IsEmailExistTEST($customerEmail))
//{
//    UpdateLicensingTEST($defaultCount, $customerEmail, $fullName, $fullName,$customerEmail);
//}
//else

if(!empty($customerEmail) && !IsEmailExist($customerEmail) ){
    
    SendPKeys($defaultCount, $customerEmail, $fullName, $fullName,$customerEmail,true);
    debug_log('paychecker finished.',true,true);
    
}

header("HTTP/1.1 200 OK");
exit;

?>
