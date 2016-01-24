using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TilePoolVisuals : MonoBehaviour {

    public GameFlow.PlacableTiles WatchedTile = GameFlow.PlacableTiles.Street;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Text>().text = FindObjectOfType<GameFlow>().Placable[(int)WatchedTile].ToString();
	}
}
