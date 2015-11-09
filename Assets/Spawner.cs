using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    class Spawner : MonoBehaviour
    {
        public static int MaxX = 180, MaxZ = 120;
        public const int villX = 30, villY = 30;
        public static int Humans = 100, PlacesPerVillage = 3, villages = 7;

        void Start()
        {
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < villages; ++i)
            {
                Vector3 villPos = new Vector3(Random.Range(0, MaxX), 1, Random.Range(0, MaxZ));
                int max = 120;
                while (positions.Any(p => Vector3.Distance(villPos, p) < --max))
                    villPos = new Vector3(Random.Range(0, MaxX), 1, Random.Range(0, MaxZ));
                positions.Add(villPos);
                for (int j = 0; j < PlacesPerVillage; ++j)
                {
                    max = 40;
                    Vector3 pos = new Vector3(villPos.x + Random.Range(0, villX), 1, villPos.z + Random.Range(0, villY));
                    while (FindObjectsOfType<Place>().Any(p => Vector3.Distance(pos, p.gameObject.transform.position) < --max))
                        pos = new Vector3(villPos.x + Random.Range(0, villX), 1, villPos.z + Random.Range(0, villY));
                    PlaceFactory.CreatePlace((Place.Type)((j+i)%5), pos);
                }
            }
            for (int i = 0; i < Humans; ++i)
            {
                HumanFactory.CreateHuman(new Vector3(Random.Range(0, MaxX), 1, Random.Range(0, MaxZ)));
            }
        }

        void Update()
        {

        }

    }
}
