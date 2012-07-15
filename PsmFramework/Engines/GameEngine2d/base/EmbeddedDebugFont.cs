/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */
using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// Embedded font data for on screen debug prints.
	/// </summary>
	public class EmbeddedDebugFontData
	{
		/// <summary>
		/// The size of each character in pixels (this is a fixed size font).
		/// </summary>
		static public Vector2i CharSizei = new Vector2i(8, 8);

		/// <summary>
		/// The size of each character in pixels, as a Vector2.
		/// </summary>
		static public Vector2 CharSizef = CharSizei.Vector2();

		/// <summary>
		/// The number of Ascii characters available in this font, starting from ' '.
		/// </summary>
		static public int NumChars { get { return 95; } }

		/// <summary>
		/// Create a Texture2D object containing the font data.
		/// It is up to the caller to Dipose of the created texture.
		/// </summary>
		public static Texture2D CreateTexture ()
		{
			Texture2D texture;
			
			// SCEI PSP font
			
			uint[] SCE_PSP_FONT_DATA = new uint[95 * 2]
			{
				0x00000000, 0x00000000, // ' '
				0x10101010, 0x00100000, // '!'
				0x00282828, 0x00000000, // '"'
				0x287c2828, 0x0028287c, // '#'
				0x38147810, 0x00103c50, // '$'
				0x10204c0c, 0x00606408, // '%'
				0x08141408, 0x00582454, // '&'
				0x00102040, 0x00000000, // '''
				0x10102040, 0x00402010, // '('
				0x10100804, 0x00040810, // ')'
				0x10385410, 0x00105438, // '*'
				0x7c101000, 0x00001010, // '+'
				0x00000000, 0x08101000, // ','
				0x7c000000, 0x00000000, // '-'
				0x00000000, 0x00181800, // '.'
				0x10204000, 0x00000408, // '/'
				0x54644438, 0x0038444c, // '0'
				0x10141810, 0x007c1010, // '1'
				0x20404438, 0x007c0418, // '2'
				0x30404438, 0x00384440, // '3'
				0x24283020, 0x0020207c, // '4'
				0x403c047c, 0x00384440, // '5'
				0x3c040830, 0x00384444, // '6'
				0x1020447c, 0x00101010, // '7'
				0x38444438, 0x00384444, // '8'
				0x78444438, 0x00182040, // '9'
				0x00100000, 0x00001000, // ':'
				0x00100000, 0x08101000, // ';'
				0x0c183060, 0x00603018, // '<'
				0x007c0000, 0x0000007c, // '='
				0x6030180c, 0x000c1830, // '>'
				0x20404438, 0x00100010, // '?'
				0x54744438, 0x00380474, // '@'
				0x44442810, 0x0044447c, // 'A'
				0x3c48483c, 0x003c4848, // 'B'
				0x04044830, 0x00304804, // 'C'
				0x4848281c, 0x001c2848, // 'D'
				0x3c04047c, 0x007c0404, // 'E'
				0x3c04047c, 0x00040404, // 'F'
				0x74044438, 0x00384444, // 'G'
				0x7c444444, 0x00444444, // 'H'
				0x10101038, 0x00381010, // 'I'
				0x20202070, 0x00182420, // 'J'
				0x0c142444, 0x00442414, // 'K'
				0x04040404, 0x007c0404, // 'L'
				0x54546c44, 0x00444444, // 'M'
				0x544c4c44, 0x00446464, // 'N'
				0x44444438, 0x00384444, // 'O'
				0x3c44443c, 0x00040404, // 'P'
				0x44444438, 0x00582454, // 'Q'
				0x3c44443c, 0x00442414, // 'R'
				0x38044438, 0x00384440, // 'S'
				0x1010107c, 0x00101010, // 'T'
				0x44444444, 0x00384444, // 'U'
				0x44444444, 0x00102828, // 'V'
				0x54444444, 0x00446c54, // 'W'
				0x10284444, 0x00444428, // 'X'
				0x38444444, 0x00101010, // 'Y'
				0x1020407c, 0x007c0408, // 'Z'
				0x10101070, 0x00701010, // '['
				0x10080400, 0x00004020, // '\'
				0x1010101c, 0x001c1010, // ']'
				0x00442810, 0x00000000, // '^'
				0x00000000, 0x007c0000, // '_'
				0x00100804, 0x00000000, // '`'
				0x40380000, 0x00784478, // 'a'
				0x4c340404, 0x00344c44, // 'b'
				0x44380000, 0x00384404, // 'c'
				0x64584040, 0x00586444, // 'd'
				0x44380000, 0x0038047c, // 'e'
				0x7c105020, 0x00101010, // 'f'
				0x64580000, 0x38405864, // 'g'
				0x4c340404, 0x00444444, // 'h'
				0x10180010, 0x00381010, // 'i'
				0x10180010, 0x0c121010, // 'j'
				0x14240404, 0x0024140c, // 'k'
				0x10101018, 0x00381010, // 'l'
				0x542c0000, 0x00545454, // 'm'
				0x4c340000, 0x00444444, // 'n'
				0x44380000, 0x00384444, // 'o'
				0x4c340000, 0x0404344c, // 'p'
				0x64580000, 0x40405864, // 'q'
				0x4c340000, 0x00040404, // 'r'
				0x04780000, 0x003c403c, // 's'
				0x083c0808, 0x00304808, // 't'
				0x24240000, 0x00582424, // 'u'
				0x44440000, 0x00102844, // 'v'
				0x54440000, 0x00285454, // 'w'
				0x28440000, 0x00442810, // 'x'
				0x44440000, 0x38405864, // 'y'
				0x207c0000, 0x007c0810, // 'z'
				0x04080830, 0x00300808, // '{'
				0x10101010, 0x00101010, // '|'
				0x2010100c, 0x000c1010, // '}'
				0x0000007c, 0x00000000, // '~'
			};

			Vector2i font_size = new Vector2i (1024, 8);

			byte[] data = new byte[font_size.X * font_size.Y];

			for (int i=0; i < data.Length; ++i)
				data [i] = 0x00;

			for (int c = 0; c != NumChars; ++c)
			{
				for (int y = 0; y != CharSizei.Y; ++y)
				{
					for (int x = 0; x != CharSizei.X; ++x)
					{
						// get the pixel (x,y) for character c:
						bool white = false;
						
						uint a = SCE_PSP_FONT_DATA [c * 2 + 0];
						uint b = SCE_PSP_FONT_DATA [c * 2 + 1];
						uint index = (uint)(x + CharSizei.X * y);
						
						if (index < (uint)32)
							white = ((a & (1 << (int)index)) != 0);
						else
							white = ((b & (1 << (int)(index - 32))) != 0);
						
						data[(c * CharSizei.X + x) + y * font_size.X] = (byte)(white ? 0xff : 0x00);
					}
				}
			}
			
			texture = new Texture2D(font_size.X, font_size.Y, false, PixelFormat.Luminance);
			texture.SetPixels (0, data, PixelFormat.Luminance);
			
			texture.SetFilter (TextureFilterMode.Nearest, TextureFilterMode.Nearest, TextureFilterMode.Nearest);
			
			return texture;
		}
	}
}
