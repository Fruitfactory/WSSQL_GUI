<?php
/*-----------------------------------------------------------------------------------
	Plugin Name: Newsletter Widget
	Plugin URI: http://www.shapingrain.com
	Description: Displays a newsletter sign-up form
	Version: 1.0
	Author: ShapingRain
	Author URI: http://www.shapingrain.com
-----------------------------------------------------------------------------------*/

// Add justlanded_newsletter_widgets function to widgets_init, this will load the widget.
add_action( 'widgets_init', 'justlanded_newsletter_widget' );


// Register the widget.
function justlanded_newsletter_widget() {
    register_widget( 'justlanded_Newsletter_Widget' );
}

// Extend WP_Widget with our widget.
class justlanded_newsletter_widget extends WP_Widget {


    /*-----------------------------------------------------------------------------------*/
    /*	Widget Setup
    /*-----------------------------------------------------------------------------------*/

    function justlanded_Newsletter_Widget() {

        // Widget setup
        $widget_ops = array( 'classname' => 'justlanded_newsletter_widget', 'description' => __('Newsletter sign-up form.', 'justlanded') );

        // Widget UI
        $control_ops = array( 'width' => 300, 'height' => 350, 'id_base' => 'justlanded_newsletter_widget' );

        // Widget name and description
        $this->WP_Widget( 'justlanded_newsletter_widget', __('JustLanded - Sign-Up Form Widget', 'justlanded'), $widget_ops, $control_ops );
    }

    /*-----------------------------------------------------------------------------------*/
    /*	Display The Widget To The Front End
    /*-----------------------------------------------------------------------------------*/

    function widget( $args, $instance ) {
        global $data;
        extract( $args );
        $title = apply_filters( 'widget_title', empty($instance['title']) ? 'Sign-Up Form' : $instance['title'], $instance, $this->id_base);

        echo $before_widget;
        echo $before_title . $instance["title"] . $after_title;
        echo '<div class="newsletter_widget_form">'. "\n";
        echo justlanded_get_block(JUSTLANDED_BLOCKS_DIR . "block_newsletter_plain_form.php", array())."\n";
        echo "</div>\n";
        echo $after_widget;

    }

    /*-----------------------------------------------------------------------------------*/
    /*	Update The Widget With New Options
    /*-----------------------------------------------------------------------------------*/

    function update( $new_instance, $old_instance ) {

        $instance = $old_instance;
        $instance['title'] = strip_tags( $new_instance['title'] );
        return $instance;
    }

    /*-----------------------------------------------------------------------------------*/
    /*	Widget Settings
    /*-----------------------------------------------------------------------------------*/
    function form( $instance ) {
        $title = isset($instance['title']) ? esc_attr($instance['title']) : 'Sign-Up Form';

        ?>
        <p><label for="<?php echo $this->get_field_id('title'); ?>"><?php _e('Title:', 'justlanded'); ?></label>
            <input class="widefat" id="<?php echo $this->get_field_id('title'); ?>" name="<?php echo $this->get_field_name('title'); ?>" type="text" value="<?php echo $title; ?>" /></p>
        <?php
    }
}
?>