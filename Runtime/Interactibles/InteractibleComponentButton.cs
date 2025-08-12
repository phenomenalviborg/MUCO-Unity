using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Muco {
    public class InteractibleComponentButton : InteractibleComponentBase {
        public Transform cap;
        public UnityEvent onPress;
        public UnityEvent onPressLocalPlayer;
        public float fireValue = 0.003f;
        public float resetValue;
        public float maxPressed = 0.006f;

        public SetColor setColor;

        public AudioClip pressedSound;
        public AudioClip unpressedSoud;

        public Color colorUnpressed = new Color(0, 0, 0);
        public Color colorPressed = new Color(1, 0, 0);
        public bool useMaterialColorAsDefaultUnpressed = true;

        public bool wasPressed;

        public float value;
        public bool isGrabbed;

        public virtual void Awake() {
            if (useMaterialColorAsDefaultUnpressed) {
                colorUnpressed = setColor.gameObject.GetComponent<Renderer>().material.color;
            }
            else {
                setColor.Set(colorUnpressed);
            }
        }

        public override void Ser(List<byte> buffer) {
            Serialize.SerFloat(value, buffer);
            Serialize.SerBool(isGrabbed, buffer);
        }

        public bool PressedLocally() {
            return value != 0 && ownedLocally;
        }

        public override void Deinit() {
            if (PressedLocally())
            UpdateValueAndSend(0, false, false, new Vector2());
        }

        public override void Des(ref int cursor, byte[] buffer) {
            float newValue;
            Serialize.DesFloat(out newValue, ref cursor, buffer);
            bool newIsGrabbed;
            Serialize.DesBool(out newIsGrabbed, ref cursor, buffer);
            UpdateValue(newValue, newIsGrabbed, false, new Vector2());
        }

        public void UpdateValueAndSend(float y, bool isGrabbed, bool takeOwnership, Vector2 xz) {
            UpdateValue(y, isGrabbed, true, xz);
            interactible.SendState(takeOwnership);
        }

        public virtual void UpdateValue(float y, bool isGrabbed, bool localSource, Vector2 xz) {
            value = y;
            this.isGrabbed = isGrabbed;

            if (y > maxPressed)
                y = maxPressed;

            bool is_pressed = wasPressed;
            if (y >= fireValue)
                is_pressed = true;
            else if (y <= resetValue)
                is_pressed = false;

            if (is_pressed) {
                if (!wasPressed) {
                    onPress.Invoke();
                    if (localSource)
                        onPressLocalPlayer.Invoke();
                    PlaySound(pressedSound);
                    setColor.Set(colorPressed);
                }
            }
            else if (wasPressed) {
                PlaySound(unpressedSoud);
                setColor.Set(colorUnpressed);
            }

            MoveCap(y, xz);

            wasPressed = is_pressed;
        }

        public virtual void PlaySound(AudioClip clip) {
            if (clip) {
                var audioSource = GetComponent<AudioSource>();
                if (audioSource) {
                    audioSource.PlayOneShot(clip, 1.0f);
                }
            }
        }

        public virtual void MoveCap(float y, Vector2 xz) {
            cap.transform.localPosition = new Vector3(0, -y, 0);
        }
    }
}
