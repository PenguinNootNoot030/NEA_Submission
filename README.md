# Digital Image Watermarking and Robustness Testing

A Windows desktop application built with **C#**, **.NET 8** and **WPF** to explore text watermarking in digital images. It implements spatial-domain and frequency-domain embedding methods, along with image transformations for investigating how watermarks survive subsequent processing.

Originally developed as an independent **A-Level Computer Science NEA** project, with an emphasis on implementing the underlying image-processing and mathematical operations rather than relying on a dedicated watermarking library.

## What it does

- **Embed text watermarks** using either least-significant-bit (LSB) modification of a selected RGB channel or discrete cosine transform (DCT) coefficients in the luminance channel.
- **Extract embedded text** from an image using the corresponding method and parameters.
- **Apply image transformations** including rotation, Gaussian blur, centre cropping and a custom DCT-based lossy-compression simulation.
- **Experiment interactively** through a WPF interface with configurable parameters for each operation.

## How it works

### Watermark encoding

Text is converted to a bitstream with an **8-bit message-length prefix**, followed by one byte per character. The implementation accepts messages of up to **50 characters**; basic ASCII text is recommended because character encoding is handled as single-byte values.

### LSB watermarking

The LSB method writes individual message bits into the least significant bits of pixel values in a user-selected **red, green or blue** channel. The extractor reads the corresponding channel to reconstruct the bitstream.

### DCT watermarking

Images are divided into **8 × 8 pixel blocks**. The DCT method converts RGB data to **YCbCr**, transforms the luminance (Y) component into the frequency domain, modifies a selected coefficient to carry each bit, and applies an inverse DCT to reconstruct the image.

The project includes a custom matrix class and implementations of DCT basis generation, DCT/IDCT matrix transformations, and RGB–YCbCr conversion. DCT basis matrices are cached by block size.

### Image traversal and transformation

Both embedding and extraction use a **quadtree-based depth-first traversal** of image blocks to establish a consistent processing order. The application also provides four image-processing operations for experimenting with watermark recovery after modification:

| Operation | Implementation |
| --- | --- |
| Rotation | Angle-controlled image rotation |
| Gaussian blur | Configurable kernel size and blur intensity |
| Cropping | Centre crop based on a selected ratio |
| Lossy compression simulation | DCT-domain coefficient reduction and reconstruction |

The compression feature is a **custom simulation**, not an implementation of JPEG file encoding.

## Running the application

**Requirements:** Windows, the **.NET 8 SDK** and a Windows-capable development environment with WPF support. **Visual Studio 2022** with the .NET desktop development workload is recommended.

1. Clone this repository and open `Testing_WPF.sln` in Visual Studio.
2. Restore the project dependencies and build/run the WPF application.
3. Enter an image's local file path and select **Upload**.
4. Choose a mode (**Embed**, **Attack** or **Evaluate**), select an operation and provide the requested parameters.
5. Select **Apply**. For extraction, use the same watermarking method and parameters used during embedding.

The project targets `net8.0-windows` and references `System.Drawing.Common`. Because the project file also contains a COM reference, some build environments may require Visual Studio/MSBuild rather than a command-line-only setup.

## Code structure

| Files | Responsibility |
| --- | --- |
| `MainWindow.xaml`, `MainWindow.xaml.cs` | WPF interface and user interaction |
| `Watermark.cs` | Message validation, bitstream creation and decoding |
| `Embedder.cs`, `Extractor.cs` | Shared abstractions for watermark operations |
| `LSBWEmbedder.cs`, `LSBWExtractor.cs` | Spatial-domain embedding and extraction |
| `DCTEmbedder.cs`, `DCTExtractor.cs` | Frequency-domain embedding and extraction |
| `Matrix.cs`, `Methods.cs`, `Block.cs` | Matrix operations, DCT/IDCT and colour-space conversion |
| `ImageHandling.cs` | Bitmap conversion, block handling and quadtree traversal |
| `Attack.cs`, `ImageRotation.cs`, `ImageBlurring.cs`, `ImageCropping.cs`, `ImageCompression.cs` | Image transformation implementations |

## Scope and limitations

This is an educational **prototype**, not a production-ready watermarking or copyright-protection tool. Watermark extraction can be affected by image transformations, numerical rounding and the selected DCT parameters; successful recovery is **not guaranteed** across all settings. The repository provides interactive operations rather than an automated benchmark with measured recovery rates.

Images are processed and displayed in the WPF application; the current interface does **not** include a dedicated export/save command for processed images.

## Project background

Created for an A-Level Computer Science Non-Exam Assessment (NEA), with a focus on applying mathematics and software design to a practical image-processing problem. The main technical work includes custom linear-algebra utilities, image block processing, two watermarking approaches and a configurable desktop interface.
