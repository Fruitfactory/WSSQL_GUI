<?php

/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


$versions_xml  = getcwd().'/downloads/clicktwice/full/version_info.xml';
$path_first = 'http://outlookfinder.com/downloads/clicktwice/full/1033/';
$path_second = '/outlookfinder.exe';

$versions_xml_trial  = getcwd().'/downloads/clicktwice/trial/version_info.xml';
$path_first_trial = 'http://outlookfinder.com/downloads/clicktwice/trial/1033/';

if(!function_exists('getDownloadUrl')){ 

    function getDownloadUrl($verxml, $p_f, $p_s){
            if(!file_exists($verxml))
            {
                    $mes = 'Versions file not found: '.$verxml;
                    return '';
            }

            $xml = simplexml_load_file($verxml);
            $count = $xml->product->version->count();
            if($count > 0){
                    $ver = $xml->product->version[$count -1];
                    $verapp = $ver['name'];
                    return $p_f.$verapp.$p_s;  	
            }
            return '';

    }
}

if(!function_exists('getDownloadUrlForLastVersion')){ 
    function getDownloadUrlForLastVersion(){
            global $versions_xml, $path_first, $path_second;
            return getDownloadUrl($versions_xml,$path_first,$path_second);		
    }
}

if(!function_exists('getDownloadUrlforTrial')){ 
    function getDownloadUrlforTrial(){
            global $versions_xml_trial, $path_first_trial, $path_second;
            return getDownloadUrl($versions_xml_trial,$path_first_trial,$path_second);	
    }
}