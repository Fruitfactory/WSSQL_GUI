<aside id="sidebar" class="three columns">
<?php if ( is_active_sidebar('primary-widget-area') ) : ?>
<div id="primary" class="widget-area">
<ul class="sid">
<?php dynamic_sidebar('primary-widget-area'); ?>
</ul>
</div>
<?php endif; ?>
</aside>