# Backup de ataques antes da lista de hits

Snapshot dos registros YAML originais, incluindo curvas, dados de combo e post-dash. Sem migracao automatica. Data: 2026-11-10.

## Assets\Prefab\Player.prefab
```yaml
--- !u!114 &8183698727932517817
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 1437302767631231260}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: f3e60df30ac7efa4886966e1817b14f2, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Combat.PostDashAttackController
  _active: 1
  _attackHitbox: {fileID: 8664224395122197948}
  _animator: {fileID: 7997707731514666529}
  _characterAnimationController: {fileID: 8709150000114935703}
  _comboAttackSet: {fileID: 11400000, guid: 99245aad33389444e83aaa49757b9a15, type: 2}
  _list_ComboAttackData:
  - _animationStateName: 
    _transitionDuration: 0
    _damageData:
      _damageType: 0
      _baseDamage: 0
      _physicForceScaling: 0
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0
      KnockbackDuration: 0
      KnockbackCurve:
        serializedVersion: 2
        m_Curve: []
        m_PreInfinity: 0
        m_PostInfinity: 0
        m_RotationOrder: 0
    _hitboxCenterOffset: {x: 0, y: 0, z: 0}
    _hitboxHalfExtents: {x: 0, y: 0, z: 0}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 0
    _queryTriggerInteraction: 0
    _maxTargets: 0
  _triggerIdle: 1
  _backToIdleTriggerParameter: Idle
  _comboRetentionDuration: 1.5
  _drawGizmos: 1
  _debugAttackIndex: 0
  _debugHitboxColor: {r: 0, g: 0, b: 0, a: 1}
  _leftAttackInput: {fileID: 7277778564736042800, guid: a6c85231e9084f242931e75e86d0edbc, type: 3}
  _rightAttackInput: {fileID: -3105849121650280034, guid: a6c85231e9084f242931e75e86d0edbc, type: 3}
  _dash: {fileID: 8889931655858333428}
  _weaponManager: {fileID: 0}
  _directionHandler: {fileID: 8402393144809859780}
  _hittableReference: {fileID: 1437302767631231260}
  _maintainComboIndexOnDash: 1

```
## Assets\Scriptable Object\Weapon Enchantment\Hands\Hand_WeaponLoadoutSO.asset
```yaml
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 1fabc2f4b11ea33438dcdf23fa8d7447, type: 3}
  m_Name: Hand_WeaponLoadoutSO
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Weapon.WeaponLoadoutSO
  ID: Handed
  DisplayName: Hands
  ComboAttackSet: {fileID: 11400000, guid: 857cbb28a04eea04bba8de91619771a2, type: 2}
  HasPostDashAttack: 1
  PostDashAttack:
    _animationStateName: Base Layer.Attack.Post Dash Attack
    _transitionDuration: 0.01
    _damageData:
      _damageType: 0
      _baseDamage: 25
      _physicForceScaling: 90
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.1
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 0.55}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.55}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 2
    _maxTargets: 8
  AnimatorController: {fileID: 22100000, guid: 176b08979747f1e48b3ae4650509c18e, type: 2}
  List_WeaponModels: []

```
## Assets\Scriptable Object\Weapon Enchantment\Axe\Axe_WeaponLoadoutSO_Workshop_ef3b1610\Axe_ComboAttackSet.asset
```yaml
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 8752c0cdc5e0b984dbd591abb37d7505, type: 3}
  m_Name: Axe_ComboAttackSet
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Combat.ComboAttackSetSO
  _list_ComboAttackData:
  - _animationStateName: Base Layer.Attack.Attack 1
    _transitionDuration: 0.05
    _damageData:
      _damageType: 0
      _baseDamage: 50
      _physicForceScaling: 110
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0.000000018191287, y: -0.12092799, z: 0.84740114}
    _hitboxHalfExtents: {x: 0.5000003, y: 0.5662227, z: 0.90021384}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 8
  - _animationStateName: Base Layer.Attack.Attack 2
    _transitionDuration: 0.05
    _damageData:
      _damageType: 0
      _baseDamage: 50
      _physicForceScaling: 110
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 1}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.5}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 8
  - _animationStateName: Base Layer.Attack.Attack 3
    _transitionDuration: 0.05
    _damageData:
      _damageType: 0
      _baseDamage: 50
      _physicForceScaling: 110
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 1}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.5}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 8
  - _animationStateName: Base Layer.Attack.Attack 4
    _transitionDuration: 0.05
    _damageData:
      _damageType: 0
      _baseDamage: 100
      _physicForceScaling: 110
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0.000000005130026, y: 0.011736274, z: 0.9569667}
    _hitboxHalfExtents: {x: 0.5000002, y: 0.5078242, z: 0.94989693}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 8

```
## Assets\Scriptable Object\Weapon Enchantment\Two Axes\TwoAxe_WeaponLoadoutSO.asset
```yaml
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 1fabc2f4b11ea33438dcdf23fa8d7447, type: 3}
  m_Name: TwoAxe_WeaponLoadoutSO
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Weapon.WeaponLoadoutSO
  ID: TwoSwords
  DisplayName: TwoSwords
  ComboAttackSet: {fileID: 11400000, guid: b651d1cfa3cf7b1419d6d27c780ed9d1, type: 2}
  HasPostDashAttack: 1
  PostDashAttack:
    _animationStateName: Base Layer.Attack.Post Dash Attack
    _transitionDuration: 0.05
    _damageData:
      _damageType: 0
      _baseDamage: 75
      _physicForceScaling: 200
      _magicForceScaling: 10
    _attackKnockback:
      KnockbackDistance: 1.25
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 2
          outSlope: 2
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0.00000007002618, y: 0.09305692, z: 0.41257823}
    _hitboxHalfExtents: {x: 0.50000006, y: 0.3982191, z: 1.1223179}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 2
    _maxTargets: 8
  AnimatorController: {fileID: 22100000, guid: c1964069ba9db2a459bbb6d4147c3b9c, type: 2}
  List_WeaponModels:
  - WeaponPrefabReference:
      m_AssetGUID: a9de5a98f016a334c9a1113e07b1a12f
      m_SubObjectName: 
      m_SubObjectType: 
      m_SubObjectGUID: 
      m_EditorAssetChanged: 0
    SocketID: Hand_Right_1
    LocalPositionOffset: {x: -0.012, y: 0.015, z: 0.041}
    LocalEulerAngles: {x: -56.695, y: 4.516, z: 76.005}
    LocalScale: {x: 1, y: 1, z: 1}
  - WeaponPrefabReference:
      m_AssetGUID: a9de5a98f016a334c9a1113e07b1a12f
      m_SubObjectName: 
      m_SubObjectType: 
      m_SubObjectGUID: 
      m_EditorAssetChanged: 0
    SocketID: Hand_Left_1
    LocalPositionOffset: {x: -0.012, y: 0.015, z: 0.041}
    LocalEulerAngles: {x: -56.695, y: 4.516, z: -73.827}
    LocalScale: {x: 1, y: 1, z: 1}

```
## Assets\Scriptable Object\Weapon Enchantment\Axe\Axe_WeaponLoadoutSO.asset
```yaml
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 1fabc2f4b11ea33438dcdf23fa8d7447, type: 3}
  m_Name: Axe_WeaponLoadoutSO
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Weapon.WeaponLoadoutSO
  ID: Test
  DisplayName: Machadinho
  ComboAttackSet: {fileID: 11400000, guid: 124620e5373355142a9e27ca9bf8b9f1, type: 2}
  HasPostDashAttack: 1
  PostDashAttack:
    _animationStateName: Base Layer.Attack.Post Dash Attack
    _transitionDuration: 0.05
    _damageData:
      _damageType: 0
      _baseDamage: 75
      _physicForceScaling: 150
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0.000000037896307, y: -0.043349862, z: 0.68210304}
    _hitboxHalfExtents: {x: 0.5000002, y: 0.4277507, z: 0.9768455}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 2
    _maxTargets: 8
  AnimatorController: {fileID: 22100000, guid: aa0e8e1adb5cf2743ba45087c9acd529, type: 2}
  List_WeaponModels:
  - WeaponPrefabReference:
      m_AssetGUID: bda0e0a54afa7464ab2ce114082b55d6
      m_SubObjectName: 
      m_SubObjectType: 
      m_SubObjectGUID: 
      m_EditorAssetChanged: 0
    SocketID: Hand_Right_1
    LocalPositionOffset: {x: -0.18, y: 0.11, z: -0.01}
    LocalEulerAngles: {x: -7.61, y: 160.7, z: -60.4}
    LocalScale: {x: 1, y: 1, z: 1}

```
## Assets\Scriptable Object\Weapon Enchantment\Hands\Hand_ComboAttackSetSO.asset
```yaml
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 8752c0cdc5e0b984dbd591abb37d7505, type: 3}
  m_Name: Hand_ComboAttackSetSO
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Combat.ComboAttackSetSO
  _list_ComboAttackData:
  - _animationStateName: Base Layer.Attack.Attack 1
    _transitionDuration: 0.02
    _damageData:
      _damageType: 0
      _baseDamage: 20
      _physicForceScaling: 100
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 0.55}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.55}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 5
  - _animationStateName: Base Layer.Attack.Attack 2
    _transitionDuration: 0.02
    _damageData:
      _damageType: 0
      _baseDamage: 20
      _physicForceScaling: 100
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 0.78}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.78}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 5
  - _animationStateName: Base Layer.Attack.Attack 3
    _transitionDuration: 0.02
    _damageData:
      _damageType: 0
      _baseDamage: 40
      _physicForceScaling: 150
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 0.75}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.75}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 5

```
## Assets\Scriptable Object\Weapon Enchantment\Two Axes\TwoAxe_ComboAttackSetSO.asset
```yaml
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 8752c0cdc5e0b984dbd591abb37d7505, type: 3}
  m_Name: TwoAxe_ComboAttackSetSO
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Combat.ComboAttackSetSO
  _list_ComboAttackData:
  - _animationStateName: Base Layer.Attack.Attack 1
    _transitionDuration: 0.02
    _damageData:
      _damageType: 0
      _baseDamage: 50
      _physicForceScaling: 120.00001
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 0.93}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.93}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 5
  - _animationStateName: Base Layer.Attack.Attack 2
    _transitionDuration: 0.02
    _damageData:
      _damageType: 1
      _baseDamage: 25
      _physicForceScaling: 120.00001
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 0.93}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.93}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 5
  - _animationStateName: Base Layer.Attack.Attack 3
    _transitionDuration: 0.05
    _damageData:
      _damageType: 0
      _baseDamage: 25
      _physicForceScaling: 120.00001
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.25
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0.00000005650621, y: -0.011632204, z: 0.525992}
    _hitboxHalfExtents: {x: 0.50000024, y: 0.4709198, z: 0.7442745}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 8

```
## Assets\Scriptable Object\Weapon Enchantment\Heavy Hammer Sword\HeavyHammer_WeaponLoadoutSO.asset
```yaml
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 1fabc2f4b11ea33438dcdf23fa8d7447, type: 3}
  m_Name: HeavyHammer_WeaponLoadoutSO
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Weapon.WeaponLoadoutSO
  ID: HeavyHammer
  DisplayName: HeavyHammer
  ComboAttackSet: {fileID: 11400000, guid: 99245aad33389444e83aaa49757b9a15, type: 2}
  HasPostDashAttack: 1
  PostDashAttack:
    _animationStateName: Base Layer.Attack.Post Dash Attack
    _transitionDuration: 0.05
    _damageData:
      _damageType: 0
      _baseDamage: 200
      _physicForceScaling: 250
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 1
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 2
          outSlope: 2
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0.00000003143297, y: 0.077552676, z: 0.7363218}
    _hitboxHalfExtents: {x: 0.50000024, y: 0.44189537, z: 1.2980993}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 2
    _maxTargets: 8
  AnimatorController: {fileID: 22100000, guid: 667fb320325a72749b2e274ad6010218, type: 2}
  List_WeaponModels:
  - WeaponPrefabReference:
      m_AssetGUID: aac5a422caf682d45bb63a88e5b01e00
      m_SubObjectName: 
      m_SubObjectType: 
      m_SubObjectGUID: 
      m_EditorAssetChanged: 0
    SocketID: Hand_Right_1
    LocalPositionOffset: {x: -0.03, y: -0.012, z: 0.065}
    LocalEulerAngles: {x: 54.363, y: -95.612, z: 100.293}
    LocalScale: {x: 1, y: 1, z: 1}

```
## Assets\Scriptable Object\Weapon Enchantment\Heavy Hammer Sword\HeavyHammer_ComboAttackSetSO.asset
```yaml
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 8752c0cdc5e0b984dbd591abb37d7505, type: 3}
  m_Name: HeavyHammer_ComboAttackSetSO
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Combat.ComboAttackSetSO
  _list_ComboAttackData:
  - _animationStateName: Base Layer.Attack.Attack 1
    _transitionDuration: 0.02
    _damageData:
      _damageType: 0
      _baseDamage: 85
      _physicForceScaling: 200
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 1
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 1.02}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 1.02}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 5
  - _animationStateName: Base Layer.Attack.Attack 2
    _transitionDuration: 0.02
    _damageData:
      _damageType: 0
      _baseDamage: 85
      _physicForceScaling: 200
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 1
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 1.02}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 1.02}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 5
  - _animationStateName: Base Layer.Attack.Attack 3
    _transitionDuration: 0.02
    _damageData:
      _damageType: 0
      _baseDamage: 100
      _physicForceScaling: 200
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0.5
      KnockbackDuration: 0.1
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 1
          outSlope: 1
          tangentMode: 34
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 0.85}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 0.85}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 128
    _queryTriggerInteraction: 0
    _maxTargets: 5

```
## Assets\Prefab\Addressable\Enemies\Enemy (Ranged).prefab
```yaml
--- !u!114 &526492648152875300
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 7668894675120357355}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 6ef24f00f26f05448be7b57241c4c72e, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Combat.EnemyComboController
  _active: 1
  _attackHitbox: {fileID: 2416860590309405740}
  _animator: {fileID: 7345589929129320918}
  _characterAnimationController: {fileID: 8320340935477062313}
  _comboAttackSet: {fileID: 0}
  _list_ComboAttackData:
  - _animationStateName: Attack
    _transitionDuration: 0.1
    _damageData:
      _damageType: 1
      _baseDamage: 0
      _physicForceScaling: 0
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0
      KnockbackDuration: 0
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 0}
    _hitboxHalfExtents: {x: 0, y: 0, z: 0}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 0
    _queryTriggerInteraction: 0
    _maxTargets: 0
  _triggerIdle: 1
  _backToIdleTriggerParameter: Idle
  _comboRetentionDuration: 0.87
  _drawGizmos: 1
  _debugAttackIndex: 0
  _debugHitboxColor: {r: 1, g: 0.92156863, b: 0.015686275, a: 1}

```
## Assets\Prefab\Addressable\Enemies\Enemy (Melee).prefab
```yaml
--- !u!114 &2651949697427157578
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 54779437082285969}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: 6ef24f00f26f05448be7b57241c4c72e, type: 3}
  m_Name: 
  m_EditorClassIdentifier: Assembly-CSharp::Scripts.Combat.EnemyComboController
  _active: 1
  _attackHitbox: {fileID: 8733042703824664223}
  _animator: {fileID: 1651896244028188169}
  _characterAnimationController: {fileID: 301626572502348817}
  _comboAttackSet: {fileID: 0}
  _list_ComboAttackData:
  - _animationStateName: Attack
    _transitionDuration: 0.1
    _damageData:
      _damageType: 0
      _baseDamage: 10
      _physicForceScaling: 10
      _magicForceScaling: 0
    _attackKnockback:
      KnockbackDistance: 0
      KnockbackDuration: 0
      KnockbackCurve:
        serializedVersion: 2
        m_Curve:
        - serializedVersion: 3
          time: 0
          value: 0
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        - serializedVersion: 3
          time: 1
          value: 1
          inSlope: 0
          outSlope: 0
          tangentMode: 0
          weightedMode: 0
          inWeight: 0
          outWeight: 0
        m_PreInfinity: 2
        m_PostInfinity: 2
        m_RotationOrder: 4
    _hitboxCenterOffset: {x: 0, y: 0, z: 1}
    _hitboxHalfExtents: {x: 0.5, y: 0.5, z: 1}
    _targetLayerMask:
      serializedVersion: 2
      m_Bits: 64
    _queryTriggerInteraction: 0
    _maxTargets: 5
  _triggerIdle: 1
  _backToIdleTriggerParameter: Idle
  _comboRetentionDuration: 0.87
  _drawGizmos: 1
  _debugAttackIndex: 0
  _debugHitboxColor: {r: 1, g: 0.92156863, b: 0.015686275, a: 1}

```

