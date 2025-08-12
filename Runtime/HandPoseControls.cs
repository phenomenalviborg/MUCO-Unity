using System.Collections;
using UnityEngine;
namespace Muco
{
    public class HandGestureControls : MonoBehaviour
    {
        public HandPoseDetection handPoseDetectionLeft;
        public HandPoseDetection handPoseDetectionRight;
        bool loadingDeveloperMode;
        public float loadingTime = 3f;

        void Update()
        {
            if (!loadingDeveloperMode)
            {
                if (handPoseDetectionLeft.handPoseType == HandPoseDetection.HandPoseType.Loser && handPoseDetectionRight.handPoseType == HandPoseDetection.HandPoseType.Peace)
                {
                    loadingDeveloperMode = true;
                    StartCoroutine("WaitAndTriggerDeveloperMode", loadingTime);
                }
                else
                {
                    loadingDeveloperMode = false;
                     StopCoroutine("WaitAndTriggerDeveloperMode");
                }
            }
        }

        IEnumerator WaitAndTriggerDeveloperMode(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            DeveloperMode.TheDeveloperMode.isOn = !DeveloperMode.TheDeveloperMode.isOn;
            loadingDeveloperMode = false;
        }
    }
}

