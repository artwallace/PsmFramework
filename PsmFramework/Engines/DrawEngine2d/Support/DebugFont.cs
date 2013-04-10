using System;
using System.Collections.Generic;
using PsmFramework.Engines.DrawEngine2d.Textures;
using Sce.PlayStation.Core.Graphics;

namespace PsmFramework.Engines.DrawEngine2d.Support
{
	internal sealed class DebugFont : IDisposablePlus
	{
		#region Constructor, Dispose
		
		internal DebugFont(DrawEngine2d drawEngine2d)
		{
			Initialize(drawEngine2d);
		}
		
		public void Dispose()
		{
			if(IsDisposed)
				return;
			
			Cleanup();
			IsDisposed = true;
		}
		
		public Boolean IsDisposed { get; private set; }
		
		#endregion
		
		#region Initialize, Cleanup
		
		private void Initialize(DrawEngine2d drawEngine2d)
		{
			InitializeDrawEngine2d(drawEngine2d);
			InitializeHardcodedFontInfo();
			InitializeCharacterData();
			InitializeTexture();
		}
		
		private void Cleanup()
		{
			CleanupTexture();
			CleanupCharacterData();
			CleanupHardcodedFontInfo();
			CleanupDrawEngine2d();
		}
		
		#endregion
		
		#region DrawEngine
		
		private void InitializeDrawEngine2d(DrawEngine2d drawEngine2d)
		{
			DrawEngine2d = drawEngine2d;
		}
		
		private void CleanupDrawEngine2d()
		{
			DrawEngine2d = null;
		}
		
		internal DrawEngine2d DrawEngine2d;
		
		#endregion
		
		#region Hardcoded Font Info
		
		private void InitializeHardcodedFontInfo()
		{
		}
		
		private void CleanupHardcodedFontInfo()
		{
		}
		
		//Determines the width of the texture by number of chars wide.
		private const Int32 MaxTextureCharCapacity = 128;
		
		//These values are tied directly to the hardcoded glyph data and should not be altered.
		public const Int32 FontWidth = 8;
		public const Int32 FontHeight = 8;
		
		private Int32 TextureWidth = MaxTextureCharCapacity * FontWidth;
		private Int32 TextureHeight = FontHeight;
		
		private const Byte PixelDark = (Byte)0x00;
		private const Byte PixelLit = (Byte)0xff;
		
		#endregion
		
		#region Character Data
		
		private void InitializeCharacterData()
		{
			UpperCharData = new Dictionary<Char, UInt32>();
			LowerCharData = new Dictionary<Char, UInt32>();
			
			CharGlyphMap = new Dictionary<Char, Int32>();
			CharGlyphIndex = 0;
			
			PopulateCharacterData();
		}
		
		private void CleanupCharacterData()
		{
			CharGlyphMap.Clear();
			CharGlyphMap = null;
			
			CharGlyphIndex = 0;
			
			UpperCharData.Clear();
			LowerCharData.Clear();
			
			UpperCharData = null;
			LowerCharData = null;
		}
		
		//These two should be in sync at all times.
		//They should be unified by combining the upper and lower data
		// into a single value and kept as a single dict.
		private Dictionary<Char, UInt32> UpperCharData;
		private Dictionary<Char, UInt32> LowerCharData;
		
		private Dictionary<Char, Int32> CharGlyphMap;
		private Int32 CharGlyphIndex;
		
