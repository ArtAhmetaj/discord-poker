using System.Collections.Generic;
using System.Drawing;
public class ImageMerger
{
    public static Image mergeImages(string[] paths)
    {
        int height, width;
        height = 200 * paths.Length;
        width = 100 * paths.Length;
        Image img = new Bitmap(height, width);
        // Image img2 = Image.FromFile("Image.jpg");
        Graphics g = Graphics.FromImage(img);

        for (int i = 0; i < paths.Length; i++)
        {
            g.DrawImage(Image.FromFile(paths[i]), new Point((i * 200) + 20, 50));
        }
        return img;
    }

}
