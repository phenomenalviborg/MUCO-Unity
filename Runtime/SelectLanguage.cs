using UnityEngine;

namespace Muco
{
    public class SelectLanguage : MonoBehaviour
    {
        public Language language;

        public void SetSelectedLanguage()
        {
            Player.ThePlayer.language = language;

            foreach (var room in RoomManager.TheRoomManager.rooms) {
                if (room) {
                    room.UpdateLanguage(language);
                }
            }
        }
    }
}