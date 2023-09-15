using System.Drawing;
using System.Drawing.Imaging;

namespace LIN.Auth.Services;


public class Image
{


    /// <summary>
    /// Comprime una imagen
    /// </summary>
    /// <param name="originalImage">Original image</param>
    /// <param name="width">Ancho</param>
    /// <param name="height">Alto</param>
    /// <param name="max">Maximo</param>
    public static byte[] Zip(byte[] originalImage, int width = 50, int height = 50, int max = 1900)
    {
        try
        {

            // Si la imagen ya esta comprimida o pesa muy poco
            if (originalImage.Length <= max)
                return originalImage;

            // Cargar la imagen a memoria
            MemoryStream memoryStream = new(originalImage);

            // Crear la imagen
            var image = System.Drawing.Image.FromStream(memoryStream);

            // Imagen redimensionada
            Bitmap nuevaImagen = new(width, height);

            // Crea un objeto Graphics para dibujar la imagen original en el Bitmap redimensionado
            using (Graphics graphics = Graphics.FromImage(nuevaImagen))
            {
                // Dibuja la imagen original en el nuevo Bitmap con las dimensiones deseadas
                graphics.DrawImage(image, 0, 0, 50, 50);
            }


            byte[] imagenBytes;
            using (MemoryStream stream = new())
            {
                nuevaImagen.Save(stream, ImageFormat.Png);
                imagenBytes = stream.ToArray();
            }

            nuevaImagen.Dispose();
            image.Dispose();

            return imagenBytes;
        }
        catch
        {
        }
        return Array.Empty<byte>();
    }


}
