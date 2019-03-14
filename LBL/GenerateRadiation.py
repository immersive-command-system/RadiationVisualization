#!/usr/bin/env python
# coding: utf-8

# This file sends radiation data over a server as x,y,z, intensity.
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
rad_data = f["im3D"]
HOST = '192.168.0.198'
PORT = 50008
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
start = time.time()

for i in range(len(rad_data)):
	for j in range(len(rad_data[0])):
		for k in range(len(rad_data[0][0])):
			# sending message in format [label]:[timestamp]:x,y,z,intensity
			rad = "Radiation:" + str(0) + ":" + str(i) + ", " + str(j) + ", " + str(k) + ", " + str(rad_data[i][j][k]) + "\n"
			print(rad)
			# str_to_send = json.dumps(rad)
			s.send(rad.encode())
s.close()
f.close()