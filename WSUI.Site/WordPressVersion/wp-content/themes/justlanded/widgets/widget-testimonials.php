<?php
/*-----------------------------------------------------------------------------------
	Plugin Name: Testimonials Widget
	Plugin URI: http://www.shapingrain.com
	Description: Displays testimonials sourced from the landing page testimonials block.
	Version: 1.0
	Author: ShapingRain
	Author URI: http://www.shapingrain.com
-----------------------------------------------------------------------------------*/

// Add justlanded_testimonials_widgets function to widgets_init, this will load the widget.
add_action( 'widgets_init', 'justlanded_testimonials_widget' );


// Register the widget.
function justlanded_testimonials_widget() {
    register_widget( 'justlanded_Testimonials_Widget' );
}

// Extend WP_Widget with our widget.
class justlanded_testimonials_widget extends WP_Widget {


    /*-----------------------------------------------------------------------------------*/
    /*	Widget Setup
    /*-----------------------------------------------------------------------------------*/

    function justlanded_Testimonials_Widget() {

        // Widget setup
        $widget_ops = array( 'classname' => 'justlanded_testimonials_widget', 'description' => __('Testimonials from the testimonials landing page block.', 'justlanded') );

        // Widget UI
        $control_ops = array( 'width' => 300, 'height' => 350, 'id_base' => 'justlanded_testimonials_widget' );

        // Widget name and description
        $this->WP_Widget( 'justlanded_testimonials_widget', __('JustLanded - Testimonials Widget', 'justlanded'), $widget_ops, $control_ops );
    }

    /*-----------------------------------------------------------------------------------*/
    /*	Display The Widget To The Front End
    /*-----------------------------------------------------------------------------------*/

    function widget( $args, $instance ) {
        global $data;
        extract( $args );
        $title = apply_filters( 'widget_title', empty($instance['title']) ? 'Testimonials' : $instance['title'], $instance, $this->id_base);

        if ( ! $number = absint( $instance['number'] ) ) $number = 5;

        echo $before_widget;
        echo $before_title . $instance["title"] . $after_title;

        $testimonials = $data['testimonials']; //get the testimonials from settings
        if (is_array($testimonials)) {
            $testimonials = array_slice($testimonials, 0, $number);

            foreach ($testimonials as $testimonial) { ?>
                <blockquote>
                    <q><?php echo do_shortcode(stripslashes(@$testimonial['description']));?></q>
                    <footer><div><?php echo stripslashes(@$testimonial['title']);?></div> <?php echo stripslashes(@$testimonial['subtitle']);?></footer>
                </blockquote>
            <?php }
            echo $after_widget;
        }
    }

    /*-----------------------------------------------------------------------------------*/
    /*	Update The Widget With New Options
    /*-----------------------------------------------------------------------------------*/

    function update( $new_instance, $old_instance ) {

        $instance = $old_instance;
        $instance['title'] = strip_tags( $new_instance['title'] );
        $instance['number'] = strip_tags( $new_instance['number'] );

        return $instance;
    }

    /*-----------------------------------------------------------------------------------*/
    /*	Widget Settings
    /*-----------------------------------------------------------------------------------*/
    function form( $instance ) {
        $title = isset($instance['title']) ? esc_attr($instance['title']) : 'Testimonials';
        $number = isset($instance['number']) ? absint($instance['number']) : 5;

        ?>
    <p><label for="<?php echo $this->get_field_id('title'); ?>"><?php _e('Title:', 'justlanded'); ?></label>
        <input class="widefat" id="<?php echo $this->get_field_id('title'); ?>" name="<?php echo $this->get_field_name('title'); ?>" type="text" value="<?php echo $title; ?>" /></p>



    <p><label for="<?php echo $this->get_field_id('number'); ?>"><?php _e('Amount of testimonials to show:', 'justlanded'); ?></label>
        <input id="<?php echo $this->get_field_id('number'); ?>" name="<?php echo $this->get_field_name('number'); ?>" type="text" value="<?php echo $number; ?>" size="3" /></p>


    <?php
    }
}
?>