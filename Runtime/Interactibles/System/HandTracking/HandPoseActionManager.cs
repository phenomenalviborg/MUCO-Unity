using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Muco {
    [System.Serializable]
    public enum Hand {
        Left,
        Right,
    }

    [System.Serializable]
    public struct HandPoseCriterion {
        public Hand hand;
        public HandPoseDetection.HandPoseType handPoseType;
    }

    [System.Serializable]
    public struct HandPoseCondition {
        public List<HandPoseCriterion> criteria;
        public float duration = 2;
    }


    [System.Serializable]
    public class HandPoseAction {
        public HandPoseCondition condition;
        public UnityEvent unityEvent;
    }

    public class HandPoseActionManager : MonoBehaviour {
        public HandPoseDetection[] hands;
        public List<HandPoseAction> actions;
        private float[] timers;

        void Awake() {
            timers = new float[actions.Count];
            for (int i = 0; i < timers.Length; i++) {
                timers[i] = 0;
            }
        }

        bool IsMet(HandPoseCondition action) {
            foreach (var criterion in action.criteria) {
                var hand = hands[(int)criterion.hand];
                if (hand.handPoseType != criterion.handPoseType)
                    return false;
            }
            return true;
        }

        void Update() {
            string debugString = "";
            foreach (var timer in timers) {
                debugString += timer + " ";

            }
            VrDebug.SetValue("HandPose", "timers", debugString);

            for (int actionIndex = 0; actionIndex < actions.Count; actionIndex++) {
                var action = actions[actionIndex];
                bool hasFired = timers[actionIndex] >= action.condition.duration;
                if (!IsMet(action.condition))
                    timers[actionIndex] = 0;
                timers[actionIndex] += Time.deltaTime;
                bool shouldHaveFired = timers[actionIndex] >= action.condition.duration;
                if (shouldHaveFired && !hasFired)
                    action.unityEvent.Invoke();
            }
        }
    }
}
