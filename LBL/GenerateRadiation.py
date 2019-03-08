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
rad_data = f["im3D"]
HOST = '192.168.0.198'
PORT = 50007
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
start = time.time()

for i in range(len(rad_data)):
	for j in range(len(rad_data[0])):
		for k in range(len(rad_data[0][0])):
			rad = "Radiation: " + str(i) + ", " + str(j) + ", " + str(k) + ", " + str(rad_data[i][j][k])
			print(rad)
			str_to_send = json.dumps(rad)
			s.send(str_to_send.encode())
s.close()