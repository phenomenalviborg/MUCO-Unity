using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Muco {
    public class InteractibleComponentSlider : InteractibleComponentBase {
        public UnityEvent<float> onSlide;
        public UnityEvent<float> onSlideLocal;
        public float valueMin;
        public float valueMax;
        public Transform handle;
        public float handleMin;
        public float handleMax;
        public SetColor setColor;
        public bool snapToIncrement = false;
        public bool softsnap = true;
        public float snappingIncrement = 1.0f;
        public bool sendPreciseSnapValue = true;

        //state
        public float value;
        public bool isGrabbed;

        public override void Ser(List<byte> buffer) {
            Serialize.SerFloat(value, buffer);
            Serialize.SerBool(isGrabbed, buffer);
        }

        public override void Des(ref int cursor, byte[] buffer) {
            float newValue;
            Serialize.DesFloat(out newValue, ref cursor, buffer);
            bool newIsGrabbed;
            Serialize.DesBool(out newIsGrabbed, ref cursor, buffer);
            UpdateLocally(newValue, newIsGrabbed);
        }

        public void UpdateAndSend(float newValue, bool newIsGrabbed, bool takeOwnership) {
            UpdateLocally(newValue, newIsGrabbed);
            interactible.SendState(takeOwnership);
        }

        void UpdateLocally(float newValue, bool newIsGrabbed) {
            var oldValue = value;
            var wasGrabbed = isGrabbed;
            value = newValue;
            isGrabbed = newIsGrabbed;

            var lastSnapped = Snap(oldValue);
            var snapped = Snap(value);

            if (snapped != lastSnapped) {
                var audioSource = GetComponent<AudioSource>();
                if (audioSource) {
                    audioSource.PlayOneShot(audioSource.clip, 1.0f);
                }
            }

            float softSnapped = softsnap ? Mathf.Lerp(snapped, value, 0.2f) : snapped;
            
            if (snapToIncrement) {
                value = snapped;
            }

            SetVisuals(softsnap ? softSnapped : snapped);

            onSlide.Invoke(value);
            onSlideLocal.Invoke(value);


            if (isGrabbed != wasGrabbed) {
                if (isGrabbed)
                    setColor.Set(Color.red);
                else
                    setColor.Set(Color.white);
            }
        }

        public void SetVisuals(float x) {
            var w = (x - valueMin) / (valueMax - valueMin);
            var p = new Vector3(Mathf.Lerp(handleMin, handleMax, w), 0, 0);
            handle.localPosition = p;
        }

        public float Snap(float x) {
            return Mathf.Round(x / snappingIncrement) * snappingIncrement;
        }
    }
}
