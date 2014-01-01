<?php
/**
 * The base configurations of the WordPress.
 *
 * This file has the following configurations: MySQL settings, Table Prefix,
 * Secret Keys, WordPress Language, and ABSPATH. You can find more information
 * by visiting {@link http://codex.wordpress.org/Editing_wp-config.php Editing
 * wp-config.php} Codex page. You can get the MySQL settings from your web host.
 *
 * This file is used by the wp-config.php creation script during the
 * installation. You don't have to use the web site, you can just copy this file
 * to "wp-config.php" and fill in the values.
 *
 * @package WordPress
 */

// ** MySQL settings - You can get this info from your web host ** //
/** The name of the database for WordPress */
define('DB_NAME', 'coffxbmx_otlkfdr');

/** MySQL database username */
define('DB_USER', 'coffxbmx_otlkfdr');

/** MySQL database password */
define('DB_PASSWORD', 'PSj6R1W[0@');

/** MySQL hostname */
define('DB_HOST', 'localhost');

/** Database Charset to use in creating database tables. */
define('DB_CHARSET', 'utf8');

/** The Database Collate type. Don't change this if in doubt. */
define('DB_COLLATE', '');

/**#@+
 * Authentication Unique Keys and Salts.
 *
 * Change these to different unique phrases!
 * You can generate these using the {@link https://api.wordpress.org/secret-key/1.1/salt/ WordPress.org secret-key service}
 * You can change these at any point in time to invalidate all existing cookies. This will force all users to have to log in again.
 *
 * @since 2.6.0
 */
define('AUTH_KEY',         'jcwj4jctonxjxw3c0fjhrzpclb3yefl91hve6qbcjllt1o5rlst3h1rhgpzeu4b7');
define('SECURE_AUTH_KEY',  'pwoputxjrrwb6kxf06sfggckoqi9l85j7qet3y7qaucldtavlzy7q9xiczu7jibn');
define('LOGGED_IN_KEY',    '73sdpnda5kjhghyqp3hau19rdtv9l9t3qakwi8fp9rknaokbpeekcptormvxrxba');
define('NONCE_KEY',        'clu2egen7hqfqnjpqwsaaiedhdszrakxhxb6ra0wgxurobpv8p3rsu25xpjnw1ck');
define('AUTH_SALT',        'dnqcghjrl0lrzuxwjmdwtle1zt5cufmofnhoossuubgf3uskfs92uxhlu0w8pmen');
define('SECURE_AUTH_SALT', 'fb3ldebficyqasu8uerdnqsplfx1ep8unyvnj1pxjnfeomlir9dqzr2h2uesidtb');
define('LOGGED_IN_SALT',   'whqmlgelerlmni7broj6nz6dx65qwl8yapxxsrv7ddm2tvgia4pu1mnywsogeode');
define('NONCE_SALT',       '9oortyejb6lp7z1yqxrr8bnvuz03fe52pyjf4ugjfhc1tt6gwwuubtdhshnz5gzr');

/**#@-*/

/**
 * WordPress Database Table prefix.
 *
 * You can have multiple installations in one database if you give each a unique
 * prefix. Only numbers, letters, and underscores please!
 */
$table_prefix  = 'wp_';

/**
 * WordPress Localized Language, defaults to English.
 *
 * Change this to localize WordPress. A corresponding MO file for the chosen
 * language must be installed to wp-content/languages. For example, install
 * de_DE.mo to wp-content/languages and set WPLANG to 'de_DE' to enable German
 * language support.
 */
define('WPLANG', '');

/**
 * For developers: WordPress debugging mode.
 *
 * Change this to true to enable the display of notices during development.
 * It is strongly recommended that plugin and theme developers use WP_DEBUG
 * in their development environments.
 */
define('WP_DEBUG', false);

/* That's all, stop editing! Happy blogging. */

/** Absolute path to the WordPress directory. */
if ( !defined('ABSPATH') )
	define('ABSPATH', dirname(__FILE__) . '/');

/** Sets up WordPress vars and included files. */
require_once(ABSPATH . 'wp-settings.php');
