<?php
/*-----------------------------------------------------------------------------------
	Plugin Name: Recent Comments w/Avatar Thumbnails Widget
	Plugin URI: http://www.shapingrain.com
	Description: Displays recent comments (with avatars).
	Version: 1.0
	Author: ShapingRain
	Author URI: http://www.shapingrain.com
-----------------------------------------------------------------------------------*/

// Add justlanded_recent_comments_widgets function to widgets_init, this will load the widget.
add_action( 'widgets_init', 'justlanded_recent_comments_widget' );


// Register the widget.
function justlanded_recent_comments_widget() {
    register_widget( 'justlanded_Recent_comments_Widget' );
}

// Extend WP_Widget with our widget.
class justlanded_recent_comments_widget extends WP_Widget {


    /*-----------------------------------------------------------------------------------*/
    /*	Widget Setup
    /*-----------------------------------------------------------------------------------*/

    function justlanded_Recent_Comments_Widget() {

        // Widget setup
        $widget_ops = array( 'classname' => 'justlanded_recent_comments_widget', 'description' => __('The most recent comments on your site, with avatars.', 'justlanded') );

        // Widget UI
        $control_ops = array( 'width' => 300, 'height' => 350, 'id_base' => 'justlanded_recent_comments_widget' );

        // Widget name and description
        $this->WP_Widget( 'justlanded_recent_comments_widget', __('JustLanded - Recent Comments Widget', 'justlanded'), $widget_ops, $control_ops );
    }

    /*-----------------------------------------------------------------------------------*/
    /*	Display The Widget To The Front End
    /*-----------------------------------------------------------------------------------*/

    function widget( $args, $instance ) {
        extract( $args );
        $title = apply_filters( 'widget_title', empty($instance['title']) ? 'Recent Comments' : $instance['title'], $instance, $this->id_base);

        if ( ! $number = absint( $instance['number'] ) ) $number = 5;

        $recent_comments = get_comments( array(
            'number'    =>  $number,
            'status'    => 'approve'
        ) );

        echo $before_widget;

        echo $before_title . $instance["title"] . $after_title;


        echo "<ul class=\"recent_comments\">\n";
        foreach ( $recent_comments as $comment )
        {
            $post = get_post($comment->comment_post_ID, 'OBJECT');
            ?>
        <li class="justlanded-recent-commment-item">
            <?php echo get_avatar( $comment->comment_author_email, 60); ?>
            <p>
                <span><?php echo $comment->comment_author ?></span>
                <a href="<?php print get_permalink($post->ID); ?>"><?php print $post->post_title; ?></a>
            </p>
        </li>
        <?php
        }

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
        $title = isset($instance['title']) ? esc_attr($instance['title']) : 'Recent Comments';
        $number = isset($instance['number']) ? absint($instance['number']) : 5;

        ?>
    <p><label for="<?php echo $this->get_field_id('title'); ?>"><?php _e('Title:', 'justlanded'); ?></label>
        <input class="widefat" id="<?php echo $this->get_field_id('title'); ?>" name="<?php echo $this->get_field_name('title'); ?>" type="text" value="<?php echo $title; ?>" /></p>



    <p><label for="<?php echo $this->get_field_id('number'); ?>"><?php _e('Amount of comments to show:', 'justlanded'); ?></label>
        <input id="<?php echo $this->get_field_id('number'); ?>" name="<?php echo $this->get_field_name('number'); ?>" type="text" value="<?php echo $number; ?>" size="3" /></p>


    <?php
    }
}
?>