using System;

namespace Assets.Scripts.Common
{
    [Serializable]
    public struct Stats
    {
        public int health;
        public int speed;
        public int initiative;
        public int minDamageMelee;
        public int maxDamageMelee;
        public int minDamageRange;
        public int maxDamageRange;
        public float turnRepeatChance; // * 100%
    }
}
