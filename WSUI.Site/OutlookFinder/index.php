<?php 
require("PaymentSettings.php");
$path_trial = getDownloadUrlforTrial();

?>


<!DOCTYPE html>
<html>

<head>
    <meta charset='utf-8' />
    <meta http-equiv="X-UA-Compatible" content="chrome=1" />
    <meta name="description" content="Outlook Finder:" />
    <link href="css/metro-bootstrap.css" rel="stylesheet">
    <link href="css/metro-bootstrap-responsive.css" rel="stylesheet">
    <link href="css/docs.css" rel="stylesheet">
    <link href="js/prettify/prettify.css" rel="stylesheet">

    <!-- Load JavaScript Libraries -->
    <script src="js/jquery/jquery.min.js"></script>
    <script src="js/jquery/jquery.widget.min.js"></script>
    <script src="js/jquery/jquery.mousewheel.js"></script>
    <script src="js/prettify/prettify.js"></script>
	<script src="js/docs.js"></script>

    <!-- Metro UI CSS JavaScript plugins -->
    <script src="js/metro/metro-loader.js"></script>
    <title>Outlook Finder Plugin</title>
</head>

<body class="metro">
    <header class="bg-dark" data-load="header.html"></header>
    <div class="page">
        <!--page begin-->

        <div class="grid container">
            <div class="row">
               <div class="bg-white">
                    <div class="padding20 introduce bg-cyan">
                        <h1 class="ntm text-center fg-white">Outlook Finder</h1>
                    </div>
                </div>
                

            </div>

            <div class="row">
                <div class="span4 bg-red">
                    <a class="span4 padding20 subheader text-center fg-white introduce_r downloadlink"
                       href="<?= $path_trial ?>">Trial version
                       <span class="icon-download-2"></span>
                    </a>
                </div>
                <div class="span4 bg-cyan text-center">
                    <a class="span4 padding20 subheader text-center fg-white introduce_r downloadlink"
                    href="payment.php">
                        Buy version
                    </a>
                </div>
            </div>

            <div class="row border-top">

                <h2>Screenshots</h2>


                <div class="row border-top image-container full-size">
                    <img src="images/screenshots/ooutlookplugin.png" class="span12" />
                    <div class="overlay">
                        Plugin in Outlook
                    </div>
                </div>


                <div class="row border-top image-container full-size">
                    <img src="images/screenshots/standalone.png" class="span12" />
                    <div class="overlay">
                        StandAlone application
                    </div>
                </div>

            </div>
        </div>
        <div class="page-footer bg-cyan">
            <div class="page-footer-content">
                Outlook Finder Plugin gui maintained by Fruitfactory. Published with <a href="http://pages.github.com">GitHub Pages</a>
            </div>
        </div>

    </div>
    <!--page end-->
</body>
</html>
