<?php
/**
 * Template Name: Payment page
 * Description: Payment page
 */
?>
<?php
@$active_profile = get_post_meta($post->ID, 'justlanded_meta_box_selectinstance_select', true);
do_action('justlanded_before_page_options'); // custom hook
if ($active_profile != 0) {
    $data = get_option(OPTIONSPREFIX.$active_profile);
} else {
    $data = get_option(OPTIONSPREFIX.SITE_DEFAULT_PROFILE);
}
do_action('justlanded_after_page_options'); // custom hook
?>

<?php
require( dirname(__FILE__).'/PaymentSettings.php');

// Only force HTTPS when using Autorize.Net and *not* in test mode
if (($UseAuthorizeNetCC || $UseAuthorizeNetBank) && !$AuthNetTest && !(isset($_SERVER['HTTPS']) && ($_SERVER['HTTPS'] == 'on')))
{
    header("HTTP/1.1 301 Moved Permanently");
    header('Location: '.'https://'.$_SERVER['SERVER_NAME'].$_SERVER['REQUEST_URI']);
    exit;
}


if ($UseAuthorizeNetCC || $UseAuthorizeNetBank)
{
    require dirname(__FILE__).'/CCProcessor.php';
    $cc_proc = new CCProcessor();
}

if (sizeof($_POST) && $cc_proc)
{
    $cc_proc->process();

    if (!$cc_proc->errors)
    {
        // redirect to success page
        header('Location: '.$ThankYouPage);
        exit;
    }

    if ($cc_proc->is_cc_charge)
        $AuthNetCCSelected = true;
    else
        $AuthNetBankSelected = true;
}

$TotalPaymentMethods = 0;

// Count the total payment methods (to hide/show the payment selector)
// Also select the default payment. Reorder the if-statements to select a different default.

if ($UseAuthorizeNetCC)
{
    if ($TotalPaymentMethods == 0 && !$AuthNetBankSelected)
        $AuthNetCCSelected = true;

    $TotalPaymentMethods++;
}

if ($UseAuthorizeNetBank)
{
    if ($TotalPaymentMethods == 0 && !$AuthNetCCSelected)
        $AuthNetBankSelected = true;

    $TotalPaymentMethods++;
}

if ($UsePayPal)
{
    if ($TotalPaymentMethods == 0)
        $PayPalSelected = true;

    $TotalPaymentMethods++;
}

if ($UseMoneybookers)
{
    if ($TotalPaymentMethods == 0)
        $MoneybookersSelected = true;

    $TotalPaymentMethods++;
}

if ($TotalPaymentMethods == 0)
{
    echo 'You must select at least one payment method in PaymentSettings.php (e.g. $UsePayPal=true;)';
    exit;
}

$single_price = sprintf('%u.%02u',$AppPrice[0],$AppPrice[1]);

?>



<?php get_header(); ?>

