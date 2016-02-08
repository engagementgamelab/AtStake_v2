using UnityEngine;
using System.Collections;
using Models;

// TODO: get data from api
public static class DataManager {

	public static Settings GetSettings () {
		return new Settings {
			PlayerCountRange = new int[] { 3, 4 }
		};
	}
}
