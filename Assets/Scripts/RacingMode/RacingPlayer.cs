using UnityEngine;

namespace RacingMode
{
    [CreateAssetMenu(fileName = "New Racing Player", menuName = "Racing Mode/Create Racing Player", order = 0)]
    public class RacingPlayer : ScriptableObject
    {
        public string playerName;
        public Sprite playerSprite;
    }
}