<!-- script begin -->
<script type="text/javascript" src="//wordpress.org/wp-includes/js/jquery/jquery.js"></script>
<script type="text/javascript" src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.2.min.js"></script>
<script type="text/javascript"><!--
    function calcTot(dollars, cents, quantity)
    {
        var n = Math.abs((cents * quantity) % 100);
        var zeroString = Math.pow(10, Math.max(0, 2 - Math.floor(n).toString().length)).toString().substr(1);
        return (dollars * quantity + Math.floor((cents * quantity) / 100)).toString()+'.'+zeroString+n;
    }

    jQuery(document).ready(function ($){

        $("#quantity").keypress(function(evt){
            var charCode = (evt.which) ? evt.which : event.keyCode;
            return !(charCode > 31 && (charCode < 48 || charCode > 57));
        });

        var total;

        $("#quantity").keyup(function(){
            total = calcTot(<?= $AppPrice[0].','.$AppPrice[1] ?>,$("#quantity").val());
            $("#total_price").text(total);

            <?php
            if ($UseMoneybookers)
            {
                if ($UsePayPal)
                    echo 'if ($("#Moneybookers").prop("checked")){$("#mb_amt").attr("value", total);}';
                else
                    echo '$("#mb_amt").attr("value", total);';
            }
            ?>
        });

        total = calcTot(<?= $AppPrice[0].','.$AppPrice[1] ?>,$("#quantity").val());
        $("#total_price").text(total);


        $("#signup_form").submit(function(){
            $("#submit").prop("disabled", true);
            <?php
            if ($TotalPaymentMethods == 1)
            {
                if ($UsePayPal)
                    echo '$("#progress").html("Proceeding to PayPal for checkout...");';
                else if ($UseMoneybookers)
                    echo '$("#progress").html("Proceeding to Moneybookers for checkout...");';
                else
                    echo '$("#progress").html("Placing your order - please wait...");';
            }
            else
            {?>
            if ($("#PayPal").prop("checked"))
                $("#progress").html("Proceeding to PayPal for checkout...");
            else if ($("#Moneybookers").prop("checked"))
                $("#progress").html("Proceeding to Moneybookers for checkout...");
            else
                $("#progress").html("Placing your order - please wait...");
            <?php
            }
            ?>
        });

        $("#Bank").click(function(){
            $("#signup_form").attr("action", "");
            $("#cc_info").slideUp("fast");
            $("#bank_info").slideDown("fast");
        });
        $("#Card").click(function(){
            $("#signup_form").attr("action", "");
            $("#bank_info").slideUp("fast");
            $("#cc_info").slideDown("fast");
        });
        $("#PayPal").click(function(){
            $("#signup_form").attr("action", "<?= $PayPalSandbox ? 'https://www.sandbox.paypal.com/cgi-bin/webscr':'https://www.paypal.com/cgi-bin/webscr' ?>");
            $("#cc_info").slideUp("fast");
            $("#bank_info").slideUp("fast");
            <?php if ($UseMoneybookers) echo '$("#mb_amt").attr("value", '.$single_price.');';?>
        });
        $("#Moneybookers").click(function(){
            $("#signup_form").attr("action", "https://www.moneybookers.com/app/payment.pl");
            $("#cc_info").slideUp("fast");
            $("#bank_info").slideUp("fast");

            $("#mb_amt").attr("value", total);
        });


        if ($("#PayPal").prop("checked")){
            $("#signup_form").attr("action", "<?= $PayPalSandbox ? 'https://www.sandbox.paypal.com/cgi-bin/webscr':'https://www.paypal.com/cgi-bin/webscr' ?>");
            $("#cc_info").hide();
            $("#bank_info").hide();
        }
        else if ($("#Bank").prop("checked")){
            $("#cc_info").hide();
            $("#bank_info").show();
        }
        else if ($("#Moneybookers").prop("checked")){
            $("#signup_form").attr("action", "https://www.moneybookers.com/app/payment.pl");
            $("#cc_info").hide();
            $("#bank_info").hide();
            $("#mb_amt").attr("value", total);
        }

        <?php
        if ($UseMoneybookers && $TotalPaymentMethods == 1)
            echo '$("#mb_amt").attr("value", total);';
        ?>
    });
    //--></script>
