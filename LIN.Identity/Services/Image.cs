using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace LIN.Identity.Services;


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
        if (OperatingSystem.IsWindows())
            return ZipOnWindows(originalImage, width, height, max);
        else
            return ZipOthers(originalImage, width, height, max);
    }



    /// <summary>
    /// Comprime una imagen (En Windows)
    /// </summary>
    /// <param name="originalImage">Original image</param>
    /// <param name="width">Ancho</param>
    /// <param name="height">Alto</param>
    /// <param name="max">Maximo</param>
    [SupportedOSPlatform("windows")]
    public static byte[] ZipOnWindows(byte[] originalImage, int width = 50, int height = 50, int max = 1900)
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
            using (var graphics = Graphics.FromImage(nuevaImagen))
            {
                // Dibuja la imagen original en el nuevo Bitmap con las dimensiones deseadas
                graphics.DrawImage(image, 0, 0, 50, 50);
            }


            byte[] imagenBytes;
            using (MemoryStream stream = new())
            {
                nuevaImagen.Save(stream, ImageFormat.Jpeg);
                imagenBytes = stream.ToArray();
            }

            nuevaImagen.Dispose();
            image.Dispose();

            return imagenBytes;

        }
        catch
        {
        }
        return [];
    }



    /// <summary>
    /// Comprime una imagen (Plataformas .NET)
    /// </summary>
    /// <param name="originalImage">Original image</param>
    /// <param name="width">Ancho</param>
    /// <param name="height">Alto</param>
    public static byte[] ZipOthers(byte[] originalImage, int width = 100, int height = 100, int max = 1900)
    {
        try
        {

            MemoryStream memoryStream = new(originalImage);

            // Cargar imagen
            using Aspose.Imaging.Image pic = Aspose.Imaging.Image.Load(memoryStream);

            // Cambiar el tamaño de la imagen y guardar la imagen redimensionada

            pic.ResizeWidthProportionally(100);

            byte[] imagenBytes;
            using MemoryStream stream = new();
            pic.Save(stream);
            imagenBytes = stream.ToArray();

            var ss = Convert.ToBase64String(imagenBytes);

            return imagenBytes;

        }
        catch
        {
        }
        return [];
    }


}