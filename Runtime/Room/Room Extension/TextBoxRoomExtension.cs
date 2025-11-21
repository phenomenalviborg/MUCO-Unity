using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Muco {
    public class TextBoxRoomExtension : RoomExtension {
        public override void UpdateLanguage(Language language) {
            var textBoxes = FindObjectsByType<MultiLangTextBox>(FindObjectsSortMode.None);
            foreach (MultiLangTextBox box in textBoxes) {
                box.SelectLanguage(language);
            }
        }
    }
}
