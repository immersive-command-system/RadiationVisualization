#!/usr/bin/env python
# coding: utf-8

# In[12]:

import h5py 
import numpy as np
import math
import struct
import time
import socket

HOST = 'localhost'
PORT = 50007
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
while(1):
	data = s.recv(1000)
#vals = data.split(,)
	print(data)
s.close()