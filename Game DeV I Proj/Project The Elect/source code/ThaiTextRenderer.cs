using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Project_The_Elect.source_code
{
    /// <summary>
    /// Renders Thai text through Windows GDI/Uniscribe so combining vowels and tone marks
    /// are positioned as a Thai text run instead of as independent SpriteFont glyphs.
    /// </summary>
    public sealed class ThaiTextRenderer : IDisposable
    {
        private const int TextWidth = 1640;
        private const int TextHeight = 120;
        private const int TransparentBackground = 1;
        private const int AntialiasedQuality = 4;
        private const uint Blackness = 0x00000042;
        private const uint DrawTextNoPrefix = 0x00000800;
        private const uint DrawTextWordBreak = 0x00000010;

        private static bool _privateFontLoaded;
        private static IntPtr _privateFontHandle;
        private static IntPtr _privateFontMemory;
        private static readonly Dictionary<int, ThaiTextRenderer> SharedInstances = new Dictionary<int, ThaiTextRenderer>();

        private readonly GraphicsDevice _graphicsDevice;
        private readonly IntPtr _memoryDc;
        private readonly IntPtr _bitmap;
        private readonly IntPtr _bits;
        private readonly IntPtr _font;
        private readonly IntPtr _originalBitmap;
        private readonly IntPtr _originalFont;
        private readonly Texture2D _texture;
        private readonly byte[] _sourcePixels = new byte[TextWidth * TextHeight * 4];
        private readonly byte[] _texturePixels = new byte[TextWidth * TextHeight * 4];
        private string _lastText;
        private bool _disposed;

        private ThaiTextRenderer(GraphicsDevice graphicsDevice, string fontPath, int fontSize)
        {
            _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            if (!File.Exists(fontPath))
                throw new FileNotFoundException("Noto Serif Thai font file was not copied beside the game.", fontPath);

            if (!_privateFontLoaded)
            {
                byte[] fontBytes = File.ReadAllBytes(fontPath);
                _privateFontMemory = Marshal.AllocHGlobal(fontBytes.Length);
                Marshal.Copy(fontBytes, 0, _privateFontMemory, fontBytes.Length);
                uint loadedFontCount = 0;
                _privateFontHandle = AddFontMemResourceEx(
                    _privateFontMemory,
                    (uint)fontBytes.Length,
                    IntPtr.Zero,
                    ref loadedFontCount);
                if (_privateFontHandle == IntPtr.Zero || loadedFontCount == 0)
                    throw new InvalidOperationException("Windows could not load the Noto Serif Thai font file.");
                _privateFontLoaded = true;
            }

            _memoryDc = CreateCompatibleDC(IntPtr.Zero);
            if (_memoryDc == IntPtr.Zero)
                throw new InvalidOperationException("Windows could not create a text-rendering surface.");

            _font = CreateFont(
                -fontSize, 0, 0, 0, 600, 0, 0, 0,
                222, 0, 0, AntialiasedQuality, 0,
                "Noto Serif Thai SemiBold");
            if (_font == IntPtr.Zero)
                throw new InvalidOperationException("Windows could not create the Noto Serif Thai font.");

            BitmapInfo info = new BitmapInfo
            {
                Header = new BitmapInfoHeader
                {
                    Size = (uint)Marshal.SizeOf<BitmapInfoHeader>(),
                    Width = TextWidth,
                    Height = -TextHeight,
                    Planes = 1,
                    BitCount = 32,
                    Compression = 0,
                    SizeImage = (uint)(TextWidth * TextHeight * 4)
                }
            };

            _bitmap = CreateDIBSection(_memoryDc, ref info, 0, out _bits, IntPtr.Zero, 0);
            if (_bitmap == IntPtr.Zero || _bits == IntPtr.Zero)
                throw new InvalidOperationException("Windows could not create a text bitmap.");

            _originalBitmap = SelectObject(_memoryDc, _bitmap);
            _originalFont = SelectObject(_memoryDc, _font);
            SetBkMode(_memoryDc, TransparentBackground);
            SetTextColor(_memoryDc, 0x00FFFFFF);
            _texture = new Texture2D(_graphicsDevice, TextWidth, TextHeight, false, SurfaceFormat.Color);
        }

        public static ThaiTextRenderer GetShared(GraphicsDevice graphicsDevice, string fontPath, int fontSize = 42)
        {
            if (!SharedInstances.TryGetValue(fontSize, out ThaiTextRenderer renderer))
            {
                renderer = new ThaiTextRenderer(graphicsDevice, fontPath, fontSize);
                SharedInstances.Add(fontSize, renderer);
            }
            return renderer;
        }

        public static string[] SplitTextElements(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<string>();

            int[] starts = StringInfo.ParseCombiningCharacters(text);
            string[] elements = new string[starts.Length];
            for (int i = 0; i < starts.Length; i++)
            {
                int end = i + 1 < starts.Length ? starts[i + 1] : text.Length;
                elements[i] = text.Substring(starts[i], end - starts[i]);
            }
            return elements;
        }

        public void SetText(string text)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ThaiTextRenderer));
            if (text == _lastText)
                return;

            _lastText = text;
            PatBlt(_memoryDc, 0, 0, TextWidth, TextHeight, Blackness);
            if (!string.IsNullOrEmpty(text))
            {
                Rect bounds = new Rect { Left = 0, Top = 0, Right = TextWidth, Bottom = TextHeight };
                DrawText(_memoryDc, text, text.Length, ref bounds, DrawTextNoPrefix | DrawTextWordBreak);
            }

            Marshal.Copy(_bits, _sourcePixels, 0, _sourcePixels.Length);
            for (int i = 0; i < _sourcePixels.Length; i += 4)
            {
                byte alpha = Math.Max(_sourcePixels[i], Math.Max(_sourcePixels[i + 1], _sourcePixels[i + 2]));
                _texturePixels[i] = alpha;
                _texturePixels[i + 1] = alpha;
                _texturePixels[i + 2] = alpha;
                _texturePixels[i + 3] = alpha;
            }

            _texture.SetData(_texturePixels);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (_disposed || string.IsNullOrEmpty(_lastText))
                return;

            spriteBatch.Draw(_texture, position, Color.White);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _texture.Dispose();
            if (_originalFont != IntPtr.Zero)
                SelectObject(_memoryDc, _originalFont);
            if (_originalBitmap != IntPtr.Zero)
                SelectObject(_memoryDc, _originalBitmap);
            if (_bitmap != IntPtr.Zero)
                DeleteObject(_bitmap);
            if (_font != IntPtr.Zero)
                DeleteObject(_font);
            if (_memoryDc != IntPtr.Zero)
                DeleteDC(_memoryDc);
            _disposed = true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BitmapInfoHeader
        {
            public uint Size;
            public int Width;
            public int Height;
            public ushort Planes;
            public ushort BitCount;
            public uint Compression;
            public uint SizeImage;
            public int XPelsPerMeter;
            public int YPelsPerMeter;
            public uint ColorsUsed;
            public uint ColorsImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BitmapInfo
        {
            public BitmapInfoHeader Header;
            public uint Colors;
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr AddFontMemResourceEx(
            IntPtr font,
            uint size,
            IntPtr reserved,
            ref uint fontsAdded);

        [DllImport("gdi32.dll", EntryPoint = "CreateFontW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateFont(
            int height, int width, int escapement, int orientation, int weight,
            uint italic, uint underline, uint strikeOut, uint charSet,
            uint outputPrecision, uint clipPrecision, uint quality,
            uint pitchAndFamily, string faceName);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateDIBSection(
            IntPtr hdc, ref BitmapInfo info, uint usage, out IntPtr bits,
            IntPtr section, uint offset);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr obj);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern int SetBkMode(IntPtr hdc, int mode);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern uint SetTextColor(IntPtr hdc, uint color);

        [DllImport("user32.dll", EntryPoint = "DrawTextW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int DrawText(IntPtr hdc, string text, int length, ref Rect bounds, uint format);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool PatBlt(IntPtr hdc, int x, int y, int width, int height, uint operation);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr obj);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteDC(IntPtr hdc);
    }
}
