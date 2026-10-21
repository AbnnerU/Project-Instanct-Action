namespace Scripts.Combat
{
    public interface ICriticalDamagePermissionsProvider
    {
        CriticalDamageTypes GetAllowedCriticalDamageTypes();
        void SetAllowedCriticalDamageTypes(CriticalDamageTypes types);
        void Allow(CriticalDamageTypes types);
    }
}