		private void PopulateCharacterData()
		{
			AddCharacterData('0', 0x54644438, 0x0038444c);
			AddCharacterData('1', 0x10141810, 0x007c1010);
			AddCharacterData('2', 0x20404438, 0x007c0418);
			AddCharacterData('3', 0x30404438, 0x00384440);
			AddCharacterData('4', 0x24283020, 0x0020207c);
			AddCharacterData('5', 0x403c047c, 0x00384440);
			AddCharacterData('6', 0x3c040830, 0x00384444);
			AddCharacterData('7', 0x1020447c, 0x00101010);
			AddCharacterData('8', 0x38444438, 0x00384444);
			AddCharacterData('9', 0x78444438, 0x00182040);
			
			AddCharacterData('A', 0x44442810, 0x0044447c);
			AddCharacterData('B', 0x3c48483c, 0x003c4848);
			AddCharacterData('C', 0x04044830, 0x00304804);
			AddCharacterData('D', 0x4848281c, 0x001c2848);
			AddCharacterData('E', 0x3c04047c, 0x007c0404);
			AddCharacterData('F', 0x3c04047c, 0x00040404);
			AddCharacterData('G', 0x74044438, 0x00384444);
			AddCharacterData('H', 0x7c444444, 0x00444444);
			AddCharacterData('I', 0x10101038, 0x00381010);
			AddCharacterData('J', 0x20202070, 0x00182420);
			AddCharacterData('K', 0x0c142444, 0x00442414);
			AddCharacterData('L', 0x04040404, 0x007c0404);
			AddCharacterData('M', 0x54546c44, 0x00444444);
			AddCharacterData('N', 0x544c4c44, 0x00446464);
			AddCharacterData('O', 0x44444438, 0x00384444);
			AddCharacterData('P', 0x3c44443c, 0x00040404);
			AddCharacterData('Q', 0x44444438, 0x00582454);
			AddCharacterData('R', 0x3c44443c, 0x00442414);
			AddCharacterData('S', 0x38044438, 0x00384440);
			AddCharacterData('T', 0x1010107c, 0x00101010);
			AddCharacterData('U', 0x44444444, 0x00384444);
			AddCharacterData('V', 0x44444444, 0x00102828);
			AddCharacterData('W', 0x54444444, 0x00446c54);
			AddCharacterData('X', 0x10284444, 0x00444428);
			AddCharacterData('Y', 0x38444444, 0x00101010);
			AddCharacterData('Z', 0x1020407c, 0x007c0408);
			
			AddCharacterData('a', 0x40380000, 0x00784478);
			AddCharacterData('b', 0x4c340404, 0x00344c44);
			AddCharacterData('c', 0x44380000, 0x00384404);
			AddCharacterData('d', 0x64584040, 0x00586444);
			AddCharacterData('e', 0x44380000, 0x0038047c);
			AddCharacterData('f', 0x7c105020, 0x00101010);
			AddCharacterData('g', 0x64580000, 0x38405864);
			AddCharacterData('h', 0x4c340404, 0x00444444);
			AddCharacterData('i', 0x10180010, 0x00381010);
			AddCharacterData('j', 0x10180010, 0x0c121010);
			AddCharacterData('k', 0x14240404, 0x0024140c);
			AddCharacterData('l', 0x10101018, 0x00381010);
			AddCharacterData('m', 0x542c0000, 0x00545454);
			AddCharacterData('n', 0x4c340000, 0x00444444);
			AddCharacterData('o', 0x44380000, 0x00384444);
			AddCharacterData('p', 0x4c340000, 0x0404344c);
			AddCharacterData('q', 0x64580000, 0x40405864);
			AddCharacterData('r', 0x4c340000, 0x00040404);
			AddCharacterData('s', 0x04780000, 0x003c403c);
			AddCharacterData('t', 0x083c0808, 0x00304808);
			AddCharacterData('u', 0x24240000, 0x00582424);
			AddCharacterData('v', 0x44440000, 0x00102844);
			AddCharacterData('w', 0x54440000, 0x00285454);
			AddCharacterData('x', 0x28440000, 0x00442810);
			AddCharacterData('y', 0x44440000, 0x38405864);
			AddCharacterData('z', 0x207c0000, 0x007c0810);
			
			AddCharacterData(' ', 0x00000000, 0x00000000);
			AddCharacterData('.', 0x00000000, 0x00181800);
			AddCharacterData('?', 0x20404438, 0x00100010);
			AddCharacterData('!', 0x10101010, 0x00100000);
			AddCharacterData(',', 0x00000000, 0x08101000);
			AddCharacterData(';', 0x00100000, 0x08101000);
			AddCharacterData(':', 0x00100000, 0x00001000);
			
			AddCharacterData('(', 0x10102040, 0x00402010);
			AddCharacterData(')', 0x10100804, 0x00040810);
			AddCharacterData(']', 0x1010101c, 0x001c1010);
			AddCharacterData('[', 0x10101070, 0x00701010);
			AddCharacterData('{', 0x04080830, 0x00300808);
			AddCharacterData('}', 0x2010100c, 0x000c1010);
			AddCharacterData('<', 0x0c183060, 0x00603018);
			AddCharacterData('>', 0x6030180c, 0x000c1830);
			
			AddCharacterData('\'', 0x00102040, 0x00000000);
			AddCharacterData('"', 0x00282828, 0x00000000);
			AddCharacterData('`', 0x00100804, 0x00000000);
			
			AddCharacterData('+', 0x7c101000, 0x00001010);
			AddCharacterData('-', 0x7c000000, 0x00000000);
			AddCharacterData('*', 0x10385410, 0x00105438);
			AddCharacterData('/', 0x10204000, 0x00000408);
			AddCharacterData('\\', 0x10080400, 0x00004020);
			AddCharacterData('=', 0x007c0000, 0x0000007c);
			
			AddCharacterData('#', 0x287c2828, 0x0028287c);
			AddCharacterData('$', 0x38147810, 0x00103c50);
			AddCharacterData('%', 0x10204c0c, 0x00606408);
			AddCharacterData('&', 0x08141408, 0x00582454);
			AddCharacterData('@', 0x54744438, 0x00380474);
			AddCharacterData('^', 0x00442810, 0x00000000);
			AddCharacterData('_', 0x00000000, 0x007c0000);
			AddCharacterData('|', 0x10101010, 0x00101010);
			AddCharacterData('~', 0x0000007c, 0x00000000);
		}
		
