# Radiation Visualization

## How to use the scripts

### Unity side
Create an empty GameObject and attach '''Scripts/DataServer.cs''' (Call this GameObject 'DataServer').

Create another empty GameObject and attach '''Scripts/Visualization/CloudDataSubscriber.cs''' and drag the earlier 'DataServer' GameObject onto the 'Server' field (Call this PointCloud).

Create one of the 'GameObjects/drone_model' and attach '''Scripts/Visualization/DronePositionSubscriber.cs''' and drag the same 'DataServer' GameObject onto the 'Server' field (Call this Drone).

For both of the PointCloud and Drone object, check the "Flip YZ" on the script tab.

For the DataServer object, go to the "Listen Ports" and change the size to 3 and for each element, choose an arbitrary PORT number (e.g. 50007, 50008, 50009).

### Python scripts
Change the HOST and PORT accordingly in the scripts to be the same PORT number from the Unity side and get the IP address from your terminal with 'ipconfig'.


### Start
Press play on Unity, and then run the scripts on a command prompt with the commands,
'''python GeneratePosData.py'''
'''python GenerateCloud.py'''