using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Action = AI.Action;
using Random = UnityEngine.Random;
using Behaviour = AI.Behaviour;

namespace Assets
{
    public class HumanAI : BaseAI
    {
        public int PregnancyTime = 0;
        public int ChildTime = 0;
        public enum Gender { Male, Female};
        public Gender Sex;
        private float lastBenefit = 0;
        private const float baseConsumption = 0.001F;
        private const float baseRegeneration = 0.01F;
        private const float actionRange = 1F;
        private const int pregnancyDuration = 30 * 30;
        private const int childhoodDuration = 30 * 60;
        public float BaseSpeed = 0.1F;
        public Action action;

        public int MaxGroupSize = 5;

        List<Action> possibleActions = new Action[] {
            Action.Breed, Action.EatFood, Action.GatherFood, Action.GoToFood, Action.GoToHuman, Action.Roam, Action.Wait }.ToList();

        public bool IsPregnant()
        {
            return PregnancyTime > 0;
        }

        public bool IsChild()
        {
            return ChildTime > 0;
        }

        public bool IsHungry()
        {
            return hunger > hungerThreshhold;
        }

        public bool IsInjured()
        {
            return injury > injuryThreshhold;
        }

        public bool IsFertile()
        {
            return !IsChild() && !IsPregnant();
        }

        void Start()
        {
            XCS = new Behaviour(possibleActions);
            Sex = Random.value > 0.5 ? Gender.Female : Gender.Male;
            gameObject.transform.position = new Vector3(Random.value * 100, Random.value * 100, 0);
            injuryThreshhold = Random.value;
            hungerThreshhold = Random.value;
            gameObject.name = "Human";
        }


        public override void Update()
        {
            --PregnancyTime;
            --ChildTime;
            base.Update();
            UpdateVisuals();
            action = currentAction;
        }

        private void UpdateVisuals()
        {
            if (equipedFood != null)
                equipedFood.gameObject.transform.position = gameObject.transform.position + new Vector3(0, 2, 0);
            gameObject.GetComponent<Renderer>().material.color = 
                new Color (hunger, 
                IsPregnant()? 1.0F: 0.0F,
                IsChild() ? 1.0F: 0.0F);
            gameObject.name = IsChild() ? (Sex == Gender.Female ? "Girl" : "Boy")
                : (Sex == Gender.Female ? "Woman" : "Man");
        }

        protected override float CalcBenefit()
        {
            float humansInRange = InSight().OfType<HumanAI>().Count();
            if (humansInRange > MaxGroupSize)
                humansInRange = MaxGroupSize;
            float benefit = (humansInRange / MaxGroupSize + (1 - injury) + (1 - hunger)) / 3; // max 1
            float totalBenefit = (benefit - lastBenefit) / 2 + 0.5F;
            lastBenefit = benefit;
            return totalBenefit;
        }

        protected override float CalcFoodConsumption()
        {
            float consumption = baseConsumption;
            if (IsChild())
                consumption /= 2;
            if (IsPregnant())
                consumption *= 2;
            if (IsHungry())
                consumption /= 2;
            if (IsInjured())
                consumption *= 2;
            return consumption;
        }

        protected override float CalcRegeneration()
        {
            float regeneration = baseRegeneration;
            if (IsHungry())
                regeneration /= 2;
            if (IsInjured())
                regeneration *= 2;
            return regeneration;
        }

        protected override void DoAction(AI.Action action)
        {
            BaseObject target = null;
            switch (action)
            {
                case Action.Breed:
                    target = InSight().OfType<HumanAI>().Where(
                        (other) => (this as HumanAI).Sex != other.Sex && other.IsFertile()
                        ).FirstOrDefault();
                    if (target != null)
                    {
                        MoveTowards(target);
                        TryBreed(target as HumanAI);
                    }
                    break;
                case Action.EatFood:
                    if (IsHungry() && equipedFood != null)
                        EatFood();
                    break;
                case Action.GatherFood:
                    target = InSight().OfType<Food>().FirstOrDefault();
                    if (target != null)
                    {
                        MoveTowards(target);
                        PickUp(target);
                    }
                    break;
                case Action.GoToHuman:
                    target = InSight().OfType<HumanAI>().FirstOrDefault();
                    if (target != null)
                        MoveTowards(target);
                    break;
                case Action.Roam:
                    gameObject.transform.position = Vector3.MoveTowards(
                        gameObject.transform.position,
                        new Vector3(Random.value * 100, Random.value * 100, 0),
                        CalcSpeed());
                    break;
            }
        }

        private void EatFood()
        {
            equipedFood.Size -= hunger;
            if (equipedFood.Size <= 0)
            {
                Destroy(equipedFood);
                equipedFood = null;
            }
        }

        private void PickUp(BaseObject target)
        {
            if (Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= actionRange)
            {
                if (target is Food)
                    equipedFood = target as Food;
            }
        }

        private void TryBreed(HumanAI target)
        {
            if (target.Sex != Sex &&
                Vector3.Distance(target.gameObject.transform.position, gameObject.transform.position) <= actionRange &&
                IsFertile() &&
                target.IsFertile())
            {
                if (target.Sex == Gender.Female)
                    target.GetPregnant();
                else
                    GetPregnant();
                GameObject human = GameObject.CreatePrimitive(PrimitiveType.Cube);
                human.AddComponent<HumanAI>();
                human.GetComponent<HumanAI>().BeBorn(this, target);
            }

        }

        void GetPregnant()
        {
            PregnancyTime = pregnancyDuration;
        }

        protected override float CalcSpeed()
        {
            float speed = BaseSpeed;
            if (IsInjured())
                speed /= 2;
            if (IsHungry())
                speed /= 2;
            if (IsPregnant())
                speed /= 2;
            return speed;
        }

        // create new

        public HumanAI()
        {
            
        }

        public void BeBorn (HumanAI father, HumanAI mother)
        {
            gameObject.transform.position = Vector3.Slerp(father.gameObject.transform.position, 
                mother.gameObject.transform.position, 
                0.5F);
            XCS = new Behaviour(possibleActions, father.XCS, mother.XCS);
            injuryThreshhold = (mother.injuryThreshhold + father.injuryThreshhold) / 2;
            hungerThreshhold = (mother.hungerThreshhold + father.hungerThreshhold) / 2;
            ChildTime = childhoodDuration;
        }

    }
}
