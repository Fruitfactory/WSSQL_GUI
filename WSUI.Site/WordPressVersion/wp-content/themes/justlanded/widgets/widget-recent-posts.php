<?php
/*-----------------------------------------------------------------------------------
	Plugin Name: Recent Posts w/Thumbnails Widget
	Plugin URI: http://www.shapingrain.com
	Description: Displays recent blog posts (with thumbnails).
	Version: 1.0
	Author: ShapingRain
	Author URI: http://www.shapingrain.com
-----------------------------------------------------------------------------------*/

// Add justlanded_recent_blog_widgets function to widgets_init, this will load the widget.
add_action( 'widgets_init', 'justlanded_recent_blog_widget' );


// Register the widget.
function justlanded_recent_blog_widget() {
	register_widget( 'justlanded_Recent_Blog_Widget' );
}

// Extend WP_Widget with our widget.
class justlanded_recent_blog_widget extends WP_Widget {


/*-----------------------------------------------------------------------------------*/
/*	Widget Setup
/*-----------------------------------------------------------------------------------*/
	
	function justlanded_Recent_Blog_Widget() {
	
		// Widget setup
		$widget_ops = array( 'classname' => 'justlanded_recent_blog_widget', 'description' => __('The most recent posts on your site, with post thumbnails.', 'justlanded') );

		// Widget UI
		$control_ops = array( 'width' => 300, 'height' => 350, 'id_base' => 'justlanded_recent_blog_widget' );

		// Widget name and description
		$this->WP_Widget( 'justlanded_recent_blog_widget', __('JustLanded - Recent Posts Widget', 'justlanded'), $widget_ops, $control_ops );
	}

    /*-----------------------------------------------------------------------------------*/
    /*	Display The Widget To The Front End
    /*-----------------------------------------------------------------------------------*/

    function widget( $args, $instance ) {
        extract( $args );
        $title = apply_filters( 'widget_title', empty($instance['title']) ? 'Recent Posts' : $instance['title'], $instance, $this->id_base);

        if ( ! $number = absint( $instance['number'] ) ) $number = 5;

        $args=array(
            'showposts' => $number
        );

        $justlanded_widget = null;
        $justlanded_widget = new WP_Query($args);

        echo $before_widget;

        echo $before_title . $instance["title"] . $after_title;

        echo "<ul>\n";
        while ( $justlanded_widget->have_posts() )
        {
            $justlanded_widget->the_post();

            ?>

        <li class="justlanded-recent-item">
            <a href="<?php the_permalink(); ?>" title="<?php the_title_attribute(); ?>" rel="bookmark"><?php the_post_thumbnail('custom-tiny'); ?>
                <p><?php the_title(); ?> <span><?php the_time(get_option('date_format')); ?></span></p>
            </a>
        </li>

        <?php

        }

        wp_reset_query();

        echo "</ul>\n";

        echo $after_widget;
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
        $title = isset($instance['title']) ? esc_attr($instance['title']) : 'Recent Posts';
        $number = isset($instance['number']) ? absint($instance['number']) : 5;

        ?>
    <p><label for="<?php echo $this->get_field_id('title'); ?>"><?php _e('Title:', 'justlanded'); ?></label>
        <input class="widefat" id="<?php echo $this->get_field_id('title'); ?>" name="<?php echo $this->get_field_name('title'); ?>" type="text" value="<?php echo $title; ?>" /></p>



    <p><label for="<?php echo $this->get_field_id('number'); ?>"><?php _e('Amount of posts to show:', 'justlanded'); ?></label>
        <input id="<?php echo $this->get_field_id('number'); ?>" name="<?php echo $this->get_field_name('number'); ?>" type="text" value="<?php echo $number; ?>" size="3" /></p>


    <?php
    }


}
?>