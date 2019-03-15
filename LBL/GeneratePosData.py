#!/usr/bin/env python
# coding: utf-8

# This reads from HD5 files and sends over network.
# This file sends drone position data over a server as x, y, z by sending at the right timestamp.
# Change the HOST and PORT variables accordingly.
import h5py 
import numpy as np
import math
import struct
import time
import socket
import json

f = h5py.File('RunData.h5', 'r')
x = f.keys()
pos_data = f["posData"]
HOST = '192.168.0.198'
PORT = 50008
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#s.bind((HOST, PORT))
s.connect((HOST, PORT))
offset = pos_data[0][0]
start = time.time()

for i in range(len(pos_data)):
    while(time.time() - start < pos_data[i][0] - offset):
        time.sleep(0.1)
	# sending message in format [label]:[timestamp]:data\n
    xyz = "Drone:" + str(pos_data[i][0]) + ":" + str(pos_data[i][1]) + "," + str(pos_data[i][2]) + "," + str(pos_data[i][3]) + "\n"
    print("Num: "  + str(i) + ": "+ xyz)
    # str_to_send = json.dumps(xyz)
    s.send(xyz.encode())
s.close()
f.close()