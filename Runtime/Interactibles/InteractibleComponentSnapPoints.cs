using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    [RequireComponent(typeof(Interactible))]
    public class InteractibleComponentSnapPoints : InteractibleComponentBase {
        public List<SnapPoint> snapPoints;

        public override void Init() {
            snapPoints = new List<SnapPoint>();
            var interactible = GetComponent<Interactible>();
            AddSnappingPointsRecursive(transform, interactible);
        }

        void AddSnappingPointsRecursive(Transform trans, Interactible interactible) {
            var snapPoint = trans.GetComponent<SnapPoint>();
            if (snapPoint) {
                snapPoint.networkComponent = this;
                var id = new SnapPointId {
                    interactibleId = interactible.id.interactibleId,
                    creatorId = interactible.id.creatorId,
                    index = (byte)snapPoints.Count,
                };
                snapPoint.snapPointId = id;
                snapPoints.Add(snapPoint);
            }

            for (int i = 0; i < trans.childCount; i++) {
                var child = trans.GetChild(i);
                if (child.GetComponent<Interactible>())
                    return;
                AddSnappingPointsRecursive(child, interactible);
            }
        }
    }
}
