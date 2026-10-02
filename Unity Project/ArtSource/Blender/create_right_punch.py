"""Build an editable right punch on the imported Mixamo Y Bot, without changing its rest rig."""
import math
import bpy
from mathutils import Vector, Quaternion

scene = bpy.context.scene
rig = next(o for o in scene.objects if o.type == 'ARMATURE')
prefix = 'mixamorig:'
scene.frame_set(1)
original = rig.animation_data.action
original.use_fake_user = True
base = {b.name: b.matrix_basis.copy() for b in rig.pose.bones}
rig.animation_data.action = bpy.data.actions.new('YBot_RightPunch_v01')
action = rig.animation_data.action
action.use_fake_user = True
scene.render.fps = 30
scene.frame_start, scene.frame_end = 1, 28

def orient(name, head, direction):
    bone = rig.pose.bones[prefix + name]
    rest = bone.bone.matrix_local.to_quaternion()
    aim = (rest @ Vector((0, 1, 0))).rotation_difference(direction.normalized())
    matrix = (aim @ rest).to_matrix().to_4x4()
    matrix.translation = head
    bone.matrix = matrix
    bpy.context.view_layer.update()

def arm(side, target, hand_direction):
    upper = rig.pose.bones[prefix + side + 'Arm']
    forearm = rig.pose.bones[prefix + side + 'ForeArm']
    shoulder = upper.head.copy()
    delta = Vector(target) - shoulder
    distance = min(delta.length, upper.length + forearm.length - 0.7)
    forward = delta.normalized()
    wrist = shoulder + forward * distance
    pole = Vector((-0.55 if side == 'Right' else 0.55, -1, 0))
    bend = (pole - forward * pole.dot(forward)).normalized()
    along = (upper.length**2 - forearm.length**2 + distance**2) / (2 * distance)
    height = math.sqrt(max(0, upper.length**2 - along**2))
    elbow = shoulder + forward * along + bend * height
    orient(side + 'Arm', shoulder, elbow - shoulder)
    orient(side + 'ForeArm', elbow, wrist - elbow)
    orient(side + 'Hand', wrist, Vector(hand_direction))

# Frame, wrist target, fist direction, chest twist. Coordinates are rig-local centimeters.
poses = [
    (1, (-19, 146, 24), (0, 0.8, 0.6), -4),
    (7, (-24, 144, 15), (0, 0.8, 0.6), -12),
    (10, (-17, 145, 44), (0, 0.25, 1), 2),
    (12, (-11, 146, 54), (0, 0, 1), 12),
    (14, (-11, 146, 53), (0, 0, 1), 11),
    (19, (-23, 142, 32), (0, 0.5, 1), 0),
    (28, (-19, 146, 24), (0, 0.8, 0.6), -4),
]
for frame, target, hand_direction, twist in poses:
    scene.frame_set(frame)
    for bone in rig.pose.bones:
        bone.rotation_mode = 'QUATERNION'
        bone.matrix_basis = base[bone.name]
    bpy.context.view_layer.update()
    for name, weight in [('Spine', 0.2), ('Spine1', 0.35), ('Spine2', 0.45)]:
        bone = rig.pose.bones[prefix + name]
        matrix = bone.matrix.copy()
        rotation = Quaternion(Vector((0, 1, 0)), math.radians(twist * weight))
        new_matrix = (rotation @ matrix.to_quaternion()).to_matrix().to_4x4()
        new_matrix.translation = matrix.translation
        bone.matrix = new_matrix
        bpy.context.view_layer.update()
    arm('Right', target, hand_direction)
    arm('Left', (15, 150, 26), (0, 0.9, 0.45))
    for side in ('Right', 'Left'):
        for finger in ('Index', 'Middle', 'Ring', 'Pinky'):
            for joint, angle in ((1, 65), (2, 85), (3, 65)):
                bone = rig.pose.bones[prefix + side + 'Hand' + finger + str(joint)]
                axis = bone.bone.matrix_local.to_quaternion().inverted() @ Vector((0, 0, 1 if side == 'Right' else -1))
                bone.rotation_quaternion = Quaternion(axis, math.radians(angle))
        for joint, angle in ((1, 25), (2, 40), (3, 30)):
            bone = rig.pose.bones[prefix + side + 'HandThumb' + str(joint)]
            axis = bone.bone.matrix_local.to_quaternion().inverted() @ Vector((0, 1 if side == 'Right' else -1, 0))
            bone.rotation_quaternion = Quaternion(axis, math.radians(angle))
    for bone in rig.pose.bones:
        bone.keyframe_insert('location', frame=frame, group=bone.name)
        bone.keyframe_insert('rotation_quaternion', frame=frame, group=bone.name)
        bone.keyframe_insert('scale', frame=frame, group=bone.name)

for layer in action.layers:
    for strip in layer.strips:
        for bag in strip.channelbags:
            for curve in bag.fcurves:
                for key in curve.keyframe_points:
                    key.interpolation = 'BEZIER'
                    key.handle_left_type = key.handle_right_type = 'AUTO_CLAMPED'
for name, frame in [('Guard', 1), ('Anticipation', 7), ('Contact', 12), ('Recovery', 19), ('Guard Return', 28)]:
    scene.timeline_markers.new(name, frame=frame)
scene.frame_set(12)
for obj in scene.objects:
    obj.select_set(obj == rig)
bpy.context.view_layer.objects.active = rig
for area in bpy.context.screen.areas:
    if area.type == 'VIEW_3D':
        space = area.spaces.active
        space.shading.type = 'SOLID'
        space.shading.color_type = 'MATERIAL'
        space.overlay.show_overlays = False
        space.region_3d.view_rotation = Vector((3, -5, 1.6)).to_track_quat('Z', 'Y')
        space.region_3d.view_distance = 3.4
        space.region_3d.view_location = Vector((0, -0.1, 0.95))
print('Created', action.name, 'frames', list(action.frame_range))
