using UnityEngine;

namespace Muco {
    public class TextBoxRoomExtension : RoomExtension {
        private Room room;

        public override void Init() {
            room = GetComponent<Room>();
        }

        public override void UpdateLanguage(Language language) {
            var languageTag = language.ToBcp47();
            if (room != null)
                languageTag = room.ResolveLanguage(languageTag);

            var textBoxes = FindObjectsByType<MultiLangTextBox>(FindObjectsSortMode.None);
            foreach (var box in textBoxes)
                box.SelectLanguage(languageTag);
        }
    }
}
