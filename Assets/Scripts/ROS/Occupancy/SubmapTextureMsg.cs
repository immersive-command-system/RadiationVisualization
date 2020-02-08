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

            private byte[] intensities;
            private byte[] alphas;

            public SubmapTextureMsg(JSONNode node) {
                this.width = node["width"].AsInt;
                this.height = node["height"].AsInt;
                this.resolution = node["resolution"].AsFloat;
                this.slice_pose = new PoseMsg(node["slice_pose"]);

                Debug.Log("\tWidth: " + width + "\n\tHeight: " + height + "\n\tResolution: " + resolution + "\n\tCompressed Size: " + node["cells"].Value.GetType());

                this.cells = DecompressGZipString(node["cells"]);

                Debug.Log("Raw Size: " + this.cells.Length);

                Tuple<byte[], byte[]> unpacked = decode_cells(cells, width, height);
                this.intensities = unpacked.Item1;
                this.alphas = unpacked.Item2;
            }

            public SubmapTextureMsg(byte[] cells, int width, int height, float resolution, PoseMsg slice_pose) {
                this.cells = cells;
                this.width = width;
                this.height = height;
                this.resolution = resolution;
                this.slice_pose = slice_pose;
            }

            public byte[] GetIntensities()
            {
                return this.intensities;
            }

            public byte[] GetAlphas()
            {
                return this.alphas;
            }
            

            private static Tuple<byte[], byte[]> decode_cells(byte[] cells, int width, int height) {
                Assert.AreEqual(cells.Length, width * height * 2, string.Format("Bad dimensions. Expected {0}\tgot {1}.", cells.Length, width * height * 2));
                byte[] intensity = new byte[height * width];
                byte[] alpha = new byte[height * width];

                for (int i = 0, j = 0; i < intensity.Length; i++, j += 2)
                {
                    intensity[i] = cells[j];
                    alpha[i] = cells[j + 1];
                }
                return new Tuple<byte[], byte[]>(intensity, alpha);
            }

            public static byte[] DecompressGZipString(string compressed)
            {
                byte[] data = System.Convert.FromBase64String(compressed);
                using (MemoryStream rawStream = new MemoryStream(data))
                {
                    using (MemoryStream memory = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int cnt;
                        using (GZipStream stream = new GZipStream(rawStream, CompressionMode.Decompress))
                        {
                            while ((cnt = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                memory.Write(buffer, 0, cnt);
                            }
                        }
                        return memory.ToArray();
                    }
                }
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