using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace AI
{
    public enum Action
    {
        GoToWork, GoToBar, GoToHospital, GoShopping,
        GoHome,
        
        InteractWithHuman,
        Breed
    };

    public enum State
    {
        IsHappy, IsBored, HasMoney,
        NearHuman, FeelsSick
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
            public UberBool[] RuleState;
            public Action RuleAction;
            public float Fitness = 0F;
            public float Prediction = Random.value;
            public float PredError = 0.5F;
            public UberBool this[int i]
            {
                get { return RuleState[i]; }
            }

            public static bool operator <(Rule a, Rule b)
            {
                return a.Prediction < b.Prediction;
            }

            public static bool operator >(Rule a, Rule b)
            {
                return a.Prediction > b.Prediction;
            }

            public Rule(List<Action> possibleActions, int stateSize)
            {
                RuleState = new UberBool[stateSize];
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
                if ((obj as Rule).Prediction > Prediction)
                    return 1;
                if ((obj as Rule).Prediction < Prediction)
                    return -1;
                return 0;
            }
        }

        private List<Rule> RuleSet = new List<Rule>();
        private List<List<Rule>> LastVoters = new List<List<Rule>>();
        public int NumRules;
        private float[] paybackDistr = new float[] { 0.8F, 0.2F};
        private int maxDelay = 2;
        private List<Action> possibleActions;

        public Behaviour(List<Action> possibleActions, int stateSize, int numRules = 40)
        {
            NumRules = numRules;
            for (int i = 0; i < NumRules; ++i){
                RuleSet.Add(new Rule(possibleActions, stateSize));
            }
            this.possibleActions = possibleActions;
        }

        public Behaviour (List<Action> possibleActions, Behaviour mother, Behaviour father, int stateSize)
        {
            // learn rule size
            NumRules = (mother.NumRules + father.NumRules) / 2 + (int)(Random.value * 5) - 3;
            if (NumRules < 10)
                NumRules = 10;
            RuleSet.AddRange(mother.RuleSet);
            RuleSet.AddRange(father.RuleSet);
            RuleSet.Add(new Rule(possibleActions, stateSize)); // Innovation
            RuleSet.Sort();
            RuleSet.RemoveRange(NumRules, RuleSet.Count - NumRules);
            this.possibleActions = possibleActions;
        }

        private class Counter
        {
            private int count = 0;
            private float value = 0F;
            public void Add(float val)
            {
                ++count;
                value += val;
            }

            public float Value()
            {
                return value / count;
            }

        }


        public Action BestAction(BitArray state)
        {
            List<Rule> fitting = new List<Rule>();
            foreach (Rule r in RuleSet)
            {
                if (IsActive(r, state))
                {
                    fitting.Add(r);
                }
            }
            Dictionary<Action, Counter> pred = new Dictionary<Action, Counter>();
            foreach (Action a in possibleActions)
                pred.Add(a, new Counter());
            foreach (Rule r in fitting)
            {
                pred[r.RuleAction].Add(r.Prediction);
            }
            float randomPred = Random.value * pred.Sum(val=> val.Value.Value());
            
            Action best = Action.GoHome;
            float maxValue = 0F;
            foreach(KeyValuePair<Action, Counter> k in pred)
            {
                if (k.Value.Value() > maxValue)
                {
                    maxValue = k.Value.Value();
                    best = k.Key;
                }
                //best = k.Key;
                //randomPred -= k.Value.Value();
                //if ( randomPred<= 0)
                //    break;
            }
            LastVoters.Add (fitting.Where((e)=>e.RuleAction == best).ToList());
            if (LastVoters.Count > maxDelay)
                LastVoters.RemoveAt(0);
            // return highest answer
            return best;
        }

        public void UpdateRules(float benefit, float learnRate)
        {
            // push new benefit
            
            for (int i = 0; i < LastVoters.Count; ++i)
            {

                foreach (Rule r in LastVoters[i])
                {
                    r.PredError += paybackDistr[i] * learnRate * (Math.Abs(benefit - r.Prediction) - r.PredError);
                    r.Prediction += paybackDistr[i] * learnRate * (benefit - r.Prediction);
                }
                // count mean accuracy in voters
                float relAcc = LastVoters[i].Sum((rule) => 1.0F / rule.PredError) / LastVoters.Count;

                // update fitness
                foreach (Rule r in LastVoters[i])
                {
                    r.Fitness += paybackDistr[i] * learnRate * (relAcc - r.Fitness);
                }
            }
        }

        private bool IsActive(Rule rule, BitArray state)
        {
            for(int i=0; i < state.Count; ++i)
            {
                if (! MatchBool(rule[i], state[i]))
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
