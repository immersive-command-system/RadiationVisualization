
## Installation
1. Please follow the instructions in the [README](README.md) to clone this repository.
1. Install Unity. Currently tested on Unity 18 LTS, however any version of Unity should work.
1. Install Visual Studio. Currently tested on VS 2019.
    - Enable the following modules:
        - Python
        - .NET
        - C++
        - Universal Winodws Platform development
        - Game development with Unity 
1. Set up Hololens in developer mode.
1. Install ROS and the ROSBridge-suite.
    - sudo apt install `ros-melodic-desktop`
    - sudo apt install `ros-melodic-rosbridge-suite`
    
## Settings
1. Go to Build Settings and select Windows Universal Platform.
    - Target Device: HoloLens
    - Architecture: x86
    - Build Type: D3D
1. Go to Player Settings > XR Settings > Virtual Reality Supported: Enable

## Build
1. Build
    - I recommend that you build into the Build folder so it is neater.
1. Launch RadiationVisualization.sln (can be found inside the folder you built in).
1. Connect your Hololens
1. Deploy to local device.

## Testing ROSBridge
1. Download a rosbag of precollected data: 
    - Sample LAMP bag can be found [here](https://drive.google.com/file/d/1Vb4Heq2FtjIED0b3o-n2PA6WCDHOH-i3/view).
1. In the terminal, run:
`roscore`
1. In another terminal, run:
`roslaunch rosbridge_server rosbridge_websocket.launch`
1. Build the *ScanningHololensDemo Scene* to the Hololens.
4. Play the rosbag: `rosbag play out.bag`
6. Launch the app on the Hololens if not already.
    - You should see flashes of white flying at you out from the drone model.
    - Don't panic, this is good news. I think.
