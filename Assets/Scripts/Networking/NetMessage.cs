using UnityEngine;
using System.Collections;

public class NetMessage {

	public string id;
	public string str1;
	public string str2;
	public int val;
	public byte[] bytes;
	public JSONObject json;

	public static NetMessage Create (string id, JSONObject json) {
		NetMessage msg = new NetMessage ();
		msg.id = id;
		msg.json = json;
		return msg;
	}

	public static NetMessage Create (string id, string str1="", string str2="", int val=-1, byte[] bytes=null) {
		NetMessage msg = new NetMessage ();
		msg.id = id;
		msg.str1 = str1;
		msg.str2 = str2;
		msg.val = val;
		msg.bytes = bytes;
		return msg;
	}
}
