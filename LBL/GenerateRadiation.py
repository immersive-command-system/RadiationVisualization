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

# This reads from HD5 files and sends over network.
# This file sends pointcloud data over a server as x,y,z,intensity.
# x, y, z is multiplied by pixelsize.
# Change the HOST and PORT variables accordingly.

import argparse

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--HOST", help="Host to stream the radiation data from")
    parser.add_argument("--PORT", help="Port to stream the radiation data from")
    args = parser.parse_args()

    HOST = args.HOST
    PORT = int(args.PORT)

    if args.HOST == None:
        HOST = 'LOCALHOST'

    if args.PORT == None:
        PORT = 50009

    print("Reading in data...")
    f = h5py.File('RunData.h5', 'r')
    x = f.keys()
    rad_data = f["im3D"]
    lims = f["recon_lims"]

    print("Connecting...")
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((HOST, PORT))
    print("Sending")
    start = time.time()
    lower_lims = lims[0]
    pix_size = f["recon_pixSize"][0]
    s.send("{}".format(pix_size).encode())
    for i in range(len(rad_data)):
        for j in range(len(rad_data[0])):
            for k in range(len(rad_data[0][0])):
                # sending message in format [label]:[timestamp]:x,y,z,intensity
                if (rad_data[i][j][k] != 0.0):
                    x = i * pix_size + lower_lims[0]
                    y = j * pix_size + lower_lims[1]
                    z = k * pix_size + lower_lims[2]
                    rad = "Radiation:{}:{},{},{},{}\n".format(0,x,y,z,rad_data[i][j][k])
                    # rad = "Radiation:" + str(0) + ":" + str(i + lower_lims[0]) + ", " + str(j + lower_lims[1]) + ", " + str(k + lower_lims[2]) + ", " + str(rad_data[i][j][k]) + "\n"
                    print(rad)
                    # str_to_send = json.dumps(rad)
                    s.send(rad.encode())
    s.close()
    f.close()