<style type="text/css">
    <!--
    .content{
        width:705px;
        margin:0 auto;
    }

    table{border-width:1px 0 0 0}
    td{border:0px}
    tr th{
        text-align:left;
        white-space:nowrap
    }
    #totalline{
        background:#F6F3FF;
        border-top:1px solid #9879FF;
        margin:15px 0;
        padding:5px
    }
    input,.large{font-size:110%}
    div.error{color:red}
    .error input{border:2px solid red}
    .error label{
        color:red;
        font-weight:bold
    }

    #cc-card,#cc-paypal,#cc-bank,#cc-mb,#cc-gc,.cc-secure{
        text-indent:-9999px;
        display:block;
        float:left;
        background-image:url(http://outlookfinder.com/images/sprites.png);
        height:35px;
        width:289px;
        margin-right:15px
    }
    #cc-bank{
        background-position:0 -35px;
        width:56px
    }
    #cc-paypal{
        background-position:0 -70px;
        width:55px
    }
    #cc-mb{
        background-position:0 -105px;
        width:55px
    }
    #cc-gc{
        background-position:0 -140px;
        width:55px
    }
    .cc-secure{
        background-position:0 -175px;
        height:32px;
        width:86px;
    }
    #cc-type{
        height:35px;
        margin-bottom:15px
    }
    #cc-type input{
        float:left;
        margin:8px 5px 0 10px;
        width:1em
    }
    label.link{
        cursor:pointer;
        cursor:hand;
    }
    table.price-table {
        border-left: 1px solid white;
        border-top: 1px solid white;
        margin: 20px auto;
        float: none;
        clear: both;
    }
    table.price-table tr th {
        background: none repeat scroll 0% 0% rgb(161, 191, 206);
        color: rgb(11, 58, 81);
        font-size: 18px;
        font-weight: bold;
        padding: 10px 0px 10px 20px;
        border-bottom: 1px solid white;
        border-right: 1px solid white;
    }
    
    table.price-table tr.bg-01 td {
        background: none repeat scroll 0% 0% rgb(235, 242, 247);
    }
    table.price-table td {
        font-size: 14px;
        color: rgb(18, 60, 81);
        padding: 10px 0px 10px 20px;
        border-bottom: 1px solid white;
        border-right: 1px solid white;
    }
    
    <?php
    if (!$AuthNetCCSelected)
        echo '#cc_info{display:none}';

    if (!$AuthNetBankSelected)
        echo '#bank_info{display:none}';
    ?>

    h3#su1,h3#su2,h3#su3{
        background:url(http://outlookfinder.com/images/sprites.png) no-repeat -91px -35px;
        color:#002C6D;
        margin:30px 0 20px;
        padding:2px 0 0 36px;
        min-height:26px;
        height:auto!important;
        height:26px
    }
    h3#su2{background-position:-91px -111px}
    h3#su3{background-position:-91px -187px}
    -->
</style>

<!-- script end -->


<article id="content"<?php if (justlanded_get_option('show_page_banner', 0, $data) == true) { echo ' class="row"'; } ?>>





