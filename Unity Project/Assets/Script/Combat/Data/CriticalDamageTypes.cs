using System;

namespace Scripts.Combat
{
    [Flags]
    public enum CriticalDamageTypes
    {
        NONE = 0,
        PHYSICAL = 1 << 0,
        MAGIC = 1 << 1,
        TRUE = 1 << 2
    }
}
