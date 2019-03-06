using System.Text;
using System.IO;
using System.IO.Compression;

using UnityEngine.Assertions;

using SimpleJSON;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.geometry_msgs;
using System;
using UnityEngine;

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
                this.width = node["width"].AsInt;
                this.height = node["height"].AsInt;
                this.resolution = node["resolution"].AsFloat;
                this.slice_pose = new PoseMsg(node["slice_pose"]);

                Debug.Log("\tWidth: " + width + "\n\tHeight: " + height + "\n\tResolution: " + resolution + "\n\tCompressed Size: " + node["cells"].Value.GetType());

                byte[] data = Encoding.UTF8.GetBytes(node["cells"].Value);
                using (MemoryStream rawStream = new MemoryStream(data))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int cnt;
                        using (GZipStream stream = new GZipStream(rawStream, CompressionMode.Decompress))
                        {
                            Debug.Log("Beginning decompress.");
                            while ((cnt = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                Debug.Log("Read " + cnt);
                                memory.Write(buffer, 0, cnt);
                            }
                        }
                        this.cells = memory.ToArray();
                    }
                }
                //this.cells = CompressString.StringCompressor.DecompressString(node["cells"].Value);
                Debug.Log("Raw Size: " + this.cells.Length);
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

namespace CompressString
{
    internal static class StringCompressor
    {
        /// <summary>
        /// Compresses the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static byte[] DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            Debug.Log("Decompressing: " + gZipBuffer.Length);
            //byte[] gZipBuffer = Encoding.UTF8.GetBytes(compressedText);
            Debug.Log("Step 0");
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                Debug.Log("Step 1: " + dataLength);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                byte[] buffer = new byte[dataLength];

                memoryStream.Position = 0;
                Debug.Log("Step 2");
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    Debug.Log("Step 2.5");
                    gZipStream.Read(buffer, 0, buffer.Length);
                    Debug.Log("STep 2.5 Done");
                }
                Debug.Log("Done");
                return buffer;
            }
        }
    }
}