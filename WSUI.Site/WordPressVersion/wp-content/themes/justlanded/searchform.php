<form method="get" id="searchform" action="<?php echo home_url( '/' ); ?>">
<div id="search-inputs">
<input type="text" name="s" id="s" value="Enter Search Term..." onfocus="if (this.value == 'Enter Search Term...') {this.value = '';}" onblur="if (this.value == '') {this.value = 'Enter Search Term...';}"/>
    <input id="button_search" type="button" value=" "/>
</div>
</form>