# Damage reporting and projectile impacts

## Damage reporting

- Add one `DamageDealtReporter` to the attacker GameObject.
- `DamageRequest.AttackerDamageResultReporter` is optional. `WithHit` preserves it.
- `DamageReceiver` reports the result once after applying damage. The legacy `AttackHitbox` path that hits `IHittable` directly reports its own result.
- Normal attacks cache the reporter in `Awake`; abilities cache it in `Setup` from the attacker. Projectile shooters cache it in `Awake`.
- Projectiles carry the request; they do not search for the owner or calculate healing.
- `HealOnDamageDealt._damageDealerReference` points to the reporter. If empty, it uses the local `DamageDealtReporter`, never an arbitrary `IDamageDealer` component.
- Existing healing status and damage-type filters are unchanged. Healing remains `FinalDamage * status / 100`; overkill is not capped to the remaining target health.
- `AttackHitbox.OnDamageDealt` remains a local hitbox event. `OnDamageDealtWithExecution` remains the trigger for attack-related passives. Do not subscribe healing to both the hitbox and the reporter.
- Bonus damage reports through its own request and no longer calls `AttackHitbox.NotifyDamageDealt`.
- Reporters ignore failed results and calls after their component is disabled/destroyed. Healing also rejects failed results and nonpositive damage.

## Projectile impacts

- `Projectile3DBase` sorts the returned cast hits by distance.
- Positive-distance hits preserve the actual contact point, including valid contacts at world origin.
- Zero-distance hits use `Collider.ClosestPoint(castOrigin)` as an approximate point for initial overlap. When the origin is inside the collider, this can be the origin itself.
- Projectile position uses the swept sphere center (`castOrigin + direction * distance`), not the contact point.
- Position is applied before the hit callback and pool release for stopping projectiles. No transform write happens after release.
- `DamageRequestProjectile` registers the target before damage callbacks to prevent reentrant duplicate damage.

## Focused manual checks

1. Injure the player, set VAMP to 10, and hit with a magic fireball. A final damage of 100 should heal 10, once.
2. Repeat with the bonus-damage passive, piercing thrust, and normal attacks, respecting each healing component's damage-type filters.
3. Hit a dead target or omit the reporter: no healing should occur; missing reporter must not prevent damage.
4. Fire normally and while touching/overlapping an enemy away from world origin. Both effects should appear near the target.
5. Reuse the projectile from the pool and verify its impact and healing again.

## Changed files for review

- `Assets/Script/Interface/Combat/IDamageResultReporter.cs`: optional reporting contract.
- `Assets/Script/Combat/Damage Effects/DamageDealtReporter.cs`: attacker-owned event source.
- `Assets/Script/Combat/Data/DamageRequest.cs`: reporter reference, constructor parameter, and preservation in `WithHit`.
- `Assets/Script/Combat/DamageReceiver.cs`: reports applied results.
- `Assets/Script/Combat/Damage Effects/HealOnDamageDealt.cs`: explicit event source and failed-result guard.
- `Assets/Script/Combat/AttackHitbox.cs`: cached reporter, requests, legacy direct-hit reporting; removes `NotifyDamageDealt`.
- `Assets/Script/Abilities/Active/LauchProjectileAbility.cs`: caches reporter in setup and passes it to the projectile request.
- `Assets/Script/Abilities/Active/PiercingThrustAbility.cs`: caches reporter in setup and passes it to each hit request.
- `Assets/Script/Abilities/Passive/AttackBonusDamageAbility.cs`: caches reporter in setup and removes forwarding through the hitbox.
- `Assets/Script/Abilities/Auto Setup/AttackBonusDamageAutoSetup.cs`: passes the owner to setup.
- `Assets/Script/Projectile/DamageRequestProjectileShooter.cs`: caches the attacker's reporter and includes it in requests.
- `Assets/Script/Abstract/Projectile/Projectile3DBase.cs`: contact correction, distance ordering, and positioning before release.
- `Assets/Script/Projectile/DamageRequestProjectile.cs`: registers hit targets before invoking damage callbacks.
- `Assets/Prefab/Player.prefab`: one reporter and explicit references on both healing components. The Combat Test Player Variant inherits this configuration.

## Verification performed

- Unity compilation: no errors detected.
- Isolated preview-scene checks: receiver reports once, `WithHit` preserves reporter, failed result is ignored, damage works without a reporter.
- Readback: both healing components reference their local reporter in both player prefabs.
- Gameplay healing and impact visuals still require a Play Mode check. No gameplay session was started.
