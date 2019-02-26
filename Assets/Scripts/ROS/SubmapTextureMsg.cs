using System.Text;
using System.IO;
using System.IO.Compression;

using UnityEngine.Assertions;

using SimpleJSON;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.geometry_msgs;
using System;

namespace ROSBridgeLib
{
    namespace cartographer_msgs
    {
        public class SubmapTextureMsg : ROSBridgeMsg
        {
            public byte[] cells;
        	public int width;
            public int height;
            public float resolution;
            public PoseMsg slice_pose;

            public SubmapTextureMsg(JSONNode node) {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(node["cells"]);
                using (GZipStream stream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        this.cells = memory.ToArray();
                    }
                }

                this.width = node["width"].AsInt;
                this.height = node["height"].AsInt;
                this.resolution = node["resolution"].AsFloat;
                this.slice_pose = new PoseMsg(node["slice_pose"]);
            }

            public SubmapTextureMsg(byte[] cells, int width, int height, float resolution, PoseMsg slice_pose) {
                this.cells = cells;
                this.width = width;
                this.height = height;
                this.resolution = resolution;
                this.slice_pose = slice_pose;
            }

            public static Tuple<byte[,], byte[,]> decode_cells(byte[] cells, int width, int height) {
                Assert.AreEqual(cells.Length, width * height, string.Format("Bad dimensions. Expected {0}\tgot{1}.", cells.Length, width * height));
                byte[,] intensity = new byte[height, width];
                byte[,] alpha = new byte[height, width];
                int index = 0;
                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++, index = index + 2) {
                        intensity[i, j] = cells[index];
                        alpha[i, j] = cells[index + 1];
                    }
                }
                return new Tuple<byte[,], byte[,]>(intensity, alpha);
            }
        }
    }
}