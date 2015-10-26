using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using State = AI.State;
using Action = AI.Action;
using Behaviour = AI.Behaviour;

namespace Assets
{
    

    public abstract class BaseAI : BaseObject
    {
        public int BehaviourUpdateTime = 20; // updates behaviour every x frames
        public Behaviour XCS;
        protected float learnRate = 0.01F, 
            hunger = 0.0F, injury = 0.0F, sight = 10.0F;
        public float hungerThreshhold, injuryThreshhold;
        public Weapon equipedWeapon = null;
        public Food equipedFood = null;
        protected Action currentAction = Action.Roam;

        public Vector2 Position()
        {
            // z discarded, see api
            return GetComponent<Transform>().position;
        }


        public virtual void Update()
        {
            // check if dead
            if (injury >= 1 || hunger >= 1)
                Die();
            // update behaviour
            if (Time.frameCount % BehaviourUpdateTime == 0)
            {
                XCS.UpdateRules(CalcBenefit(), learnRate);
                currentAction = XCS.BestAction(CalcState());
            }
            // update food and injury
            hunger += CalcFoodConsumption();
            injury -= CalcRegeneration();
            if (injury < 0)
                injury = 0;
            DoAction(currentAction);
        }

        protected abstract float CalcBenefit();

        protected abstract float CalcFoodConsumption();

        protected abstract float CalcRegeneration();

        protected abstract float CalcSpeed();

        private Dictionary<State, bool> CalcState()
        {
            
            Dictionary<State, bool> result = new Dictionary<State, bool>();
            List<BaseObject> inSight = InSight();
            result[State.FoodInSight] = inSight.OfType<Food>().Count() > 0;
            result[State.HasWeapon] = equipedWeapon != null;
            result[State.HasFood] = equipedFood != null;
            result[State.HumanInSight] = inSight.OfType<HumanAI>().Count() > 0;
            result[State.Hungry] = hunger > hungerThreshhold;
            result[State.Injured] = injury > injuryThreshhold;
            if (this is HumanAI) {
                result[State.FertileHumanInSight] =
                    inSight.OfType<HumanAI>().Where(
                        (other) => (this as HumanAI).Sex != other.Sex && other.IsFertile()
                        ).Count() > 0;
                result[State.Child] = (this as HumanAI).IsChild();
                result[State.Pregnant] = (this as HumanAI).IsPregnant();
                result[State.ChildInSight] = inSight.OfType<HumanAI>().Where((other) => other.IsChild()).Count() > 0;
            }
            result[State.WeaponInSight] = inSight.OfType<Weapon>().Count() > 0;
            //result[State.ZombieInSight] = inSight.OfType<ZombieAI>().Count() > 0;
            return result;
        }

        protected List<BaseObject> InSight()
        {
            return GameObject.FindObjectsOfType<BaseObject>().Where(
                    (o)=>Vector3.Distance(o.transform.position, gameObject.transform.position) < sight
                ).ToList();
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        protected void MoveTowards(BaseObject o)
        {
            if (o != null)
            {
                gameObject.transform.position = Vector3.MoveTowards(
                    gameObject.transform.position,
                    o.gameObject.transform.position,
                    CalcSpeed());
            }
        }

        protected abstract void DoAction(Action action);
        
    }
}
