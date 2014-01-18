<!--Start of Navigation Elements-->
<nav id="navigation_elements">
    <div class="row">
        <!--Start of Main Navigation-->
        <div id="main_nav">
            <?php
            if (has_nav_menu('main-menu')) {
				$this_menu = "";
				if (isset($data['menu_custom_main'])) $this_menu = $data['menu_custom_main'];
                wp_nav_menu(array('theme_location' => 'main-menu', 'menu' => $this_menu));
            } else {
                if (is_user_logged_in()) {
                    echo '<ul id="menu-main" class="menu"><li class="menu-item"><a href="#"><?php _e("Please activate a menu from Appearance &rarr; Menus", "justlanded"); ?></a></li></ul>';
                }
            }
            ?>
        </div><!--End of Main Navigation-->
        <?php include TEMPLATEPATH . DIRECTORY_SEPARATOR . 'blocks' . DIRECTORY_SEPARATOR . 'block_header_social.php'; // header social icons, if activated ?>
    </div>
</nav><!--End of Navigation Elements-->
