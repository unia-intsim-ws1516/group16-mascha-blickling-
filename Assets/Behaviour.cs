using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace AI
{
    public enum Action
    {
        Wait, Roam,
        Attack, Flee,
        GoToFood, GatherFood, GiveFood, EatFood,
        GoToWeapon, GatherWeapon, GiveWeapon,
        GoToHuman, GoToZombie,
        Breed
    };

    public enum State
    {
        FoodInSight, ZombieInSight,
        WeaponInSight, HumanInSight,
        ChildInSight,
        Hungry, Injured,
        HasWeapon, HasFood,
        FertileHumanInSight,   // human is other gender and not pregnant if woman
        Pregnant, Child
    };

    // a ruleset as well as attributes which can be evolved through inheritance
    // this system is realized through an XCS, evbolution is handled by the simulation itself
    public class Behaviour
    {
        private const int DELAY = 5;

        

        //public enum Attribute
        //{
        //    Hungry, Injured, Sight, Speed, Strength
        //}

        private class Rule : IComparable
        {
            public UberBool[] RuleState = new UberBool[Enum.GetNames(typeof (Action)).Length];
            public Action RuleAction;
            public float Fitness = 0.5F;
            public float Prediction = 0.5F;
            public float PredError = 0.5F;
            public UberBool this[State s]
            {
                get { return RuleState[(int)s]; }
            }

            public static bool operator <(Rule a, Rule b)
            {
                return a.Fitness < b.Fitness;
            }

            public static bool operator >(Rule a, Rule b)
            {
                return a.Fitness > b.Fitness;
            }

            public Rule(List<Action> possibleActions)
            {
                // random category
                for (int i = 0; i < RuleState.Count(); ++i)
                {
                    float rand = Random.value;
                    if (rand < 0.2F)
                        RuleState[i] = UberBool.False;
                    else if (rand > 0.8F)
                        RuleState[i] = UberBool.True;
                    else
                        RuleState[i] = UberBool.Any;
                }
                // random action
                RuleAction = possibleActions[(int)(Random.value * possibleActions.Count)];
            }

            public int CompareTo(object obj)
            {
                if ((obj as Rule).Fitness > Fitness)
                    return 1;
                if ((obj as Rule).Fitness < Fitness)
                    return -1;
                return 0;
            }
        }

        private List<Rule> RuleSet = new List<Rule>();
        private List<Rule> LastVoters = new List<Rule>();
        public int NumRules;

        public Behaviour(List<Action> possibleActions, int numRules = 40)
        {
            NumRules = numRules;
            for (int i = 0; i < NumRules; ++i){
                RuleSet.Add(new Rule(possibleActions));
            }
        }

        public Behaviour (List<Action> possibleActions, Behaviour mother, Behaviour father)
        {
            // learn rule size
            NumRules = (mother.NumRules + father.NumRules) / 2 + (int)(Random.value * 5) - 3;
            if (NumRules < 10)
                NumRules = 10;
            RuleSet.AddRange(mother.RuleSet);
            RuleSet.AddRange(father.RuleSet);
            RuleSet.Add(new Rule(possibleActions)); // Innovation
            RuleSet.Sort();
            RuleSet.RemoveRange(0, RuleSet.Count - NumRules);
        }

        public Action BestAction(Dictionary<State, bool> state)
        {
            List<Rule> fitting = new List<Rule>();
            foreach (Rule r in RuleSet)
            {
                if (IsActive(r, state))
                {
                    fitting.Add(r);
                }
            }
            if (fitting.Count == 0)
                return Action.Wait;
            Action best = fitting.Aggregate((l, r) => l.Prediction > r.Prediction ? l : r).RuleAction;
            LastVoters = fitting.Where((e)=>e.RuleAction == best).ToList();
            // return highest answer
            return best;
        }

        public void UpdateRules(float benefit, float learnRate)
        {
            // get all who voted
            
            foreach (Rule r in LastVoters)
            {
                r.PredError += learnRate * (Math.Abs(benefit - r.Prediction) - r.PredError);
                r.Prediction += learnRate * (benefit - r.Prediction);
            }
            // count mean accuracy in voters
            float relAcc = LastVoters.Sum((rule) => 1.0F / rule.PredError) / LastVoters.Count;
            
            // update fitness
            foreach (Rule r in LastVoters)
            {
                r.Fitness += learnRate * (relAcc - r.Fitness);
            }
        }

        private bool IsActive(Rule rule, Dictionary<State, bool> state)
        {
            foreach (KeyValuePair<State, bool> entry in state)
            {
                if (! MatchBool(rule[entry.Key], entry.Value))
                    return false;
            }
            return true;
        }

        private static bool MatchBool(UberBool ub, bool b)
        {
            return (ub == UberBool.Any) || (ub == UberBool.True && b) || (ub == UberBool.False && !b);
        }

        private enum UberBool
        {
            True, False, Any
        }
    }

    

    
}
