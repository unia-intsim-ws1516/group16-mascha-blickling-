using UnityEngine;
using System.Collections;

public class ZombieAI : MonoBehaviour {
    private float x, y;
    private int life = 100;


	// Use this for initialization
	void Start () {
        x = GetComponent<Transform>().position.x;
        y = GetComponent<Transform>().position.y;
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
