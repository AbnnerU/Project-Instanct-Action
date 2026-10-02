"""Refine only the left guard and thumbs; preserve the punch and lower body."""
import math
import bpy
from mathutils import Vector

scene = bpy.context.scene
rig = next(o for o in scene.objects if o.type == 'ARMATURE')
source = rig.animation_data.action
source.use_fake_user = True
action = source.copy()
action.name = 'YBot_RightPunch_v02'
rig.animation_data.action = action
prefix = 'mixamorig:'

def aim(name, target):
    bone = rig.pose.bones[prefix + name]
    matrix = bone.matrix.copy()
    delta = (bone.tail - bone.head).rotation_difference((target - bone.head).normalized())
    result = (delta @ matrix.to_quaternion()).to_matrix().to_4x4()
    result.translation = matrix.translation
    bone.matrix = result
    bpy.context.view_layer.update()

frames = (1, 7, 10, 12, 14, 19, 28)
changed = ['LeftArm', 'LeftForeArm', 'LeftHand']
changed += [side + 'HandThumb' + str(j) for side in ('Right', 'Left') for j in (1, 2, 3)]
for frame in frames:
    scene.frame_set(frame)
    upper = rig.pose.bones[prefix + 'LeftArm']
    lower = rig.pose.bones[prefix + 'LeftForeArm']
    shoulder = upper.head.copy()
    wrist = Vector((16, 137, 25))
    forward = (wrist - shoulder).normalized()
    distance = (wrist - shoulder).length
    pole = Vector((0.15, -1, 0))
    bend = (pole - forward * pole.dot(forward)).normalized()
    along = (upper.length**2 - lower.length**2 + distance**2) / (2 * distance)
    elbow = shoulder + forward * along + bend * math.sqrt(max(0, upper.length**2 - along**2))
    aim('LeftArm', elbow)
    aim('LeftForeArm', wrist)
    hand = rig.pose.bones[prefix + 'LeftHand']
    aim('LeftHand', hand.head + Vector((0, 0.85, 0.5)))
    for side, sign in [('Right', 1), ('Left', -1)]:
        hand_matrix = rig.pose.bones[prefix + side + 'Hand'].matrix.copy()
        # Fold across the outside of the curled fingers, not into the palm.
        for joint, point in [(1, (4.0 * sign, 7.8, 4.3)),
                             (2, (2.0 * sign, 10.0, 7.0)),
                             (3, (-1.5 * sign, 10.0, 7.0))]:
            aim(side + 'HandThumb' + str(joint), hand_matrix @ Vector(point))
    for name in changed:
        bone = rig.pose.bones[prefix + name]
        bone.keyframe_insert('rotation_quaternion', frame=frame, group=bone.name)

for layer in action.layers:
    for strip in layer.strips:
        for bag in strip.channelbags:
            for curve in bag.fcurves:
                if any('"' + prefix + n + '"' in curve.data_path for n in changed):
                    for key in curve.keyframe_points:
                        key.handle_left_type = key.handle_right_type = 'AUTO_CLAMPED'
scene.frame_set(12)
print('Updated:', action.name)
