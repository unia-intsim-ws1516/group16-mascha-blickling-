using System;
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
        public int BehaviourUpdateTime = 20 + Random.Range(0, 8); // updates behaviour every x frames
        public Behaviour XCS;
        public float learnRate = 0.01F, sight = 10.0F;
        public Action currentAction = Action.Wait;
        public int PregnancyTime = 0;
        public int ChildTime = 0;
        public enum Gender { Male, Female };
        public Gender Sex;
        private Place currentLocation = null;

        private bool isFinished = true;
        private int updateCount = 0;

        private const int stateSize = 15;//sizeof(byte)*4;
        //5 bit each happiness, boredom, money + current location

        private List<Action> possibleActions = new Action[] { Action.GoHome, Action.GoShopping, Action.GoToBar, Action.GoToHospital, Action.GoToWork }.ToList();


        private byte _Happiness = 0;
        public byte Happiness {
            get { return _Happiness; }
            set
            {
                if (value + _Happiness > 255)
                    _Happiness = 255;
                else if (value + _Happiness < 0)
                    _Happiness = 0;
                else
                    _Happiness += value;
            }   
        }

        private byte _Boredom = 0;
        public byte Boredom
        {
            get { return _Boredom; }
            set
            {
                if (value + _Boredom > 255)
                    _Boredom = 255;
                else if (value + _Boredom < 0)
                    _Boredom = 0;
                _Boredom += value;
            }
        }

        private byte _Money;
        public byte Money
        {
            get { return _Money; }
            set
            {
                if (value + _Money > 255)
                    _Money = 255;
                else if (value + Money < 0)
                    _Money = 0;
                _Money += value;
            }
        }

        private const int pregnancyDuration = 30 * 30;
        private const int childhoodDuration = 30 * 60;
        public float BaseSpeed = 0.3F;


        public bool IsPregnant()
        {
            return PregnancyTime > 0;
        }

        public bool IsChild()
        {
            return ChildTime > 0;
        }

        public bool IsFertile()
        {
            return !IsChild() && !IsPregnant();
        }

        void Start()
        {
            XCS = new Behaviour(possibleActions, stateSize);
            Sex = Random.value > 0.5 ? Gender.Female : Gender.Male;
            gameObject.transform.position = new Vector3(Random.value * 100, Random.value * 100, 0);
            gameObject.name = "Human";
        }


        void Update()
        {
            --PregnancyTime;
            --ChildTime;
            UpdateVisuals();

            if (updateCount <= 0)
            {
                XCS.UpdateRules(CalcBenefit(), learnRate);
                currentAction = XCS.BestAction(CalcState());
                updateCount = BehaviourUpdateTime;
                isFinished = false;
            }
            DoAction(currentAction);

        }

        private BitArray CalcState()
        {
            List<bool> result = new List<bool>();
            Place.Type locType = Place.Type.None;
            if (currentLocation != null)
                locType = currentLocation.Category;
            BitArray ba = new BitArray(11);
            BitArray b = new BitArray(new byte[] { (byte)((Money / 64) << 6 + (Money / 16) << 2 + (Boredom / 64)) });
            for (int i = 0; i < 8; ++i)
                ba[i] = b[i];
            //todo: locType
            return ba;
        }

        protected void MoveTowards(GameObject o)
        {
            if (o != null)
            {
                gameObject.transform.position = Vector3.MoveTowards(
                    gameObject.transform.position,
                    o.gameObject.transform.position,
                    BaseSpeed);
            }
        }

        private void UpdateVisuals()
        {
            GetComponent<Renderer>().enabled = currentLocation == null;
            gameObject.GetComponent<Renderer>().material.color = 
                new Color (Happiness * 8, 
                IsPregnant()? 1.0F: 0.0F,
                IsChild() ? 1.0F: 0.0F);
            gameObject.name = IsChild() ? (Sex == Gender.Female ? "Girl" : "Boy")
                : (Sex == Gender.Female ? "Woman" : "Man");
            
        }

        protected float CalcBenefit()
        {
            float benefit = Happiness / 64.0F - Boredom / 64.0F + 0.5F;
            return benefit;
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

        private void TryBreed(HumanAI target)
        {
            //if (target.Sex != Sex &&
            //    Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= actionRange &&
            //    IsFertile() &&
            //    target.IsFertile())
            //{
            //    if (target.Sex == Gender.Female)
            //        target.GetPregnant();
            //    else
            //        GetPregnant();
            //    GameObject human = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    human.AddComponent<HumanAI>();
            //    human.GetComponent<HumanAI>().BeBorn(this, target);
            //}
            throw new NotImplementedException();
        }

        void GetPregnant()
        {
            PregnancyTime = pregnancyDuration;
        }

        public void BeBorn (HumanAI father, HumanAI mother)
        {
            //gameObject.transform.position = Vector3.Slerp(father.gameObject.transform.position, 
            //    mother.gameObject.transform.position, 
            //    0.5F);
            //XCS = new Behaviour(possibleActions, father.XCS, mother.XCS);
            //ChildTime = childhoodDuration;
            throw new NotImplementedException();
        }

    }

    
}
