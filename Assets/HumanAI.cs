﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Action = AI.Action;
using Random = UnityEngine.Random;
using Behaviour = AI.Behaviour;
using AI;
using System.Collections;

namespace Assets
{
    public class HumanAI : MonoBehaviour
    {
        private const int maxHumans = 500;

        public int BehaviourUpdateTime = 8 + Random.Range(0, 8); // updates behaviour every x frames
        public int BreedingTime = ChildTime + Random.Range(0, 50);
        public Behaviour XCS;
        public float learnRate = 0.08F, sight = 10.0F;
        public Action currentAction = Action.GoHome;
        public const int ChildTime = 50;
        public int age = 0;
        public enum Gender { Male, Female };
        public Gender Sex;
        private Place currentLocation = null;
        private List<Action> lastActions = new Action[] { Action.GoHome, Action.GoHome, Action.GoHome }.ToList();
        private const float BaseSpeed = 0.8F;
        private int updateCount = 0;

        private const int stateSize = 4;//sizeof(byte)*4;
        //5 bit each happiness, boredom, money + current location

        private List<Action> possibleActions = new Action[] { Action.GoHome, Action.GoToHospital, Action.GoToWork, Action.GoShopping, Action.GoToBar }.ToList();


        public int Happiness = Random.Range(0, 255);

        public int Boredom = Random.Range(0, 255);

        public int Money = Random.Range(0, 255);
        private float lastBenefit;


        void Start()
        {
            XCS = new Behaviour(possibleActions, stateSize);
            Sex = Random.value > 0.5 ? Gender.Female : Gender.Male;
            gameObject.transform.position = new Vector3(Random.value * 100, Random.value * 100, 0);
            gameObject.name = "Human";
            Update();
        }


        void Update()
        {
            UpdateVisuals();

            if (updateCount <= 0)
            {
                if (Money > 255)
                    Money = 255;
                if (Happiness > 255)
                    Happiness = 255;
                if (Boredom > 255)
                    Boredom = 255;
                if (Money < 0)
                    Money = 0;
                if (Happiness < 0)
                    Happiness = 0;
                if (Boredom < 0)
                    Boredom = 0;
                XCS.UpdateRules(CalcBenefit(), learnRate);
                currentAction = XCS.BestAction(CalcState());
                lastActions.Add(currentAction);
                lastActions.RemoveAt(0);
                updateCount = BehaviourUpdateTime;

            }
            if (Time.frameCount % BreedingTime == 0 && Random.value > FindObjectsOfType<HumanAI>().Count() / (float)maxHumans)
                TryBreed();
            ++age;
            DoAction(currentAction);
            if (age > 5000 * CalcFitness())
                GameObject.Destroy(gameObject);
        }

        private BitArray CalcState()
        {
            byte b = 0;
            for (int i = 0; i < 1; ++i)
            {
                b += (byte)((int)lastActions[i] << (i * 3));
            }
            BitArray ba = new BitArray(new byte[] { b });
            ba.Length = stateSize;
            ba[3] = Money > 10;
            return ba;
            //todo: locType
        }

        protected void MoveTowards(GameObject o)
        {
            if (o != null)
            {
                gameObject.transform.position = Vector3.MoveTowards(
                    gameObject.transform.position,
                    o.gameObject.transform.position,
                    BaseSpeed);
                if  (Time.frameCount % 10 == 0)
                    Boredom++;
            }
        }

        private void UpdateVisuals()
        {
            GetComponent<Renderer>().enabled = currentLocation == null && age > ChildTime;
            gameObject.GetComponent<Renderer>().material.color =
                new Color(Happiness / 256F,
                Boredom / 256F,
                Money / 256F);


        }

        public float CalcBenefit()
        {
            float newBenefit = Happiness / 256F * (1F - Boredom / 256F);
            float benefit = Logistic((newBenefit - lastBenefit + 1) / 2);
            lastBenefit = newBenefit;
            return benefit;

        }

        public float CalcFitness()
        {
            return Happiness / 256F * (1F - Boredom / 256F);
        }

        private float Logistic(float x)
        {
            return 1F / (1F + (float)Math.Pow(Math.E, -(x - 0.5) * 12));
        }

        protected void DoAction(AI.Action action)
        {
            switch (action)
            {
                case Action.GoHome:
                    VisitPlace(Place.Type.Home);
                    break;
                case Action.GoShopping:
                    VisitPlace(Place.Type.Shop);
                    break;
                case Action.GoToBar:
                    VisitPlace(Place.Type.Bar);
                    break;
                case Action.GoToHospital:
                    VisitPlace(Place.Type.Hospital);
                    break;
                case Action.GoToWork:
                    VisitPlace(Place.Type.Office);
                    break;
            }

        }

        private void VisitPlace(Place.Type type)
        {
            if (currentLocation == null || currentLocation.Category != type)
            {
                if (currentLocation != null)
                {
                    currentLocation.Leave(this);
                    currentLocation = null;
                }

                float bestDist = float.MaxValue;
                Place destination = null;

                // find nearest place
                foreach (Place p in GameObject.FindObjectsOfType<Place>().Where(
                    (p) => { return p.Category == type; })
                    )
                {
                    float dist = Vector3.Distance(p.gameObject.transform.position, this.gameObject.transform.position);
                    if (dist < bestDist)
                    {
                        destination = p;
                        bestDist = dist;
                    }
                }

                // decide if already there TODO: Collision checking
                if (bestDist < 2)
                {
                    destination.Visit(this);
                    currentLocation = destination;
                }
                else
                {
                    MoveTowards(destination.gameObject);
                }
            }
            else
                --updateCount;
        }

        private void TryBreed()
        {
            
            List<HumanAI> possiblePartners = GameObject.FindObjectsOfType<HumanAI>().Where(h => h.age >= 200 && h != this).ToList();
            if (possiblePartners.Count == 0)
                return;
            HumanAI partner = possiblePartners[Random.Range(0, possiblePartners.Count - 1)];
            GameObject child = HumanFactory.CreateHuman(gameObject.transform.position);
            child.GetComponent<HumanAI>().BeBorn(this, partner);
        }

        public void BeBorn(HumanAI father, HumanAI mother)
        {
            //gameObject.transform.position = Vector3.Slerp(father.gameObject.transform.position, 
            //    mother.gameObject.transform.position, 
            //    0.5F);
            XCS = new Behaviour(possibleActions, father.XCS, mother.XCS, stateSize);
        }

    }


}
