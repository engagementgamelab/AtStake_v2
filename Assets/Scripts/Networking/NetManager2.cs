using UnityEngine;
using System.Collections;
using SocketIO;

public class NetManager2 {

	SocketIOComponent socket;

	public NetManager2 (SocketIOComponent socket) {
		this.socket = socket;
	}
}