		private void AddCharacterData(Char c, UInt32 upperData, UInt32 lowerData)
		{
			CharGlyphMap[c] = CharGlyphIndex;
			CharGlyphIndex++;//for clarity
			
			UpperCharData[c] = upperData;
			LowerCharData[c] = lowerData;
		}
		
		private Int32 GetGlyphIndex(Char c)
		{
			return CharGlyphMap[c];
		}
		
		public Boolean ContainsCharacterGlyph(Char c)
		{
			return CharGlyphMap.ContainsKey(c);
		}
		
		#endregion
		
		#region Texture
		
		private void InitializeTexture()
		{
			TexturePixels = new Byte[TextureWidth * TextureHeight];
			
			PopulatePixelData();
			BuildTexture();
			
			TexturePixels = null;
		}
		
		private void CleanupTexture()
		{
			TextureColumnIndex.Dispose();
			TextureColumnIndex = null;
			
			TiledFontTexture.Dispose();
			TiledFontTexture = null;
		}
		
		internal const String TextureKey = "DebugFont";
		
		private Byte[] TexturePixels;
		
		private TiledTexture TiledFontTexture;
		
		private ColumnIndex TextureColumnIndex;
		
		private void PopulatePixelData()
		{
			foreach(Char c in CharGlyphMap.Keys)
				DecodePixelData(c);
		}
		
		private void DecodePixelData(Char c)
		{
			Int32 charPstn = GetGlyphIndex(c);
			Int32 charTextureOffset = charPstn * FontWidth;
			Int32 halfway = FontWidth * FontHeight / 2;
			Boolean pixelIsLit;
			
			for (Int32 y = 0; y < FontHeight; y++)
			{
				Int32 textureRowPixelOffset = y * TextureWidth;
				Int32 charRowPixelOffset = y * FontWidth;
				
				for (Int32 x = 0; x < FontWidth; x++)
				{
					Int32 charPixelIndex =  x + charRowPixelOffset;
					
					if(charPixelIndex < halfway)
						pixelIsLit = ((UpperCharData[c] & (1 << charPixelIndex)) != 0);
					else
						pixelIsLit = ((LowerCharData[c] & (1 << (charPixelIndex - halfway))) != 0);
					
					Int32 texturePixelIndex = textureRowPixelOffset + charTextureOffset + x;
					
					TexturePixels[texturePixelIndex] = pixelIsLit ? PixelLit : PixelDark;
				}
			}
		}
		
		private void BuildTexture()
		{
			Texture2dPlus FontTexture = DrawEngine2d.CreateTexture(TextureKey, TextureWidth, TextureHeight, PixelFormat.Luminance, TextureCachePolicy.KeepAlways);
			FontTexture.SetPixels(0, TexturePixels, PixelFormat.Luminance);
			FontTexture.SetFilter(TextureFilterMode.Nearest, TextureFilterMode.Nearest, TextureFilterMode.Nearest);
			
			TiledFontTexture = DrawEngine2d.CreateTiledTexture(TextureKey, FontTexture, TextureCachePolicy.KeepAlways);
			TextureColumnIndex = TiledFontTexture.CreateColumnIndex(MaxTextureCharCapacity);
			
			FontTexture = null;
		}
		
		public Single[] GetCharTextureCoordinates(Char c)
		{
			Int32 index = GetGlyphIndex(c);
			return TextureColumnIndex.GetKey(index).TextureCoordinates;
		}
		
		#endregion
	}
}

