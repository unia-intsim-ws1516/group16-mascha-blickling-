using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    

    public class HumanFactory
    {
        public static GameObject CreateHuman(Vector3 location)
        {
            GameObject human = GameObject.CreatePrimitive(PrimitiveType.Cube);
            human.gameObject.transform.position = location;
            human.gameObject.name = "Human";
            human.AddComponent<HumanAI>();
            return human;
        }
    }

    public class PlaceFactory
    {
        public static GameObject CreatePlace(Place.Type type, Vector3 location)
        {
            GameObject place = GameObject.CreatePrimitive(PrimitiveType.Cube);
            place.gameObject.transform.position = location;
            place.gameObject.transform.localScale = new Vector3(3, 2, 2);
            place.AddComponent<Place>();
            switch (type)
            {
                case Place.Type.Bar:
                    CreateBar(place);
                    break;
                case Place.Type.Home:
                    CreateHome(place);
                    break;
                case Place.Type.Hospital:
                    CreateHospital(place);
                    break;
                case Place.Type.Office:
                    CreateOffice(place);
                    break;
                case Place.Type.Shop:
                    CreateShop(place);
                    break;
            }
            place.GetComponent<Place>().Category = type;
            return place;
        }

        private static void CreateBar(GameObject place)
        {
            place.gameObject.name = "Bar";
            place.GetComponent<Place>().MoneyIncrease = -1;
            place.GetComponent<Place>().BoredomIncrease = 0;
            place.GetComponent<Place>().HappinessIncrease = 3;
        }

        private static void CreateHospital(GameObject place)
        {
            place.gameObject.name = "Hospital";
            place.GetComponent<Place>().MoneyIncrease = -8;
            place.GetComponent<Place>().BoredomIncrease = 2;
            place.GetComponent<Place>().HappinessIncrease = -2;
            place.GetComponent<Place>().IsIsolated = true;
            place.GetComponent<Place>().CuresAid = true;
        }

        private static void CreateHome(GameObject place)
        {
            place.gameObject.name = "House";
            place.GetComponent<Place>().MoneyIncrease = 0;
            place.GetComponent<Place>().BoredomIncrease = -5;
            place.GetComponent<Place>().HappinessIncrease = 0;
            place.GetComponent<Place>().IsIsolated = true;
        }

        private static void CreateOffice(GameObject place)
        {
            place.gameObject.name = "Office";
            place.GetComponent<Place>().MoneyIncrease = 3;
            place.GetComponent<Place>().BoredomIncrease = 0;
            place.GetComponent<Place>().HappinessIncrease = -1;
        }

        private static void CreateShop(GameObject place)
        {
            place.gameObject.name = "Mall";
            place.GetComponent<Place>().MoneyIncrease = -3;
            place.GetComponent<Place>().BoredomIncrease = -1;
            place.GetComponent<Place>().HappinessIncrease = 2;
            place.GetComponent<Place>().IsIsolated = true;
        }
    }
}
