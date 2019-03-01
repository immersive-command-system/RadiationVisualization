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
s.connect((HOST, PORT))
offset = pos_data[0][0]
start = time.time()

for i in range(len(pos_data)):
    while(time.time() - start < pos_data[i][0] - offset):
        time.sleep(0.1)
    xyz = [pos_data[i][1], pos_data[i][2], pos_data[i][3]]
    print(str(xyz[0]) + ",", str(xyz[1]) + ",", str(xyz[2]))
    str_to_send = json.dumps(str(xyz[0]) + ", " + str(xyz[1]) + ", " + str(xyz[2]))
    s.send(str_to_send.encode())
s.close()