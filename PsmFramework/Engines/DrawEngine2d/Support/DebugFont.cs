using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	public static class DebugFont
	{
		public const String TextureKey = "DebugFont";
		
		public static void CreateFont(DrawEngine2d drawEngine2d)
		{
			//These two should be in sync all the time.
			//They should be unified by combining the upper and lower data
			// into a single UInt and kept as a single dict.
			var upper = new Dictionary<Char, UInt32>();
			var lower = new Dictionary<Char, UInt32>();
			
			PopulateDicts(ref upper, ref lower);
			
			Int32 textureWidth = MaxTextureCharCapacity * FontWidth;
			Int32 textureHeight = FontHeight;
			
			Byte[] texturePixels = new Byte[textureWidth * textureHeight];
			
			//This loop references only one of the two glyph dicts but
			// that is just a convenience. The data from both will be 
			// used during the loop.
			foreach(Char c in upper.Keys)
				DecodePixelData(ref upper, ref lower, ref texturePixels, c, textureWidth);
			
			Texture2dPlus texture = new Texture2dPlus(drawEngine2d,TextureCachePolicy.KeepAlways, TextureKey, textureWidth, textureHeight, PixelFormat.Luminance);
			texture.SetPixels(0, texturePixels, PixelFormat.Luminance);
			texture.SetFilter(TextureFilterMode.Nearest, TextureFilterMode.Nearest, TextureFilterMode.Nearest);
			
			TiledTexture tiledTexture = new TiledTexture(drawEngine2d,TextureCachePolicy.KeepAlways, DebugFont.TextureKey, texture);
			tiledTexture.CreateColumnIndex(MaxTextureCharCapacity);
		}
		
		public static void RemoveFont(DrawEngine2d drawEngine2d)
		{
			//TODO: Do we really need to do anything?
		}
		
		//TODO: This should be cached.
		public static TiledTextureIndex GetCharTileIndex(Char c)
		{
			Int32 index = GetGlyphIndex(c);
			return new TiledTextureIndex(index);
		}
		
		//These values are tied directly to the hardcoded glyph data and
		// should not be altered.
		public const Int32 FontWidth = 8;
		public const Int32 FontHeight = 8;
		
		private const Int32 NotPrintableChars = 32;
		
		private const Int32 MaxTextureCharCapacity = 128;
		
		private const Byte PixelDark = (Byte)0x00;
		private const Byte PixelLit = (Byte)0xff;
		
		private static void PopulateDicts(ref Dictionary<Char, UInt32> upper, ref Dictionary<Char, UInt32> lower)
		{
			AddCharToDicts(ref upper, ref lower, 0x00000000, 0x00000000, ' ');
			AddCharToDicts(ref upper, ref lower, 0x10101010, 0x00100000, '!');
			AddCharToDicts(ref upper, ref lower, 0x00282828, 0x00000000, '"');
			AddCharToDicts(ref upper, ref lower, 0x287c2828, 0x0028287c, '#');
			AddCharToDicts(ref upper, ref lower, 0x38147810, 0x00103c50, '$');
			AddCharToDicts(ref upper, ref lower, 0x10204c0c, 0x00606408, '%');
			AddCharToDicts(ref upper, ref lower, 0x08141408, 0x00582454, '&');
			AddCharToDicts(ref upper, ref lower, 0x00102040, 0x00000000, '\'');
			AddCharToDicts(ref upper, ref lower, 0x10102040, 0x00402010, '(');
			AddCharToDicts(ref upper, ref lower, 0x10100804, 0x00040810, ')');
			AddCharToDicts(ref upper, ref lower, 0x10385410, 0x00105438, '*');
			AddCharToDicts(ref upper, ref lower, 0x7c101000, 0x00001010, '+');
			AddCharToDicts(ref upper, ref lower, 0x00000000, 0x08101000, ',');
			AddCharToDicts(ref upper, ref lower, 0x7c000000, 0x00000000, '-');
			AddCharToDicts(ref upper, ref lower, 0x00000000, 0x00181800, '.');
			AddCharToDicts(ref upper, ref lower, 0x10204000, 0x00000408, '/');
			AddCharToDicts(ref upper, ref lower, 0x54644438, 0x0038444c, '0');
			AddCharToDicts(ref upper, ref lower, 0x10141810, 0x007c1010, '1');
			AddCharToDicts(ref upper, ref lower, 0x20404438, 0x007c0418, '2');
			AddCharToDicts(ref upper, ref lower, 0x30404438, 0x00384440, '3');
			AddCharToDicts(ref upper, ref lower, 0x24283020, 0x0020207c, '4');
			AddCharToDicts(ref upper, ref lower, 0x403c047c, 0x00384440, '5');
			AddCharToDicts(ref upper, ref lower, 0x3c040830, 0x00384444, '6');
			AddCharToDicts(ref upper, ref lower, 0x1020447c, 0x00101010, '7');
			AddCharToDicts(ref upper, ref lower, 0x38444438, 0x00384444, '8');
			AddCharToDicts(ref upper, ref lower, 0x78444438, 0x00182040, '9');
			AddCharToDicts(ref upper, ref lower, 0x00100000, 0x00001000, ':');
			AddCharToDicts(ref upper, ref lower, 0x00100000, 0x08101000, ';');
			AddCharToDicts(ref upper, ref lower, 0x0c183060, 0x00603018, '<');
			AddCharToDicts(ref upper, ref lower, 0x007c0000, 0x0000007c, '=');
			AddCharToDicts(ref upper, ref lower, 0x6030180c, 0x000c1830, '>');
			AddCharToDicts(ref upper, ref lower, 0x20404438, 0x00100010, '?');
			AddCharToDicts(ref upper, ref lower, 0x54744438, 0x00380474, '@');
			AddCharToDicts(ref upper, ref lower, 0x44442810, 0x0044447c, 'A');
			AddCharToDicts(ref upper, ref lower, 0x3c48483c, 0x003c4848, 'B');
			AddCharToDicts(ref upper, ref lower, 0x04044830, 0x00304804, 'C');
			AddCharToDicts(ref upper, ref lower, 0x4848281c, 0x001c2848, 'D');
			AddCharToDicts(ref upper, ref lower, 0x3c04047c, 0x007c0404, 'E');
			AddCharToDicts(ref upper, ref lower, 0x3c04047c, 0x00040404, 'F');
			AddCharToDicts(ref upper, ref lower, 0x74044438, 0x00384444, 'G');
			AddCharToDicts(ref upper, ref lower, 0x7c444444, 0x00444444, 'H');
			AddCharToDicts(ref upper, ref lower, 0x10101038, 0x00381010, 'I');
			AddCharToDicts(ref upper, ref lower, 0x20202070, 0x00182420, 'J');
			AddCharToDicts(ref upper, ref lower, 0x0c142444, 0x00442414, 'K');
			AddCharToDicts(ref upper, ref lower, 0x04040404, 0x007c0404, 'L');
			AddCharToDicts(ref upper, ref lower, 0x54546c44, 0x00444444, 'M');
			AddCharToDicts(ref upper, ref lower, 0x544c4c44, 0x00446464, 'N');
			AddCharToDicts(ref upper, ref lower, 0x44444438, 0x00384444, 'O');
			AddCharToDicts(ref upper, ref lower, 0x3c44443c, 0x00040404, 'P');
			AddCharToDicts(ref upper, ref lower, 0x44444438, 0x00582454, 'Q');
			AddCharToDicts(ref upper, ref lower, 0x3c44443c, 0x00442414, 'R');
			AddCharToDicts(ref upper, ref lower, 0x38044438, 0x00384440, 'S');
			AddCharToDicts(ref upper, ref lower, 0x1010107c, 0x00101010, 'T');
			AddCharToDicts(ref upper, ref lower, 0x44444444, 0x00384444, 'U');
			AddCharToDicts(ref upper, ref lower, 0x44444444, 0x00102828, 'V');
			AddCharToDicts(ref upper, ref lower, 0x54444444, 0x00446c54, 'W');
			AddCharToDicts(ref upper, ref lower, 0x10284444, 0x00444428, 'X');
			AddCharToDicts(ref upper, ref lower, 0x38444444, 0x00101010, 'Y');
			AddCharToDicts(ref upper, ref lower, 0x1020407c, 0x007c0408, 'Z');
			AddCharToDicts(ref upper, ref lower, 0x10101070, 0x00701010, '[');
			AddCharToDicts(ref upper, ref lower, 0x10080400, 0x00004020, '\\');
			AddCharToDicts(ref upper, ref lower, 0x1010101c, 0x001c1010, ']');
			AddCharToDicts(ref upper, ref lower, 0x00442810, 0x00000000, '^');
			AddCharToDicts(ref upper, ref lower, 0x00000000, 0x007c0000, '_');
			AddCharToDicts(ref upper, ref lower, 0x00100804, 0x00000000, '`');
			AddCharToDicts(ref upper, ref lower, 0x40380000, 0x00784478, 'a');
			AddCharToDicts(ref upper, ref lower, 0x4c340404, 0x00344c44, 'b');
			AddCharToDicts(ref upper, ref lower, 0x44380000, 0x00384404, 'c');
			AddCharToDicts(ref upper, ref lower, 0x64584040, 0x00586444, 'd');
			AddCharToDicts(ref upper, ref lower, 0x44380000, 0x0038047c, 'e');
			AddCharToDicts(ref upper, ref lower, 0x7c105020, 0x00101010, 'f');
			AddCharToDicts(ref upper, ref lower, 0x64580000, 0x38405864, 'g');
			AddCharToDicts(ref upper, ref lower, 0x4c340404, 0x00444444, 'h');
			AddCharToDicts(ref upper, ref lower, 0x10180010, 0x00381010, 'i');
			AddCharToDicts(ref upper, ref lower, 0x10180010, 0x0c121010, 'j');
			AddCharToDicts(ref upper, ref lower, 0x14240404, 0x0024140c, 'k');
			AddCharToDicts(ref upper, ref lower, 0x10101018, 0x00381010, 'l');
			AddCharToDicts(ref upper, ref lower, 0x542c0000, 0x00545454, 'm');
			AddCharToDicts(ref upper, ref lower, 0x4c340000, 0x00444444, 'n');
			AddCharToDicts(ref upper, ref lower, 0x44380000, 0x00384444, 'o');
			AddCharToDicts(ref upper, ref lower, 0x4c340000, 0x0404344c, 'p');
			AddCharToDicts(ref upper, ref lower, 0x64580000, 0x40405864, 'q');
			AddCharToDicts(ref upper, ref lower, 0x4c340000, 0x00040404, 'r');
			AddCharToDicts(ref upper, ref lower, 0x04780000, 0x003c403c, 's');
			AddCharToDicts(ref upper, ref lower, 0x083c0808, 0x00304808, 't');
			AddCharToDicts(ref upper, ref lower, 0x24240000, 0x00582424, 'u');
			AddCharToDicts(ref upper, ref lower, 0x44440000, 0x00102844, 'v');
			AddCharToDicts(ref upper, ref lower, 0x54440000, 0x00285454, 'w');
			AddCharToDicts(ref upper, ref lower, 0x28440000, 0x00442810, 'x');
			AddCharToDicts(ref upper, ref lower, 0x44440000, 0x38405864, 'y');
			AddCharToDicts(ref upper, ref lower, 0x207c0000, 0x007c0810, 'z');
			AddCharToDicts(ref upper, ref lower, 0x04080830, 0x00300808, '{');
			AddCharToDicts(ref upper, ref lower, 0x10101010, 0x00101010, '|');
			AddCharToDicts(ref upper, ref lower, 0x2010100c, 0x000c1010, '}');
			AddCharToDicts(ref upper, ref lower, 0x0000007c, 0x00000000, '~');
		}
		
		private static void AddCharToDicts(ref Dictionary<Char, UInt32> upper, ref Dictionary<Char, UInt32> lower, UInt32 upperData, UInt32 lowerData, Char c)
		{
			upper[c] = upperData;
			lower[c] = lowerData;
		}
		
		private static void DecodePixelData(ref Dictionary<Char, UInt32> upper, ref Dictionary<Char, UInt32> lower, ref Byte[] texturePixels, Char c, Int32 textureWidth)
		{
			Int32 charPstn = GetGlyphIndex(c);
			Int32 charTextureOffset = charPstn * FontWidth;
			Int32 halfway = FontWidth * FontHeight / 2;
			Boolean pixelIsLit;
			
			for (Int32 y = 0; y < FontHeight; y++)
			{
				Int32 textureRowPixelOffset = y * textureWidth;
				Int32 charRowPixelOffset = y * FontWidth;
				
				for (Int32 x = 0; x < FontWidth; x++)
				{
					Int32 charPixelIndex =  x + charRowPixelOffset;
					
					if(charPixelIndex < halfway)
						pixelIsLit = ((upper[c] & (1 << charPixelIndex)) != 0);
					else
						pixelIsLit = ((lower[c] & (1 << (charPixelIndex - halfway))) != 0);
					
					Int32 texturePixelIndex = textureRowPixelOffset + charTextureOffset + x;
					
					texturePixels[texturePixelIndex] = pixelIsLit ? PixelLit : PixelDark;
				}
			}
		}
		
		private static Int32 GetGlyphIndex(Char c)
		{
			return (Int32)c - NotPrintableChars;
		}
	}
}

