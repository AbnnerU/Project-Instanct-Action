using System;
using System.Reflection;
using Scripts.Combat;
using Scripts.Enchantment;
using Scripts.Status;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor.Tests
{
    public static class PercentageConventionChecks
    {
        private static int _checks;

        [MenuItem("Tools/Project Instanct Action/Percentages/Run Calculation Checks")]
        public static void Run()
        {
            _checks = 0;
            UnityEngine.Random.State randomState = UnityEngine.Random.state;
            try
            {
                StatusInfo attacker = Statuses(10f, 20f, 0f, 125f);
                DamageData scaled = Damage(85f, 200f, 10f);
                Expect(107f, DamageResolver.Resolve(scaled, attacker).FinalDamage, "Percentage scaling");
                Expect(85.025f, DamageResolver.Resolve(Damage(85f, 0.25f, 0f), attacker).FinalDamage, "Fractional percentage is not a multiplier");

                attacker.GetStatusData(StatusType.CRITICAL_CHANCE).CurrentValue = 100f;
                Expect(106.25f, DamageResolver.Resolve(Damage(85f), attacker).FinalDamage, "125% total critical damage");

                StatusData critical = attacker.GetStatusData(StatusType.CRITICAL_DAMAGE);
                StatusEnchantmentModifier flat = new StatusEnchantmentModifier
                {
                    StatusType = StatusType.CRITICAL_DAMAGE, ValueType = EnchantmentValueType.FLAT, Value = 12f
                };
                critical.AddCurrentValue(ModifierDelta(flat, critical.CurrentValue));
                Expect(137f, critical.CurrentValue, "Critical upgrade adds percentage points");
                Expect(116.45f, DamageResolver.Resolve(Damage(85f), attacker).FinalDamage, "Critical regression: 85 with +12 upgrade");
                flat.Value = 8f;
                critical.AddCurrentValue(ModifierDelta(flat, critical.CurrentValue));
                Expect(145f, critical.CurrentValue, "Multiple critical upgrades");
                Expect(123.25f, DamageResolver.Resolve(Damage(85f), attacker).FinalDamage, "Stacked critical upgrade damage");

                flat.ValueType = EnchantmentValueType.PERCENT_OF_CURRENT;
                flat.Value = 10f;
                Expect(12.5f, ModifierDelta(flat, 125f), "10% of current is different from +10 points");
                flat.StatusType = StatusType.ATTACK_SPEED;
                flat.ValueType = EnchantmentValueType.FLAT;
                flat.Value = 4f;
                Expect(104f, 100f + ModifierDelta(flat, 100f), "Attack speed flat upgrade");

                attacker = Statuses(10f, 20f, 100f, 125f);
                StatusInfo defender = new StatusInfo();
                defender.List_StatusData.Add(new StatusData { StatusType = StatusType.PHYSICAL_RESISTANCE, CurrentValue = 30f });
                Expect(93.625f, DamageResolver.Resolve(scaled, attacker, defender).FinalDamage, "Resistance after critical");
                Expect(133.75f, DamageResolver.Resolve(Damage(85f, 200f, 10f, DamageType.MAGIC), attacker, defender).FinalDamage, "Missing magic resistance");
                defender.List_StatusData[0].CurrentValue = 200f;
                Expect(0f, DamageResolver.Resolve(scaled, attacker, defender).FinalDamage, "Resistance capped at 100%");
                Expect(133.75f, DamageResolver.Resolve(Damage(85f, 200f, 10f, DamageType.TRUE), attacker, defender).FinalDamage, "True damage bypasses resistance");

                attacker.GetStatusData(StatusType.CRITICAL_DAMAGE).CurrentValue = 250f;
                Expect(212.5f, DamageResolver.Resolve(Damage(85f), attacker).FinalDamage, "Critical above 200%");
                attacker.GetStatusData(StatusType.CRITICAL_DAMAGE).CurrentValue = 0f;
                Expect(85f, DamageResolver.Resolve(Damage(85f), attacker).FinalDamage, "Zero critical does not silently become 200%");
                attacker.List_StatusData.Remove(attacker.GetStatusData(StatusType.CRITICAL_DAMAGE));
                Expect(170f, DamageResolver.Resolve(Damage(85f), attacker).FinalDamage, "Missing critical stat fallback");
                attacker.GetStatusData(StatusType.CRITICAL_CHANCE).CurrentValue = 0f;
                Expect(85f, DamageResolver.Resolve(Damage(85f), attacker).FinalDamage, "Zero critical chance");

                string description = EnchantmentDescriptionFormatter.FormatDamage(scaled);
                if (!description.Contains("200%") || !description.Contains("10%") || description.Contains("20000%"))
                    throw new InvalidOperationException("Incorrect percentage description: " + description);
                _checks++;
                Debug.Log($"Percentage calculation checks PASSED: {_checks} assertions.");
            }
            finally
            {
                UnityEngine.Random.state = randomState;
            }
        }

        private static float ModifierDelta(StatusEnchantmentModifier modifier, float current)
        {
            MethodInfo method = typeof(StatusEnchantmentModifier).GetMethod("GetValueToAdd", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
                throw new InvalidOperationException("StatusEnchantmentModifier.GetValueToAdd was not found.");
            return (float)method.Invoke(modifier, new object[] { current });
        }

        private static DamageData Damage(float value, float physical = 0f, float magic = 0f, DamageType type = DamageType.PHYSICAL)
        {
            DamageData data = new DamageData();
            SetField(data, "_baseDamage", value);
            SetField(data, "_physicForceScaling", physical);
            SetField(data, "_magicForceScaling", magic);
            SetField(data, "_damageType", type);
            return data;
        }

        private static void SetField(object target, string name, object value)
        {
            FieldInfo field = target.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                throw new InvalidOperationException("Missing test field " + name);
            field.SetValue(target, value);
        }

        private static StatusInfo Statuses(float physical, float magic, float chance, float critical)
        {
            StatusInfo result = new StatusInfo();
            result.List_StatusData.Add(new StatusData { StatusType = StatusType.PHYSIC_FORCE, CurrentValue = physical });
            result.List_StatusData.Add(new StatusData { StatusType = StatusType.MAGIC_FORCE, CurrentValue = magic });
            result.List_StatusData.Add(new StatusData { StatusType = StatusType.CRITICAL_CHANCE, CurrentValue = chance });
            result.List_StatusData.Add(new StatusData { StatusType = StatusType.CRITICAL_DAMAGE, CurrentValue = critical });
            return result;
        }

        private static void Expect(float expected, float actual, string label)
        {
            if (Mathf.Abs(expected - actual) > 0.001f)
                throw new InvalidOperationException($"{label}: expected {expected}, got {actual}.");
            _checks++;
        }
    }
}
