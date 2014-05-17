<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

$userEmail = $_GET['userEmail'];

$response = array();
// This calls the function in PaymentSettings.php that
// creates the product keys and send them to the user
if ($userEmail) {
    require(dirname(__FILE__) . '/wp-content/themes/justlanded/LimeLM.php');

    //TODO: set your API key found in http://wyday.com/limelm/settings/
    LimeLM::SetAPIKey('1m115715277fb67cc5a51.73953699');

    //TODO: set the version id to generate & find keys for
    // the version id is found in the url. For example http://wyday.com/limelm/version/100/
    // the version id is 100.
    $version_id = '1432';
    $submitting = true;

    try {
        // Find product keys using the email
        $xml = new SimpleXMLElement(LimeLM::FindPKey($version_id, $userEmail));

        if ($xml['stat'] == 'ok') {
            $response = array('stat' => 'ok', 'count' => $xml->pkeys['total'], 'message' => 'Key was founded.');
        } else { //failure
            $response = array('stat' => 'fail', 'count' => 0, 'message' => $xml->err['code'] == '1' ? "Key wasn't  found for this email." : "Error code" . $xml->err['code']);
        }
    } catch (Exception $e) {
        $success = false;
        $response = array('stat' => 'fail', 'count' => 0, 'message' => $e->getMessage());
    }
    LimeLM::CleanUp();
} else {
    $response = array('stat' => 'fail', 'count' => 0, 'message' => 'Email is empty');
}
echo json_encode($response);
?>