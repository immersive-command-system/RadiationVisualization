# Radiation Visualization
## How to Clone This Repository
Ensure that you have an SSH key associated with your account as you will need that to clone submodules.

In the terminal, run:

`git clone --recurse-submodules -j8 https://github.com/immersive-command-system/RadiationVisualization`

**In the case of** **“**[**Error: Permission denied (publickey)**](https://help.github.com/en/articles/error-permission-denied-publickey)**”**
Please note that the Error is not specially highlighted so you may need to look closely.

**For Windows:** 

[First make sure you have a key that is being used](https://help.github.com/en/articles/error-permission-denied-publickey#make-sure-you-have-a-key-that-is-being-used).

If `ssh-add -l` or `ssh-add -l -E md5` returns "[The agent has no identities](https://stackoverflow.com/questions/26505980/github-permission-denied-ssh-add-agent-has-no-identities).”:

Use the command `ssh-keygen -t rsa` to generate the keys and `ssh-add /path/to/my-ssh-folder/id_rsa`  (e.g. path: `~/.ssh/id_rsa` ) to add them. Then try `ssh-add` again. 


Now follow this [link](https://help.github.com/en/articles/adding-a-new-ssh-key-to-your-github-account) to add a new SSH key to your GitHub account. 
Make sure to delete the old repo you had downloaded, and try cloning again.


## How to Use the Python Scripts in ‘LBL’ Folder (HD5 Data Over Network)

**Unity Side -** An example of this has been created and saved in “Scenes/Visualization.unity”.


- **DataServer**: Create an empty GameObject and attach `Scripts/DataServer.cs`.


- **PointCloud**: Create another empty GameObject and attach `Scripts/Visualization/CloudDataSubscriber.cs` and drag the earlier 'DataServer' GameObject onto the 'Server' field.


- **Drone**: Create one of the 'GameObjects/drone_model' from the Assets folder and attach `Scripts/Visualization/DronePositionSubscriber.cs` and drag the same 'DataServer' GameObject onto the 'Server' field.


- **Radiation**: Create another empty GameObject and attach `Scripts/Visualization/RadiationPositionSubscriber.cs` and drag the same ‘DataServer’ GameObject onto the ‘Server’ field.

For both of the PointCloud and Drone object, check the "**Flip YZ**" on the script tab.

For the DataServer object, go to the "**Listen Ports**" and change the size to 3 and for each element, choose an arbitrary PORT number (e.g. 50007, 50008, 50009).

Make sure you click the check box in the `Inspector` for each of the above GameObjects. 

**Python scripts**
Change the HOST and PORT accordingly in the scripts:

- **host_address**: Get the IP address from your terminal with the command `ipconfig` and replace the **HOST** variable with this IP address in the field `Wireless LAN adapter Wi-Fi: IPv4 Address:` . If you are trying to send it to your own computer, you can use `HOST =LOCALHOST`.
- **port_number**: Make sure the PORT numbers are the same as the ones that you wrote on the **DataServer** object.

Press play on Unity, and then run the scripts in the folder ‘LBL’ on a command prompt with the commands,


    python GeneratePosData.py --HOST <host_address> --PORT <ELEMENT0>
    python GenerateRadiation.py --HOST <host_address> --PORT <ELEMENT2>
    python GenerateCloud.py --HOST <host_address> --PORT <ELEMENT1>
    
    RUN GenerateRadiation.py before GenerateCloud.py

  


If the flags for HOST and PORT are not provided, the default values for HOST and PORT are `LOCALHOST` and `50007`/`50008`/`50009` respectively. 

Go back to Unity, and you’ll see the visualizations start.

## Resources

On Google Drive inside `ISAACS/Spring 2019/Radiation Visulization`. https://drive.google.com/open?id=18_KtS9UtNDZLGDmeRgFtmxaSC26-FbCR

- `/camp_roberts`
  - `/roberts_recon_in_bag` or `data.bag` contains the ros bag and the `h5` data. We will be extracting drone position, radiation data from `RunData.h5`. (Note this `RunData.h5` file is included in our git repo. No need to download and replace.)
  - `/data.bag_points.ply` point cloud data

- `/Dataset Information google doc` contains the information for the data inside `RunData.h5`. (Need to understand and read carefully.)

