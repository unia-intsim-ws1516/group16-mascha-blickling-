using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class Place : MonoBehaviour
    {
        public Type Category;
        public static int Ticks = 20;
        private HashSet<HumanAI> visitors = new HashSet<HumanAI>();
        private static GUIStyle textStyle = new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 10};

        public enum Type
        {
            Hospital, Bar, Home, Office, Shop, None
        }

        public int MoneyIncrease = 0,
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
                        human.Happiness = (byte)(human.Happiness + HappinessIncrease);
                        human.Boredom = (byte)(human.Boredom + BoredomIncrease);
                        human.Money = (byte)(human.Money + MoneyIncrease);
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