<!-- payment begin html -->

    <div class="row">
        <div class="row">
        <?php
        if ($PayPalSelected)
            $defAction = $PayPalSandbox ? 'https://www.sandbox.paypal.com/cgi-bin/webscr' : 'https://www.paypal.com/cgi-bin/webscr';
        else if ($MoneybookersSelected)
            $defAction = 'https://www.moneybookers.com/app/payment.pl';
        else
            $defAction = '';
        ?>
        </div>

        <div class="row" style="text-align: center;">

        <form action="<?=$defAction?>" method="post" id="signup_form">
        <?php
        $headerShown = 1;

        if ($UseAuthorizeNetCC || $UseAuthorizeNetBank) {?>
        <h3 id="su<?= $headerShown++ ?>">Your info</h3>
        <table>
            <tr<?php if ($cc_proc->fname_err) echo ' class="error"'; ?>>
                <th><label for="form_first_name">First name</label></th>
                <td><input type="text" id="form_first_name" name="first_name" value="<?= htmlentities($cc_proc->first_name) ?>" size="24"/></td>
            </tr>
            <tr<?php if ($cc_proc->lname_err) echo ' class="error"'; ?>>
                <th><label for="form_last_name">Last name</label></th>
                <td><input type="text" id="form_last_name" name="last_name" value="<?= htmlentities($cc_proc->last_name) ?>" size="24"/></td>
            </tr>
            <tr<?php if ($cc_proc->email_err) echo ' class="error"'; ?>>
                <th><label for="form_email">Email</label></th>
                <td><input type="text" id="form_email" name="email" value="<?= htmlentities($cc_proc->email) ?>" size="24"/></td>
            </tr>
        </table>
        <?php } ?>

        <table class="price-table" style="width: 600px;">
            <tbody>
                <tr>
                    <th width="70%">Item name</th>
                    <th width="30%">Price (<?=$CurrencySign?>)</th>
                </tr>
                <tr class="bg-01">
                    <td><?= $AppName?></td>
                    <td class="large"><?=$CurrencySign?><?=$single_price?></td>
                </tr>
            </tbody>
        </table>
        
        

        <?php
        if ($TotalPaymentMethods == 1 && ($UseAuthorizeNetCC || $UseAuthorizeNetBank))
        {
        echo '<h3 id="su'.$headerShown.'">Enter your billing information &mdash; SECURE</h3>';
        }
        else if ($TotalPaymentMethods > 1) {

        // form the payment selection header
        $paymentHead = 'Payment method &mdash; ';

        $used = 0;

        if ($UseAuthorizeNetCC)
        {
            $paymentHead .= 'Credit card';
            $used++;
        }

        if ($UseAuthorizeNetBank)
        {
            if ($used > 0)
                $paymentHead .= $TotalPaymentMethods == $used + 1 ? ($TotalPaymentMethods > 2 ? ', or ' : ' or ') : ', ';
            $paymentHead .= 'Bank transfer';
            $used++;
        }

        if ($UsePayPal)
        {
            if ($used > 0)
                $paymentHead .= $TotalPaymentMethods == $used + 1 ? ($TotalPaymentMethods > 2 ? ', or ' : ' or ') : ', ';
            $paymentHead .= 'PayPal';
            $used++;
        }

        if ($UseMoneybookers)
        {
            if ($used > 0)
                $paymentHead .= $TotalPaymentMethods == $used + 1 ? ($TotalPaymentMethods > 2 ? ', or ' : ' or ') : ', ';
            $paymentHead .= 'Moneybookers';
        }
        ?>
        <h3 id="su<?= $headerShown ?>"><?= $paymentHead ?></h3>
        <div id="cc-type">
            <?php if ($UseAuthorizeNetCC) { ?>
                <input type="radio" id="Card" value="Card" name="mf_cardtype" title="Card"<?= $AuthNetCCSelected ? ' checked="checked"' : '' ?>/>
                <label for="Card" title="Pay with a credit card" id="cc-card" class="link">Visa / American Express / MasterCard / Discover / JCB</label>
            <?php } ?>
            <?php if ($UseAuthorizeNetBank) { ?>
                <input type="radio" id="Bank" value="Bank" name="mf_cardtype" title="Bank"<?= $AuthNetBankSelected ? ' checked="checked"' : '' ?>/>
                <label for="Bank" title="Pay with an eCheck (Bank transfer)" id="cc-bank" class="link">Pay with an eCheck (Bank transfer)</label>
            <?php } ?>
            <?php if ($UsePayPal) { ?>
                <div class="input-control radio default-style">
                    <label>
                        <input type="radio" id="PayPal" value="PayPal" name="mf_cardtype" title="PayPal"<?= $PayPalSelected ? ' checked="checked"' : '' ?>/>
                        <span class="check"></span>
                        <label for="PayPal" title="Pay with PayPal" id="cc-paypal" class="link">PayPal</label>
                    </label>
                </div>

            <?php } ?>
            <?php if ($UseMoneybookers) { ?>
                <div class="input-control radio default-style">
                    <label>
                        <input type="radio" id="Moneybookers" value="Moneybookers" name="mf_cardtype" title="Moneybookers"<?= $MoneybookersSelected ? ' checked="checked"' : '' ?>/>
                        <span class="check"></span>
                        <label for="Moneybookers" title="Pay with Moneybookers" id="cc-mb" class="link">Moneybookers</label>
                    </label>
                </div>
            <?php } ?>
        </div>
        <?php }

        if ($UseAuthorizeNetCC) { ?>
        <div id="cc_info">
            <table>
                <?php
                if ($TotalPaymentMethods == 1)
                {
                    echo '<tr><th>We accept</th><td colspan="2"><div id="cc-card">Visa / American Express / MasterCard / Discover / JCB</div><input type="hidden" name="mf_cardtype" value="Card"/></td></tr>';
                } ?>
                <tr<?php if ($cc_proc->cc_num_err) echo ' class="error"'; ?>>
                    <th><label for="form_ccnum">Card number</label></th>
                    <td><input id="form_ccnum" name="cc_num" size="22" type="text" value="<?= htmlentities($cc_proc->cc_num) ?>"/></td>
                    <td valign="top"><div class="cc-secure">Secure</div></td>
                </tr>
                <?php if ($cc_proc->cc_num_err){ ?><tr class="error"><th> </th><td><div class="error"><?= $cc_proc->cc_num_err_str ?></div></td></tr><?php } ?>
                <tr<?php if ($cc_proc->cc_exp_err) echo ' class="error"'; ?>>
                    <th><label for="form_cc_expires">Expires on</label></th>
                    <td>
                        <select id="form_cc_expires" name="cc_expires_m">
                            <option <?php if($cc_proc->cc_expires_m == 1 || !isset($cc_proc->cc_expires_m)) echo 'selected="selected" '; ?>value="1">1 - January</option>
                            <option <?php if($cc_proc->cc_expires_m == 2) echo 'selected="selected" '; ?>value="2">2 - February</option>
                            <option <?php if($cc_proc->cc_expires_m == 3) echo 'selected="selected" '; ?>value="3">3 - March</option>
                            <option <?php if($cc_proc->cc_expires_m == 4) echo 'selected="selected" '; ?>value="4">4 - April</option>
                            <option <?php if($cc_proc->cc_expires_m == 5) echo 'selected="selected" '; ?>value="5">5 - May</option>
                            <option <?php if($cc_proc->cc_expires_m == 6) echo 'selected="selected" '; ?>value="6">6 - June</option>
                            <option <?php if($cc_proc->cc_expires_m == 7) echo 'selected="selected" '; ?>value="7">7 - July</option>
                            <option <?php if($cc_proc->cc_expires_m == 8) echo 'selected="selected" '; ?>value="8">8 - August</option>
                            <option <?php if($cc_proc->cc_expires_m == 9) echo 'selected="selected" '; ?>value="9">9 - September</option>
                            <option <?php if($cc_proc->cc_expires_m == 10) echo 'selected="selected" '; ?>value="10">10 - October</option>
                            <option <?php if($cc_proc->cc_expires_m == 11) echo 'selected="selected" '; ?>value="11">11 - November</option>
                            <option <?php if($cc_proc->cc_expires_m == 12) echo 'selected="selected" '; ?>value="12">12 - December</option>
                        </select>

                        <select id="form_cc_expires_y" name="cc_expires_y">
                            <?php
                            // form the years dropdown list from the current year to 20 years from now
                            $exp_year = $cc_proc->cc_expires_y;
                            $currYear = (int)date('Y');
                            $endYear = $currYear + 21;

                            if (!$exp_year)
                                $exp_year = $currYear;

                            for (; $currYear < $endYear; $currYear++)
                            {
                                echo '<option';

                                if ($exp_year == $currYear)
                                    echo ' selected="selected"';

                                echo ' value="'.$currYear.'">'.$currYear.'</option>';
                            }
                            ?>
                        </select>

                    </td>
                </tr>
                <?php if ($cc_proc->cc_exp_err){ ?><tr class="error"><th> </th><td><div class="error">The date you've entered has expired. Either correct the date or use a card that hasn't expired.</div></td></tr><?php } ?>
                <tr<?php if ($cc_proc->cc_adr_err) echo ' class="error"'; ?>>
                    <th><label for="form_cc_adr">Street address</label></th>
                    <td><input id="form_cc_adr" name="cc_adr" size="22" type="text" value="<?= htmlentities($cc_proc->cc_adr) ?>"/></td>
                </tr>
                <tr<?php if ($cc_proc->cc_zip_err) echo ' class="error"'; ?>>
                    <th><label for="form_cc_zip">Billing ZIP code</label></th>
                    <td><input id="form_cc_zip" name="cc_zip" size="10" type="text" value="<?= htmlentities($cc_proc->cc_zip) ?>"/></td>
                </tr>
                <tr>
                    <th> </th>
                    <td><small>(or Postal Code if not in the USA)</small></td>
                </tr>
            </table>
        </div>
        <?php
        }

        if ($UseAuthorizeNetBank) {

        if ($TotalPaymentMethods == 1)
            echo '<input type="hidden" name="mf_cardtype" value="Bank"/>'; ?>
        <div id="bank_info">
            <table>
                <?php if ($cc_proc->bank_err_str){ ?><tr class="error"><th> </th><td><div class="error"><?= $cc_proc->bank_err_str ?></div></td></tr><?php } ?>
                <tr<?php if ($cc_proc->bank_acct_name_err) echo ' class="error"'; ?>>
                    <th><label for="form_bank_acct_name">Name on account</label></th>
                    <td><input id="form_bank_acct_name" name="bank_acct_name" size="27" type="text" value="<?= htmlentities($cc_proc->bank_acct_name) ?>"/></td>
                    <td valign="top" align="center"><div class="cc-secure">Secure</div></td>
                </tr>

                <tr<?php if ($cc_proc->bank_acct_type_err) echo ' class="error"'; ?>>
                    <th><label for="form_acct_type">Type of account</label></th>
                    <td>
                        <select id="form_acct_type" name="bank_acct_type">
                            <option <?php if(!$cc_proc->bank_acct_type) echo 'selected="selected" '; ?>value="">&mdash; Select one &mdash;</option>
                            <option <?php if($cc_proc->bank_acct_type == 'C') echo 'selected="selected" '; ?>value="C">Checking</option>
                            <option <?php if($cc_proc->bank_acct_type == 'S') echo 'selected="selected" '; ?>value="S">Savings</option>
                        </select>
                    </td>
                </tr>
                <?php if ($cc_proc->bank_acct_type_err){ ?><tr class="error"><th> </th><td><div class="error">Select the type of bank account from the dropdown.</div></td></tr><?php } ?>

                <tr<?php if ($cc_proc->bank_name_err) echo ' class="error"'; ?>>
                    <th><label for="form_bank_name">Bank Name</label></th>
                    <td><input id="form_bank_name" name="bank_name" size="27" type="text" value="<?= htmlentities($cc_proc->bank_name) ?>"/></td>
                </tr>
                <tr<?php if ($cc_proc->bank_route_num_err) echo ' class="error"'; ?>>
                    <th><label for="form_bank_route_num">ABA Routing Number</label></th>
                    <td><input id="form_bank_route_num" name="bank_route_num" size="11" type="text" value="<?= htmlentities($cc_proc->bank_route_num) ?>"/></td>
                </tr>
                <?php if ($cc_proc->bank_route_num_err){ ?><tr class="error"><th> </th><td><div class="error">The bank routing number is not valid &mdash; enter the correct 9 digits. You can find this number on your checks.</div></td></tr><?php } ?>

                <tr<?php if ($cc_proc->bank_acct_num_err) echo ' class="error"'; ?>>
                    <th><label for="form_bank_acct_num">Account Number</label></th>
                    <td><input id="form_bank_acct_num" name="bank_acct_num" size="18" type="text" value="<?= htmlentities($cc_proc->bank_acct_num) ?>"/></td>
                </tr>
            </table>
        </div>
        <?php
        }

        if ($UsePayPal || $UseMoneybookers) { ?>
        <input type="hidden" name="amount" value="<?=$single_price?>" id="mb_amt"/>
        <?php
        }

        if ($UsePayPal) { ?>
        <input type="hidden" name="cmd" value="_xclick"/>
        <input type="hidden" name="business" value="<?=$PayPalEmail?>"/>
        <input type="hidden" name="undefined_quantity" value="1"/>
        <input type="hidden" name="item_name" value="<?=$AppName?> license"/>
        <input type="hidden" name="item_number" value="1"/>
        <input type="hidden" name="no_shipping" value="1"/>
        <input type="hidden" name="no_note" value="1"/>
        <input type="hidden" name="currency_code" value="<?=$Currency?>"/>
        <input type="hidden" name="lc" value="US"/>
        <input type="hidden" name="rm" value="2"/>
        <input type="hidden" name="return" value="<?=$ThankYouPage?>"/>
        <input type="hidden" name="cancel_return" value="<?=$BuyPage?>"/>
        <input type="hidden" name="notify_url" value="<?=$CheckScript.'?paypal=1'?>"/>
        <?php
        if ($YourLogo)
            echo '<input type="hidden" name="image_url" value="'.$YourLogo.'"/>';
        }

        if ($UseMoneybookers) { ?>
        <input type="hidden" name="language" value="EN"/>
        <input type="hidden" name="currency" value="<?=$Currency?>"/>
        <input type="hidden" name="pay_to_email" value="<?=$MBEmail?>"/>
        <input type="hidden" name="return_url" value="<?=$ThankYouPage?>"/>
        <input type="hidden" name="cancel_url" value="<?=$BuyPage?>"/>
        <input type="hidden" name="status_url" value="<?=$CheckScript.'?moneybookers=1'?>"/>
        <input type="hidden" name="detail1_description" value="<?=$AppName?> licenses"/>
        <input type="hidden" name="detail1_text" value="<?=$AppName?> licenses"/>
        <input type="hidden" name="merchant_fields" value="quantity"/>
        <?php
        if ($YourLogo)
            echo '<input type="hidden" name="logo_url" value="'.$YourLogo.'"/>';
        }
        ?>

        <div><input type="image" src="http://outlookfinder.com/images/order.png" name="submit" id="submit" alt="Place my order" style="width:180px;height:38px;vertical-align:middle;"/>&nbsp;&nbsp;<strong id="progress"></strong></div>
        </form>
        </div>
        <?php
        // Show warnings and errors in the config.

        // Don't delete these lines. You won't see them on your
        // live site if you have everything configured correctly.

