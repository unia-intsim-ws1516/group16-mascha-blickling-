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
        public static int MaxX = 100, MaxY = 100, MaxZ = 0;
        public static int Humans = 100, Places = 12;

        void Start()
        {
            PlaceFactory.CreatePlace(Place.Type.Bar, new Vector3(Random.Range(0, MaxX), Random.Range(0, MaxY), Random.Range(0, MaxZ)));
            PlaceFactory.CreatePlace(Place.Type.Home, new Vector3(Random.Range(0, MaxX), Random.Range(0, MaxY), Random.Range(0, MaxZ)));
            PlaceFactory.CreatePlace(Place.Type.Shop, new Vector3(Random.Range(0, MaxX), Random.Range(0, MaxY), Random.Range(0, MaxZ)));
            PlaceFactory.CreatePlace(Place.Type.Office, new Vector3(Random.Range(0, MaxX), Random.Range(0, MaxY), Random.Range(0, MaxZ)));
            PlaceFactory.CreatePlace(Place.Type.Hospital, new Vector3(Random.Range(0, MaxX), Random.Range(0, MaxY), Random.Range(0, MaxZ)));
            for (int i = 0; i < Places; ++i)
            {
                PlaceFactory.CreatePlace((Place.Type)Random.Range(0, 4), new Vector3 (Random.Range(0, MaxX), Random.Range(0, MaxY), Random.Range(0, MaxZ)));
            }

            for (int i = 0; i < Humans; ++i)
            {
                HumanFactory.CreateHuman(new Vector3(Random.Range(0, MaxX), Random.Range(0, MaxY), Random.Range(0, MaxZ)));
            }
        }

        void Update()
        {

        }

    }
}
