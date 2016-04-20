using UnityEngine;
using UnityEngine.Networking;

public class MasterMsgTypes
{
	public enum NetworkMasterServerEvent
	{
		RegistrationFailedGameName, // Registration failed because an empty game name was given.
		RegistrationFailedGameType, // Registration failed because an empty game type was given.
		RegistrationFailedNoServer, // Registration failed because no server is running.
		RegistrationSucceeded, // Registration to master server succeeded, received confirmation.
		UnregistrationSucceeded, // Unregistration to master server succeeded, received confirmation.
		HostListReceived, // Received a host list from the master server.
		RegisteredClientFailed, // Client could not be added to room.
	}

	// -------------- client to masterserver Ids --------------

	public const short RegisterHostId = 150;
	public const short UnregisterHostId = 151;
	public const short RequestListOfHostsId = 152;
	public const short RegisterClientId = 153;
	public const short GenericClientToHostId = 158;
	public const short GenericHostToClientsId = 159;

	// -------------- masterserver to client Ids --------------

	public const short RegisteredHostId = 160;
	public const short UnregisteredHostId = 161;
	public const short ListOfHostsId = 162;
	public const short RegisteredClientId = 163;
	public const short UnregisteredClientId = 164;
	public const short GenericHostFromClientId = 166;
	public const short GenericClientsFromHostId = 167;

	// -------------- message for passing common values --------------

	public class GenericMessage : MessageBase
	{
		public string id;
		public string str1;
		public string str2;
		public int val;
		public byte[] bytes;

		public static GenericMessage Create (string id, string str1="", string str2="", int val=-1, byte[] bytes=null) {
			GenericMessage msg = new GenericMessage ();
			msg.id = id;
			msg.str1 = str1;
			msg.str2 = str2;
			msg.val = val;
			msg.bytes = bytes;
			return msg;
		}

		public CompressedGenericMessage ToCompressed () {
			CompressedGenericMessage msg = new CompressedGenericMessage ();
			msg.id = CLZF2.Compress (id);
			msg.str1 = CLZF2.Compress (str1);
			msg.str2 = CLZF2.Compress (str2);
			msg.val = val;
			msg.bytes = bytes;
			return msg;
		}
	}

	public class CompressedGenericMessage : MessageBase
	{
		public byte[] id;
		public byte[] str1;
		public byte[] str2;
		public int val;
		public byte[] bytes;

		public GenericMessage ToDecompressed () {
			GenericMessage msg = new GenericMessage ();
			msg.id = CLZF2.DecompressToString (id);
			msg.str1 = CLZF2.DecompressToString (str1);
			msg.str2 = CLZF2.DecompressToString (str2);
			msg.val = val;
			msg.bytes = bytes;
			return msg;
		}
	}

	// -------------- client to server messages --------------

	public class RegisterHostMessage : MessageBase
	{
		public string gameTypeName;
		public string gameName;
		public string comment;
		public bool passwordProtected;
		public int playerLimit;
		public int hostPort;
	}

	public class UnregisterHostMessage : MessageBase
	{
		public string gameTypeName;
		public string gameName;
	}

	public class RequestHostListMessage : MessageBase
	{
		public string gameTypeName;
	}

	public class RegisterClientMessage : MessageBase
	{
		public string gameTypeName;
		public string gameName;
		public string clientName;
	}
	
	// -------------- server to client messages --------------

	public class Room
	{
		public string name;
		public string comment;
		public bool passwordProtected;
		public int playerLimit;
		public string hostIp;
		public int hostPort;
		public int connectionId;
		public Player[] players;
	}

	public class Player
	{
		public string name;
		public int connectionId;
	}

	public class ListOfHostsMessage : MessageBase
	{
		public int resultCode;
		public Room[] hosts;
	}

	public class RegisteredHostMessage : MessageBase
	{
		public int resultCode;
	}

	public class UnregisteredHostMessage : MessageBase
	{
		public int resultCode;
	}

	public class RegisteredClientMessage : MessageBase
	{
		public int resultCode;
		public string clientName;
	}

	public class UnregisteredClientMessage : MessageBase
	{
		public string clientName;
	}
}