//        if (!function_exists('curl_init'))
//        echo '<p><strong><font color="#FF0000">The curl extension is required, but not installed on this server.</font></strong></p>';
//
//        if (!extension_loaded('simplexml'))
//        echo '<p><strong><font color="#FF0000">The SimpleXML extension is required, but not installed on this server.</font></strong></p>';
//
//
//        if ($UsePayPal && $PayPalSandbox)
//        echo '<p><strong><font color="#FF0000">PayPal sandbox is on</font> &mdash; payments will not be sent to your live account. To disable sandbox mode set <code>$PayPalSandbox = false;</code> in <code>PaymentSettings.php</code>.</strong></p>';
//
//        if (($UseAuthorizeNetCC || $UseAuthorizeNetBank) && $AuthNetTest)
//        {
//        echo '<p><strong><font color="#FF0000">Authorize.Net payments is in Test Mode</font> &mdash; payments will not be sent to your live account. To turn off test mode set <code>$AuthNetTest = false;</code> in <code>PaymentSettings.php</code>.</strong></p>';
//
//        if (!(isset($_SERVER['HTTPS']) && ($_SERVER['HTTPS'] == 'on')))
//        {
//            echo '<p><strong><font color="#FF0000">Authorize.Net payments <em>requires</em> a secure connection (https://).</font> &mdash; when you set <code>$AuthNetTest = false;</code> in <code>PaymentSettings.php</code> this page will automatically redirect you to the secure version of this page (https://) if you visit it from the non-secure version (http://). However, in test mode you can use the non-secure version to remove the hassle of creating self-signed SSL certificates.</strong></p>';
//        }
//        }
//
//        if ($debug)
//        echo '<p><strong><font color="#FF0000">Debugging turned on</font> &mdash; All actions will be logged to log file.</strong></p>';
//        ?>

    </div>

<!-- payment end html -->

<p></p>
</article>
<?php get_footer(); ?>