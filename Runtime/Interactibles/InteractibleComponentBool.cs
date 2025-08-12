using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Muco {
    public class InteractibleComponentBool : InteractibleComponentBase
    {
        [SerializeField]
        private bool _value;

        public bool Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }

        public UnityEvent OnToggleLocal;
        public UnityEvent OnTrueLocal;
        public UnityEvent OnFalseLocal;
        public override void Ser(List<byte> buffer) {
            Serialize.SerBool(Value,buffer);
        }

        public override void Des(ref int cursor, byte[] buffer) {
            bool newValue;
            Serialize.DesBool(out newValue, ref cursor, buffer);
            Value = newValue;
        }
        public void UpdateAndSend(bool takeOwnership)
        {
            interactible.SendState(takeOwnership);
            OnToggleLocal.Invoke();
            if (Value) {
                OnTrueLocal.Invoke();
            } else {
                OnFalseLocal.Invoke();
            }
        }
    }
}