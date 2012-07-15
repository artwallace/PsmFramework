/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// Given a Font object and a text containing all the characters you intend to use, 
	/// FontMap creates a Texture2D object containg the characters and its corresponding 
	/// table of UVs. This data is used by GameEngine2D in various text rendering functions.
	///
	/// Examples:
	/// 
	/// new FontMap( new Font( "D:\\Blah\\YourFont.TTF", 32, FontStyle.Bold ), 512 );
	/// 
	/// new FontMap( new Font( FontAlias.System, 32, FontStyle.Bold ), 512 );
	/// </summary>
	public class FontMap : System.IDisposable
	{
		/// <summary>The UV data for a single character.</summary>
		public struct CharData
		{
			/// <summary>UV in FontMap's Texture.</summary>
			public Bounds2 UV; 
			/// <summary>The pixel size for this character (depends on the font.)</summary>
			public Vector2 PixelSize; 
		};
		/// <summary>The font texture containing all the characters.</summary>
		public Texture2D Texture;
		/// <summary>Map characters to their corresponding CharData (UV and size data).</summary>
		public Dictionary< char, CharData > CharSet; 
		/// <summary>Character height in pixels - all characters have the same pixel height.</summary>
		public float CharPixelHeight;
		/// <summary>The ascii character set as a string.</summary>
		static public string AsciiCharSet = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		CharData[] m_ascii_char_data; // shortcut in CharSet, for readable ASCII characters
		bool[] m_ascii_char_data_valid;

		/// <summary>
		/// Create a FontMap for the ASCII char set.
		/// </summary>
		/// <param name="font">The font to use to render characters. Note that FontMap disposes of this Font object.</param>
		/// <param name="fontmap_width">The internal width used by the texture. Height is adjusted automatically so that all characters fit.</param>
		public FontMap( Font font, int fontmap_width = 512 )
		{
			Initialize( font, AsciiCharSet, fontmap_width );
		}

		/// <summary>
		/// </summary>
		/// <param name="font">The font to use to render characters. Note that FontMap disposes of this Font object.</param>
		/// <param name="charset">A string containing all the characters you will ever need when drawing text with this FontMap.</param>
		/// <param name="fontmap_width">The internal with used by the texture (height is adjusted automatically).</param>
		public FontMap( Font font, string charset, int fontmap_width = 512 )
		{
			Initialize( font, charset, fontmap_width );
		}

		/// <summary>
		/// Dispose implementation.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				Common.DisposeAndNullify< Texture2D >( ref Texture );
				m_disposed = true;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="font">The font to use to render characters. Note that FontMap disposes of this Font object.</param>
		/// <param name="charset">A string containing all the characters you will ever need when drawing text with this FontMap.</param>
		/// <param name="fontmap_width">The internal with used by the texture (height is adjusted automatically).</param>
		public void Initialize( Font font, string charset, int fontmap_width = 512 )
		{
			CharSet = new Dictionary< char, CharData >();

			CharPixelHeight = font.Metrics.Height;

			Image image = null;
			Vector2i totalsize = new Vector2i( 0, 0 );

			for ( int k=0; k < 2; ++k )
			{
				Vector2i turtle = new Vector2i( 0, 0 );	// turtle is in Sce.PlayStation.Core.Imaging.Font's coordinate system
				int max_height = 0;

				for ( int i=0; i < charset.Length; ++i )
				{
					if ( CharSet.ContainsKey( charset[i] ) )
						continue; // this character is already in the map

					Vector2i char_size = new Vector2i(
						font.GetTextWidth( charset[i].ToString(), 0, 1 ),
						font.Metrics.Height
						);

					max_height = Common.Max( max_height, char_size.Y );

					if ( turtle.X + char_size.X > fontmap_width )
					{
						// hit the right side, go to next line
						turtle.X = 0;
						turtle.Y += max_height;	// Sce.PlayStation.Core.Imaging.Font's coordinate system: top is 0, so we += to move down
						max_height = 0;

						// make sure we are noit going to newline forever due to lack of fontmap_width
						Common.Assert( char_size.Y <= fontmap_width ); 
					}

					if ( k > 0 )
					{
						// that starts from top left
						image.DrawText( charset[i].ToString(), new ImageColor(255,255,255,255), font
										, new ImagePosition( turtle.X, turtle.Y ) );

						var uv = new Bounds2( turtle.Vector2() / totalsize.Vector2()
											 , ( turtle + char_size ).Vector2() / totalsize.Vector2() );

						// now fix the UV to be in GameEngine2D's UV coordinate system, where 0,0 is bottom left
						uv = uv.OutrageousYVCoordFlip().OutrageousYTopBottomSwap();

						CharSet.Add( charset[i], new CharData(){ UV = uv, PixelSize = char_size.Vector2()} );
					}

					turtle.X += char_size.X;

					if ( k == 0 )
					{
						totalsize.X = Common.Max( totalsize.X, turtle.X );
						totalsize.Y = Common.Max( totalsize.Y, turtle.Y + max_height );
					}
				}

				if ( k == 0 )
				{
//					System.Console.WriteLine( "FontMap.Initialize: totalsize " + totalsize );
					image = new Image( ImageMode.A, new ImageSize( totalsize.X, totalsize.Y ), new ImageColor(0,0,0,0) );

					CharSet.Clear(); // we want to go through the same add logic on second pass, so clear
				}
			}

			Texture = new Texture2D( image.Size.Width, image.Size.Height, false, PixelFormat.Luminance );
			Texture.SetPixels( 0, image.ToBuffer() );
//			image.Export("uh?","hey.png");
			image.Dispose();

			{
				// cache ascii entries so we can skip TryGetValue logic for those
				m_ascii_char_data = new CharData[ AsciiCharSet.Length ];
				m_ascii_char_data_valid = new bool[ AsciiCharSet.Length ];
				for ( int i=0; i < AsciiCharSet.Length; ++i )
				{
					CharData cdata;
					m_ascii_char_data_valid[i] = CharSet.TryGetValue( AsciiCharSet[i], out cdata );
					m_ascii_char_data[i] = cdata;
				}
			}

			// dispose of the font by default
			font.Dispose();
		}

		/// <summary>
		/// Try to get the CharData needed to draw the character 'c'.
		/// </summary>
		public bool TryGetCharData( char c, out CharData cdata ) 
		{
			{
				int index = (int)c-(int)' ';
				if ( index >= 0 && index < AsciiCharSet.Length )
				{
					cdata = m_ascii_char_data[index];
					return m_ascii_char_data_valid[index];
				}
			}

			if ( !CharSet.TryGetValue( c, out cdata ) )
			{
				System.Console.WriteLine( "The character [" + c + "] is not present in the FontMap you are trying to use. Please double check the input character set." );
				return false;
			}

			return true;
		}
	}
}

