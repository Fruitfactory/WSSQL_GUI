<?php
class LimeLM
{
	private static $api_key;
	private static $request;

	public static function SetAPIKey($api_key)
	{
		self::$api_key = $api_key;

		//NOTE: If you're using the self-hosted version of LimeLM (that is,
		//      LimeLM running on your own servers), then replace the URL with your own.

		// Almost all users should leave this line unchanged.
		self::$request = curl_init('https://wyday.com/limelm/api/rest/');
	}

	public static function CleanUp()
	{
		// close curl object
		curl_close(self::$request);
	}

	public static function AddFeature($version_id, $name, $required, $type = null)
	{
		$post_data = array(
			'method' => 'limelm.feature.add',
			'version_id' => $version_id,
			'name' => $name,
			'required' => $required ? 'true' : 'false'
		);

		if ($type)
			$post_data['type'] = $type;

		return self::PostRequest($post_data);
	}

	public static function DeleteFeature($feature_id)
	{
		$post_data = array(
			'method' => 'limelm.feature.delete',
			'feature_id' => $feature_id
		);

		return self::PostRequest($post_data);
	}

	public static function EditFeature($feature_id, $name = null, $required = null)
	{
		$post_data = array(
			'method' => 'limelm.feature.edit',
			'feature_id' => $feature_id
		);

		if ($name)
			$post_data['name'] = $name;

		if ($required !== null)
			$post_data['required'] = $required ? 'true' : 'false';

		return self::PostRequest($post_data);
	}

	public static function FindPKey($version_id, $email)
	{
		$post_data = array(
			'method' => 'limelm.pkey.find',
			'version_id' => $version_id,
			'email' => $email
		);

		return self::PostRequest($post_data);
	}

	public static function GeneratePKeys($version_id, $num_keys = 1, $num_acts = 1, $email = null, $feature_names = null, $feature_values = null)
	{
		$post_data = array(
			'method' => 'limelm.pkey.generate',
			'version_id' => $version_id,
			'num_keys' => $num_keys,
			'num_acts' => $num_acts
		);

		if ($email)
			$post_data['email'] = $email;

		if ($feature_names)
		{
			$post_data['feature_name'] = $feature_names;
			$post_data['feature_value'] = $feature_values;
		}

		return self::PostRequest($post_data);
	}

	public static function GetPKeyDetails($pkey_id)
	{
		$post_data = array(
			'method' => 'limelm.pkey.getDetails',
			'pkey_id' => $pkey_id
		);

		return self::PostRequest($post_data);
	}

	public static function GetPKeyID($version_id, $pkey)
	{
		$post_data = array(
			'method' => 'limelm.pkey.getID',
			'version_id' => $version_id,
			'pkey' => $pkey
		);

		return self::PostRequest($post_data);
	}

	public static function ManualActivation($xml_act_request)
	{
		$post_data = array(
			'method' => 'limelm.pkey.manualActivation',
			'act_req_xml' => $xml_act_request
		);

		return self::PostRequest($post_data);
	}

	public static function SetPKeyDetails($pkey_id, $num_acts = null, $email = null, $feature_names = null, $feature_values = null)
	{
		$post_data = array(
			'method' => 'limelm.pkey.setDetails',
			'pkey_id' => $pkey_id
		);

		if ($num_acts !== null)
			$post_data['num_acts'] = $num_acts;

		if ($email !== null)
			$post_data['email'] = $email;

		if ($feature_names !== null)
		{
			$post_data['feature_name'] = $feature_names;
			$post_data['feature_value'] = $feature_values;
		}

		return self::PostRequest($post_data);
	}

	public static function GenerateTrialExtension($version_id, $is_online, $length, $expires, $customer_id = null, $max_uses = null)
	{
		$post_data = array(
			'method' => 'limelm.trialExtension.generate',
			'version_id' => $version_id,
			'is_online' => $is_online ? 'true' : 'false',
			'length' => $length,
			'expires' => $expires
		);

		if ($is_online)
			$post_data['max_uses'] = $max_uses;

		if ($customer_id !== null)
			$post_data['customer_id'] = $customer_id;

		return self::PostRequest($post_data);
	}

	public static function TestEcho($params)
	{
		$params['method'] = 'limelm.test.echo';
		return self::PostRequest($params);
	}

	private static function PostRequest($post_data)
	{
		if (!self::$api_key)
			throw new Exception('You must specify your LimeLM API key and set it using SetAPIKey().');

		$post_data['api_key'] = self::$api_key;

		// This section takes the input fields and converts them to the proper format
		// for an http post.  For example: "method=limelm.pkey.find&version_id=100"
		$post_string = '';
		foreach ($post_data as $key => $value)
		{
			if (is_array($value))
			{
				foreach ($value as $sub_value)
				{
					$post_string .= $key.'[]='.urlencode($sub_value).'&';
				}
			}
			else
				$post_string .= $key.'='.urlencode($value).'&';
		}

		$post_string = rtrim($post_string, '& ');

		curl_setopt(self::$request, CURLOPT_HEADER, 0); // eliminate header info from response
		curl_setopt(self::$request, CURLOPT_ENCODING, ""); // support gzip & deflate responses if available
		curl_setopt(self::$request, CURLOPT_RETURNTRANSFER, 1); // Returns response data instead of TRUE(1)
		curl_setopt(self::$request, CURLOPT_POSTFIELDS, $post_string); // use HTTP POST to send form data
		curl_setopt(self::$request, CURLOPT_SSL_VERIFYPEER, FALSE); // uncomment this line if you get no gateway response.

		return curl_exec(self::$request); // execute curl post and store results in $post_response
	}
}
?>