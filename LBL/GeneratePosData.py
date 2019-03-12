#!/usr/bin/env python
# coding: utf-8

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
HOST = 'localhost'
PORT = 50007
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
#s.bind((HOST, PORT))
s.connect((HOST, PORT))
offset = pos_data[0][0]
start = time.time()

for i in range(len(pos_data)):
    while(time.time() - start < pos_data[i][0] - offset):
        time.sleep(0.1)
	# sending message in format [label]:[timestamp]:data
    xyz = "Drone:" + str(pos_data[i][0]) + ":" + str(xyz[0]) + ",", str(xyz[1]) + ",", str(xyz[2]) + "\n"
    print(xyz)
    # str_to_send = json.dumps(xyz)
    s.send(xyz.encode())
s.close()
f.close()