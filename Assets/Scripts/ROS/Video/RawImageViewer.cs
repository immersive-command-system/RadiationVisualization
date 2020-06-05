using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

public class RawImageViewer : MonoBehaviour
{
    public ImageConnection imageConnection = null;
    public RawImage cameraDisplay = null;

    public bool verbose = true;
    private static int num = 0;
    private float lastUpdateTime = 0;

    private void Update()
    {
        if (imageConnection == null)
        {
            imageConnection = GetComponent<ImageConnection>();
        }
        if (imageConnection != null && lastUpdateTime < imageConnection.last_update_time)
        {
            lastUpdateTime = Time.time;
            int curr = num;
            num++;
            if (verbose)
            {
                Debug.Log("Start processing " + curr);
            }

            uint width = imageConnection.width;
            uint height = imageConnection.height;
            byte[] raw_data = imageConnection.curr_image;
            string encoding = imageConnection.encoding.ToLower();

            if (encoding.Equals("bayer_bggr8"))
            {
                width /= 2;
                height /= 2;
            }

            Debug.LogFormat("{0} x {1}", width, height);

            Texture2D tex = cameraDisplay.texture as Texture2D;
            if (tex == null || width != tex.width || height != tex.height)
            {
                tex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                cameraDisplay.texture = tex;
            }

            NativeArray<Color32> image_data = tex.GetRawTextureData<Color32>();
            if (image_data == null || image_data.Length * 4 != raw_data.Length)
            {
                tex = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                cameraDisplay.texture = tex;
                image_data = tex.GetRawTextureData<Color32>();
            }

            if (encoding.Equals("bayer_bggr8"))
            {
                for (int ind = 0, o_ind = raw_data.Length - 4; ind < image_data.Length; ind += 1, o_ind -= 4)
                {
                    image_data[ind] = new Color32(raw_data[o_ind + 3], (byte)((raw_data[o_ind + 1] + raw_data[o_ind + 2]) / 2), raw_data[o_ind], 255);
                }
            } else if (encoding.Equals("mono8"))
            {
                for (int ind = 0, o_ind = raw_data.Length - 1; ind < image_data.Length; ind += 1, o_ind -= 1)
                {
                    image_data[ind] = new Color32(raw_data[o_ind], raw_data[o_ind], raw_data[o_ind], 255);
                }
            }

            //tex.LoadRawTextureData(image_data);
            tex.Apply();

            if (verbose)
            {
                Debug.Log("Finish processing " + curr);
            }
        }
    }
}