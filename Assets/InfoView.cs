﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Action = AI.Action;

namespace Assets
{
    class InfoView : MonoBehaviour
    {
        public static bool VirusView = true;

        private Dictionary<Action, float[]> histograms = new Dictionary<Action, float[]>();

        private int bufferSize = 120;
        private static Action[] actions = new Action[] { Action.GoToWork, Action.GoToHospital, Action.GoToBar, Action.GoShopping, Action.GoHome };

        void Start()
        {
            foreach (Action a in actions)
            {
                histograms[a] = new float[bufferSize];
            }
            
        }

        private void DrawHistogram(float[] hist, Vector4 position)
        {
            
        }

        void Update()
        {
            float money = 0F, boredom = 0F, happy = 0F, benefit = 0F;
            int count = 0;
            Dictionary<Action, float> actions = new Dictionary<Action, float>();
            actions[Action.GoToWork] = 0;
            actions[Action.GoToHospital] = 0;
            actions[Action.GoHome] = 0;
            actions[Action.GoShopping] = 0;
            actions[Action.GoToBar] = 0;
            foreach (HumanAI human in GameObject.FindObjectsOfType<HumanAI>())
            {
                if (human.age > 200)
                {
                    ++count;
                    money += human.Money;
                    happy += human.Happiness;
                    boredom += human.Boredom;
                    benefit += human.CalcFitness();
                    ++actions[human.currentAction];
                }
            }
            GetComponent<Text>().text = string.Format(
                "Avg. Happy: {0:0.##}\nAvg. Boredom: {1:0.##}\nAvg. Money: {2:0.##}\nAvg. Fitness: {8:0.##}\n" +
                "At work: {3:0.##}\nAt home: {4:0.##}\nAt shop: {5:0.##}\n" +
                "At bar: {6:0.##}\nAt hospital: {7:0.##}\n"+
                "Total humans: {9}\nFPS: {10}",
                happy / count, boredom / count, money / count,
                actions[Action.GoToWork] / count, actions[Action.GoHome] / count, actions[Action.GoShopping] / count,
                actions[Action.GoToBar] / count, actions[Action.GoToHospital] / count,
                benefit / count, FindObjectsOfType<HumanAI>().Count(),
                1/Time.deltaTime);
        }
    }
}
