# Radiation Visualization README

# Radiation Visualization
## How to Clone This Repository

In the terminal, run:

`git clone` `--``recurse-submodules https://github.com/jeshlee121/ISAACS-RadiationVisualization`

**In the case of**  [****](https://help.github.com/en/articles/error-permission-denied-publickey)**“**[**Error: Permission denied (publickey)**](https://help.github.com/en/articles/error-permission-denied-publickey)**”**

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


- **Radiation**: Create another empty GameObject and attach `Scripts/Visualization/DronePositionSubscriber.cs` and drag the same ‘DataServer’ GameObject onto the ‘Server’ field.

For both of the PointCloud and Drone object, check the "**Flip YZ**" on the script tab.

For the DataServer object, go to the "**Listen Ports**" and change the size to 3 and for each element, choose an arbitrary PORT number (e.g. 50007, 50008, 50009).

**Python scripts**
Change the HOST and PORT accordingly in the scripts:

- **host_address**: Get the IP address ****from your terminal with the command `ipconfig` and replace the **HOST** variable with this IP address in the field `Wireless LAN adapter Wi-Fi: IPv4 Address:` . If you are trying to send it to your own computer, you can use `HOST =LOCALHOS``T`.
- **port_number**: Make sure the PORT numbers are the same as the ones that you wrote on the **DataServer** object.

Press play on Unity, and then run the scripts in the folder ‘LBL’ on a command prompt with the commands,
  

    python GeneratePosData.py --HOST <host_address> --PORT <port_number>
    python GenerateCloud.py --HOST <host_address> --PORT <port_number>
    python GenerateRadiation.py --HOST <host_address> --PORT <port_number>

  
If the flags for HOST and PORT are not provided, the default values for HOST and PORT are `LOCALHOST` and `50007`/`50008` /`50009` respectively. 
  
Go back to Unity, and you’ll see the visualizations start.

