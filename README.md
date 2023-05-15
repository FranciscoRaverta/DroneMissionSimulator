# Drone Mission Simulator
Simulate drone flights and take images of your 3D objects.

## Description
This simulator was built on Unity. It was developed to fly over a [terrain](https://docs.unity3d.com/Manual//terrain-UsingTerrains.html) and take images of it, like a drone would. The project is based on [this](https://www.mapsmadeeasy.com/flight_planner) other flight planner. 

The project has three possible outputs:
1. The mission simulation, which outputs geotagged images and a XML with the simulation description.
2. 2D maps of your terrain can be generated, such as an elevation or segmentation map. It might be useful if you want to have a ground truth.
3. 3D meshes and point clouds of the terrain, trees or whole scene can also be saved. 

## Instalation
The project doesn't have a binary or something like that yet. We only provide the Unity project so you can run it by yourself. 
To run the simulator, you will have to:
1. Download [Unity Hub](https://store.unity.com/download?ref=personal)
2. Download a version of the Unity Editor (the project was built with the version 2019.1.5)
3. Clone or download this repo
4. Go to the folder `Assets > Scenes`, choose any of the folders and open the scene file (file with`.unity` extension).

### Important
The project has only been tested on Windows and Ubuntu 20.04. In fact, for Geotagging, we use [exiftool](https://www.sno.phy.queensu.ca/~phil/exiftool/). Independently on which operating system you are using, in the Geotagging script the current direction to the exiftool executable you are using should be provided.  

## Running
When openning the proyect, you might see some errors:
![Errors](https://user-images.githubusercontent.com/15222168/61068348-2e854b00-a3e0-11e9-935e-25b601edf45e.PNG)
That's ok, it's because some files were not added to the repo in order to reduce the size. Those files will be auto-generated, so don't worry.

### Simulation
To run the simulation, simply press the `Play` button. The simulation will start, and the georeferenced images will be generated in a folder named `Images` inside the repo (this can be modified of course). Once the simulation is over, and the images are generated and tagged, the simulation will stop by itself.
It might look like the simulation 'has lag'. This can happen if the flight speed and the images' resolution are too high, but the images will be captured correctly, so don't worry.

#### Flight Parameters
To modify the mission's parameters, you must first select the camera on the `Hierarchy` menu, and then you will be able to adjust your flight plan on the `Inspector` on the right.
![Steps](https://user-images.githubusercontent.com/15222168/61068352-30e7a500-a3e0-11e9-963f-241b11f5ab1f.PNG)

#### Light Baking
It might happen that at some point, the scene becomes a little darker. This is because Unity regenerated the high quality lights in the scene when it detects a change. You can know if this process is happening by checking at the bottom right corner.
If this process is ongoing, you can just wait a few seconds for the process to regenarate the lights.

### 2D Maps
To generate the 2D Maps, you can go to `Window > Terrain Tools`.

You will be able to determine the size and the quality of the map.

![Options](https://user-images.githubusercontent.com/15222168/61068359-37761c80-a3e0-11e9-975d-4c93ae77c7f3.PNG)

There are 4 tools provided: 
1. Convert Terrain Trees to Tree Objects: it is useful for obteining the 3D meshes and cloud points. 
2. Elevation Map: it can be used to generate a 2D Map of the height of the terrain, including (optionally) the trees' altitudes. 
3. Segmentation: it is used to generate a 2D image that separates the visible terrain and the visible trees from a top view.
4. Tree Crown Estimation: indicates in a 2D image the estimated tree crowns.

### 3D Meshes and Cloud Points:
Two tools where incorporated to the Forest Simulator in order to export the 3D meshes and Cloud Points: the [MeshTerrainEditor] (https://assetstore.unity.com/packages/tools/terrain/mesh-terrain-editor-free-67758) and the [SceneObjExporter] (https://assetstore.unity.com/packages/tools/utilities/scene-obj-exporter-22250). The first one is used to convert the terrain to a mesh mesh object, and the second is to export the meshes and point clouds of the scene. The recomended way to obtain the mesh and cloud points is:

1. Generate a new scene to edit. If this is done duplicating a pre-existing scene, then replace the terrain with a new one, because modifying a copied one will have the changes done in the original scene too.
2. Modify the terrain changing the altitude and adding trees with the terrain editor provided in unity.
3. Use the Mesh Terrain Editor Tool to create and object with mesh of the terrain, and the tool "Convert Terrain Trees to Tree Objects" to generate the tree objects with mesh to export. 
4. Make inactive the terrain, as we want to export the new terrain with mesh and the trees generated in the last step only. 
5. Use the SceneObjExporter to export the .obj file of the terrain, the trees or both of them, as it is needed. 
6. Finally, erase the terrain object created with the Mesh Terrain Editor and the tree objects and make active again the terrain. 


## Terrain
If you want to modify the terrain that already comes with the project, you can easily add/remove elevation, trees, grass, water, etc. Check the [documentation](https://docs.unity3d.com/Manual/script-Terrain.html) on how to do so.
The proyect already has some trees downloaded from the [Unity Asset Store](https://assetstore.unity.com/). You can download more if you like. To check the pre-downloaded models, navigate to `Assets > Models`.

## What's coming in the future?
There are a few improvement to be made, starting with:
1. Making the simulator cross platform.
2. Building the project so that it is not necessary to install Unity.
3. Fix problem with numeric formats where commas and dots are switched.

### Contributing 
If you happen to tackle any of the issue above, or any others, please send a PR our way and we will be more than happy to merge them!
