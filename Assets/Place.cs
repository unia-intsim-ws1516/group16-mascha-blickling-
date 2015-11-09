using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets
{
    public class Place : MonoBehaviour
    {
        public Type Category;
        public static int Ticks = 1;
        private HashSet<HumanAI> visitors = new HashSet<HumanAI>();
        private static GUIStyle textStyle = new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 10 };

        public enum Type
        {
            Hospital, Bar, Home, Office, Shop, None
        }

        public float MoneyIncrease = 0,
            HappinessIncrease = 0,
            BoredomIncrease = 0;

        public bool IsIsolated = false;
        public bool CuresAid = false;

        void Update()
        {
            if (Time.frameCount % Ticks == 0)
            {
                foreach (HumanAI human in visitors)
                {
                    if (MoneyIncrease + human.Money >= 0)
                    {
                        human.Happiness += HappinessIncrease;
                        human.Boredom += BoredomIncrease;
                        human.Money += MoneyIncrease;
                        if (CuresAid)
                        {
                            Disease d = human.GetComponent<Disease>();
                            if (d != null && Random.value < d.HealChance)
                                GameObject.Destroy(d);
                        }
                    }
                    else
                    {
                        human.Boredom += 1;
                    }
                }
                if (!IsIsolated)
                {
                    foreach (HumanAI infected in visitors.Where(v => v.GetComponent<Disease>() != null))
                    {
                        Disease d = infected.GetComponent<Disease>();
                        foreach (HumanAI human in visitors)
                        {
                            if (human.GetComponent<Disease>() == null && Random.value < d.SpreadChance)
                                human.gameObject.AddComponent<Disease>();
                        }
                    }
                }
            }
        }

        void Start()
        {

        }

        void OnGUI()
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 15, 100, 50), gameObject.name, textStyle);
        }

        public void Visit(HumanAI visitor)
        {
            visitors.Add(visitor);
        }

        public void Leave(HumanAI visitor)
        {
            visitors.Remove(visitor);
        }

    }


}
