using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using Xunit;

namespace ImageSharp.BmpBug;

public class BmpSavedWrong
{
    // Fails on ImageSharp >= 2.0.0
    [Fact]
    public void File_SaveAndReopen_Should_Results_In_Same_PixelData()
    {
        // Load original bmp datamatrix into pixel array
        using var dmImage = Image.Load<Rgb24>("datamatrix.bmp");
        var originalPixelData = _getPixelData(dmImage);

        // Save bmp image, and open saved version, then load into pixel array
        using var changed = _saveAndReopen(dmImage);
        var saveAndReopenedPixelData = _getPixelData(changed);

        // Assert that both arrays of pixels are the same
        Assert.Equal(originalPixelData, saveAndReopenedPixelData);
    }

    // Always succeeds
    [Fact]
    public void PixelArray_SaveAndReopen_Should_Results_In_Same_PixelData()
    {
        // Load original bmp datamatrix into pixel array
        using var dmImage = _createDataMatrixImage();
        var originalPixelData = _getPixelData(dmImage);

        // Create new image by copying just the pixel data,
        // Then save bmp image, and open saved version, then load into pixel array
        using var changed = _saveAndReopen(dmImage);
        var saveAndReopenedPixelData = _getPixelData(changed);

        // Assert that both arrays of pixels are the same
        Assert.Equal(originalPixelData, saveAndReopenedPixelData);
    }

    // Always succeeds
    [Fact]
    public void PixelArrayAndFile_Have_Same_PixelData()
    {
        // Load original bmp datamatrix into pixel array
        using var fromFile = _createDataMatrixImage();
        var originalPixelData = _getPixelData(fromFile);

        // Load datamatrix from hardcoded array
        using var fromArray = _createDataMatrixImage();
        var saveAndReopenedPixelData = _getPixelData(fromArray);

        // Assert that both arrays of pixels are the same
        Assert.Equal(originalPixelData, saveAndReopenedPixelData);
    }

    private static Image<Rgb24> _saveAndReopen(Image<Rgb24> input)
    {
        using (var ms = new MemoryStream())
        {
            input.SaveAsBmp(ms);
            ms.Position = 0;

            return Image.Load<Rgb24>(ms);
        }
    }

    private static Image<Rgb24> _createDataMatrixImage()
    {
        var b = new Rgb24(0x00, 0x00, 0x00);
        var w = new Rgb24(0xFF, 0xFF, 0xFF);

        var pixelData = new Rgb24[]
        {
             b, w, b, w, b, w, b, w, b, w,
             b, b, w, w, b, w, b, b, w, b,
             b, b, w, w, w, w, w, b, w, w,
             b, b, w, w, w, b ,b ,b, w, b,
             b, b, w, w, w, w, b, w, w, w,
             b, w, w, w, w, w, b, b, b, b,
             b, b, b, w, b, b, w, w, w, w,
             b, b, b, b, w, b, b, w, w, b,
             b, w, w, b, b, b, w, b, w, w,
             b, b, b, b, b, b, b, b, b, b
        };

        return Image.LoadPixelData<Rgb24>(pixelData, 10, 10);
    }

    private Rgb24[] _getPixelData(Image<Rgb24> input)
    {
        var pixels = new Rgb24[input.Width * input.Height];

        for(int y = 0; y < input.Height; y++)
        {
            for(int x = 0; x < input.Width; x++)
            {
                pixels[y * input.Width + x] = input[x, y];
            }
        }

        return pixels;
    }
}
