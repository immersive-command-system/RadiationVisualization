#!/usr/bin/env python
# coding: utf-8

# This file sends pointcloud data over a server as x,y,z,rgb. RGB value is currently arbitrary.
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
cld_data = f["cld"]
HOST = '192.168.0.198'
PORT = 50007
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
start = time.time()

for i in range(len(cld_data)):
	# sending message in format [label]:[timestamp]:x,y,z,rgb\n
	cloud = "Cloud:" + str(0) + ":" + str(cld_data[i][0]) + ", " + str(cld_data[i][1]) + ", " + str(cld_data[i][2]) + ", " + str(cld_data[i][3]) + "\n"
	print(cloud)
	# str_to_send = json.dumps(cloud)
	s.send(cloud.encode())
s.close()
f.close()