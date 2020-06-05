Official Windows installation instructions are [here](https://docs.microsoft.com/en-us/windows/mixed-reality/using-visual-studio#deploying-an-app-over-usb---hololens-1st-gen).
## Hololens Installation
1. Please follow the instructions in the [README](README.md) to clone this repository.
1. Install Unity. Currently tested on Unity 18 LTS, however any version of Unity should work (update 2019.3.6 does not appepar to work at the moment).
1. Install Visual Studio. Currently developed on VS 2019. Testing has also passed VS 2017 (Note that the modules. (Anything older is incapable of deploying to HoloLens.)
    - Enable the following modules (* marks HoloLens Deploy Dependencies):
        - Python
        - .NET
        - Desktop C++ *
        - Universal Winodws Platform development *
        - Game development with Unity
1. Set up Hololens in developer mode.

## Server Installation
1. [Install ROS](http://wiki.ros.org/melodic/Installation/Ubuntu).
    - `sudo apt install ros-melodic-desktop`
    - `echo "source /opt/ros/melodic/setup.bash" >> ~/.bashrc`
    - `source ~/.bashrc`
    - `sudo apt install python-rosdep python-rosinstall python-rosinstall-generator python-wstool build-essential`
    - `rosdep init`
    - `rosdep update`
1. Install ROSBridge-suite.
    - `sudo apt install ros-melodic-rosbridge-suite`
    - `rosdep install rosbridge-server`
1. Install custom messages. Remember to add them to your .bashrc file.
    - VoxBlox Messages: https://voxblox.readthedocs.io/en/latest/pages/Installation.html
    - PCFace Messages: To come
    
## Settings
1. Go to Build Settings and select Windows Universal Platform. (If no such setting, there should be a button that says install Universal Windows Platform module.)
    - Target Device: HoloLens
    - Architecture: x86
    - Build Type: D3D
1. Go to launch Player Settings: Build Settings > Player Settings:
    - Under XR Settings > Virtual Reality Supported: Enable

## Build
1. Build
    - I recommend that you build into the Build folder so it is neater.
1. Launch RadiationVisualization.sln (can be found inside the folder you built in).
1. Connect your Hololens
1. [Deploy](https://docs.microsoft.com/en-us/windows/mixed-reality/using-visual-studio#deploying-an-app-over-usb---hololens-1st-gen) to device.
    - Select `x86` as the build configuration.
    - Select `Device` as the target.
        - To see this option you may need to [pair your HoloLens](https://docs.microsoft.com/en-us/windows/mixed-reality/using-visual-studio#pairing-your-device) if you have not already.


## Testing ROSBridge Server
1. Download a rosbag of precollected data: 
    - Sample LAMP bag can be found [here](https://drive.google.com/file/d/1Vb4Heq2FtjIED0b3o-n2PA6WCDHOH-i3/view).
1. In the terminal, run:
`roscore`
1. In another terminal, run:
`roslaunch rosbridge_server rosbridge_websocket.launch`
4. Play the rosbag: `rosbag play out.bag`


## Testing ROSBridge Hololens
1. Update your server's ip address.
    - Get the ip with `hostname -I` or `ipconfig`
    - Set the `Ip` field in the `Rosbridge Connection` object in the ScanningHololensDemo scene with the ip address you just found.
    - You may need to set up port forwarding if the two devices are not in the same network.
1. Play within the Unity Editor just to make sure it works.
4. On the server, play the rosbag: `rosbag play out.bag`
1. Build the *ScanningHololensDemo Scene* to the Hololens.
4. On the server, play the rosbag: `rosbag play out.bag`
6. Launch the app on the Hololens if not already.
    - You should see flashes of white flying at you out from the drone model.
    - Don't panic, this is good news. I think.
7. Hololens view looks like red-squares.
