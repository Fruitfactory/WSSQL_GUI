$("#submit").click(function(){
            var emailVal = $("#useremail").val();
            if(!isEmail(emailVal)){
                $("#validateResult").text("Email is not valid").show().fadeOut(1000);
                return;
            }
            //method=limelm.pkey.find&version_id=1432&email=yariki4@gmail.com&api_key=1m115715277fb67cc5a51.73953699
            //{method:methodApi,version_id:version,email:emailVal,api_key:apiKey,format:'json'},
            //'?method=limelm.pkey.find&version_id=1432&email='+emailVal+'&api_key=1m115715277fb67cc5a51.73953699&format=json' ,
            // + '?method=limelm.pkey.find&version_id='+version+'&email='+emailVal+'&api_key='+apiKey+'&format=json'
            var urlLimeLM = 'http://outlookfinder.com/findChecker.php';//'http://www.outlookfinder.dev/findChecker.php'; //
            
            $.ajax({
               type : 'GET',
               dataType: 'json',
               crossDomain:true,
               url:urlLimeLM,
               data:{userEmail:emailVal},
               complete:function(response, textStatus){
                        var pStatus = $.parseJSON(response.responseText);
                        if(pStatus === null)
                            return;
                        switch(pStatus.stat){
                            case 'ok':
                                var valNotify = $("#notify_url").val();
                                valNotify = valNotify + "&userEmail="+emailVal;
                                $("#notify_url").val(valNotify);
                                $("#signup_form").submit();
                                break;
                            case 'fail':
                                $("#validateResult").text(pStatus.message).show().fadeOut(1000);
                                break;
                        }
                    }
            });
            
        });