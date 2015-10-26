using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    public class Food : BaseObject
    {
        public float Size = 2.0f;

        void Start()
        {
            gameObject.transform.position = new Vector3(Random.value * 100, Random.value * 100, 0);
            gameObject.name = "Food";
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
}
