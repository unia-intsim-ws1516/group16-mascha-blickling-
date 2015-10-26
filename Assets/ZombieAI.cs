//using UnityEngine;
//using System.Collections;
//using Assets;

//public class ZombieAI : BaseAI {
    

//	// Use this for initialization
//	void Start () {

//    }
	
   

//	// Update is called once per frame
//	void Update () {
//        gameObject.transform.position = Vector3.MoveTowards(Position(), GetTarget(), Speed);
//	}

//    // returns nearest human position
//    private Vector2 GetTarget()
//    {
//        Vector2 result = Position();
//        float best_dist = float.MaxValue;
//        foreach (HumanAI ai in FindObjectsOfType<HumanAI>())
//        {
//            float dist = Vector2.Distance(Position(), ai.Position());
//            if (dist < best_dist)
//            {
//                best_dist = dist;
//                result = ai.Position();
//            }
//        }
//        return result;
//    }
//}
