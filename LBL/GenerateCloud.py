#!/usr/bin/env python
# coding: utf-8

# This reads from HD5 files and sends over network.
# This file sends pointcloud data over a server as x,y,z,rgb. RGB value is currently arbitrary.
# Change the HOST and PORT variables accordingly.
import h5py 
import numpy as np
import math
import struct
import time
import socket
import json
import argparse

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--HOST", help="Host to stream the point cloud from")
    parser.add_argument("--PORT", help="Port to stream the point cloud from")
    args = parser.parse_args()

    HOST = args.HOST
    PORT = args.PORT

    if args.HOST == None:
        HOST = 'LOCALHOST'

    if args.PORT == None:
        PORT = 50009
		
    try:
	    PORT = int(PORT)
    except ValueError:
        print("PORT variable must be an integer")
        print("Exitting now...")
        exit()

    print("Reading in data...")
    f = h5py.File('RunData.h5', 'r')
    x = f.keys()
    cld_data = f["cld"]

    print("Connecting...")
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((HOST, PORT))
    start = time.time()

    print("Sending")
    for i in range(len(cld_data)):
        # sending message in format [label]:[timestamp]:x,y,z,rgb\n
        cloud = "Cloud:" + str(0) + ":" + str(cld_data[i][0]) + ", " + str(cld_data[i][1]) + ", " + str(cld_data[i][2]) + ", " + str(cld_data[i][3]) + "\n"
        # print("Num: " + str(i) + "- " + cloud)
        s.send(cloud.encode())

    cloud = "Cloud:12345:End of Cloud\n"
    print(cloud)
    s.send(cloud.encode())
    s.close()
    f.close()
