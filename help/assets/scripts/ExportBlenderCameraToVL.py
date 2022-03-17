import bpy
import xml.etree.ElementTree as ET

depsgraph = bpy.context.evaluated_depsgraph_get()

s_objects = bpy.context.selected_objects
camera = None

for object in s_objects:
    if object.type == 'CAMERA':
        camera = object
    elif 'CAMERA' not in object.type:
        continue
    
if camera == None:
    print('no camera was found')
elif camera.type == 'CAMERA':
    print('camera selected: '+ camera.name)
    
    

node = ET.Element('BL_CAMERA')
node.attrib= {'name':camera.name}
world_matr = camera.matrix_world.copy()
world_matr = world_matr.inverted()
projection_matrix = camera.calc_matrix_camera(depsgraph)
projection_matrix = projection_matrix.copy()
projection_matrix.transpose()

world_matr.transpose()

#RENDERER
renderer_engine =  bpy.context.scene.render.engine
renderer_resolusion = [bpy.context.scene.render.resolution_x, bpy.context.scene.render.resolution_y, bpy.context.scene.render.resolution_percentage]
node_renderer = ET.SubElement(node, 'RENDERER')
node_renderer.attrib = {'engine': renderer_engine, 'resolution_x':str(renderer_resolusion[0]), 'resolution_y':str(renderer_resolusion[1]), 'resolution_percentage':str(renderer_resolusion[2])}

node_data = ET.SubElement(node, 'DATA')
node_lens = ET.SubElement(node_data, 'LENS')
node_lens.text = str(camera.data.lens)
node_lens.attrib= {'unit':'mm'}
node_clip = ET.SubElement(node_data, 'CLIP')
node_clip.attrib = {'end': str(camera.data.clip_end), 'start': str(camera.data.clip_start)}
node_sensor = ET.SubElement(node_data, 'SENSOR')
node_sensor.attrib = {'width': str(camera.data.sensor_width), 'height': str(camera.data.sensor_height), 'fit':camera.data.sensor_fit }
node_shift = ET.SubElement(node_data, 'SHIFT')
node_shift.attrib = {'x': str(camera.data.shift_x), 'y': str(camera.data.shift_y)}



node_matrix = ET.SubElement(node, 'MATRIX')
node_matrix.attrib = {'id':'0', 'name':'WORLD'}
node_rows = ET.SubElement(node_matrix, 'ROWS')

for i, row in enumerate(world_matr):
    node_row = ET.SubElement(node_rows, 'ROW')
    node_row.attrib = {'id':str(i)}
    for j, co in enumerate(row):
        node_col = ET.SubElement(node_row, 'COL')
        node_col.attrib = {'id':str(j)}
        node_col.text = str(co)
        print(co)

node_matrix = ET.SubElement(node, 'MATRIX')
node_matrix.attrib = {'id':'0', 'name':'PROJECTION'}
node_rows = ET.SubElement(node_matrix, 'ROWS')

for i, row in enumerate(projection_matrix):
    node_row = ET.SubElement(node_rows, 'ROW')
    node_row.attrib = {'id':str(i)}
    for j, co in enumerate(row):
        node_col = ET.SubElement(node_row, 'COL')
        node_col.attrib = {'id':str(j)}
        node_col.text = str(co)
        print(co)

ET.dump(node)



        