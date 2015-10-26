using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class Spawner : MonoBehaviour
    {
        void Start()
        {
            for (int i = 0; i < 80; ++i)
            {
                GameObject human = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //Instantiate(human);
                human.AddComponent<HumanAI>();
                //human.AddComponent<MeshCollider>();
                //human.AddComponent<NavMeshAgent>();
            }

            for (int i = 0; i < 10; ++i)
            {
                GameObject food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Instantiate(food);
                food.AddComponent<Food>();
            }
        }

        void Update()
        {
            if (Time.frameCount % 60 == 0)
            {
                GameObject food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //Instantiate(food);
                food.AddComponent<Food>();
            }
        }

    }
}
