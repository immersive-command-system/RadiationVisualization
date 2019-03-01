#!/usr/bin/env python
# coding: utf-8

import h5py 
import numpy as np
import math
import struct
import time
import socket

f = h5py.File('RunData.h5', 'r')
x = f.keys()
pos_data = f["posData"]
HOST = 'localhost'
PORT = 50007
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((HOST, PORT))
s.listen(1)
conn, addr = s.accept()
print('Connected by', addr)
i = 0

while True:
	try:
		data = conn.recv(1024)
		if not data: break
		print("RECEIVED")
		conn.sendall(data)
	except socket.error:
		print("error")
		break
		
conn.close()