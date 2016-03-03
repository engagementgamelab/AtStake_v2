public class NetworkMessageContent {

	public readonly string id, str1, str2;
	public readonly int val;

	public NetworkMessageContent (string id, string str1="", string str2="", int val=-1) {
		this.id = id;
		this.str1 = str1;
		this.str2 = str2;
		this.val = val;
	}
}