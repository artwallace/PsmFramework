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
	/// TextureInfo holds a Texture2D object and caches associated tile UV data. 
	/// The source region for tiling is not necesseraly the entire texture, it 
	/// can be any oriented box in UV domain. 
	/// TextureInfo takes ownership of the Texture2D object passed to it, and 
	/// disposes of it in its Dispose function.
	/// </summary>
	public class TextureInfo : System.IDisposable
	{
		/// <summary>Cached UV information for each tile.</summary>
		public class CachedTileData
		{
			/// <summary>Bottom left point UV.</summary>
			public Vector2 UV_00;
			/// <summary>Bottom right point UV.</summary>
			public Vector2 UV_10;
			/// <summary>Top left point UV.</summary>
			public Vector2 UV_01;
			/// <summary>Top right point UV.</summary>
			public Vector2 UV_11;
		}
		/// <summary>The texture object.</summary>
		public Texture2D Texture;
		/// <summary>Return texture size in pixels as a Vector2.</summary>
		public Vector2 TextureSizef { get { return new Vector2( Texture.Width, Texture.Height ); } } 
		/// <summary>Return texture size in pixels as a Vector2i.</summary>
		public Vector2i TextureSizei { get { return new Vector2i( Texture.Width, Texture.Height ); } } 
		/// <summary>Return tile size in uv units.</summary>
		public Vector2 TileSizeInUV;
		/// <summary>Return tile size in pixels as a Vector2. All tiles have the same size.</summary>
		public Vector2 TileSizeInPixelsf { get { Common.Assert( Texture != null ); return TextureSizef * TileSizeInUV; } }
		/// <summary>Return the dimensions of the tiles grid.</summary>
		public Vector2i NumTiles; // default (1,1) for no tiles

		bool m_disposed = false;
		/// <summary>Return true if this object been disposed.</summary>
		public bool Disposed { get { return m_disposed; } }

		CachedTileData[] m_tiles_uvs; // cache tiles uvs

		/// <summary>Return the CachedTileData (which contains tile UV information) for a given tile.</summary>
		public CachedTileData GetCachedTiledData( ref Vector2i tile_index ) 
		{ 
//			Common.Assert( 0 <= tile_index.X && tile_index.X < NumTiles.X );
//			Common.Assert( 0 <= tile_index.Y && tile_index.Y < NumTiles.Y );
			return m_tiles_uvs[ tile_index.X + NumTiles.X * tile_index.Y ];
		}

		/// <summary>TextureInfo constructor.</summary>
		public TextureInfo()
		{
		}

		/// <summary>TextureInfo constructor.</summary>
		public TextureInfo( string filename )
		{
			Initialize( new Texture2D( filename, false ), Math._11i, TRS.Quad0_1 );
		}

		/// <summary>
		/// TextureInfo constructor.
		/// Note: TextureInfo takes ownership of the Texture2D passed to this constructor, and disposes of it in Dispose.
		/// </summary>
		public TextureInfo( Texture2D texture )
		{
			Initialize( texture, Math._11i, TRS.Quad0_1 );
		}

		/// <summary>
		/// TextureInfo constructor.
		/// Note: TextureInfo takes ownership of the Texture2D passed to this constructor, and disposes of it in Dispose.
		/// </summary>
		/// <param name="texture">The source texture.</param>
		/// <param name="num_tiles">The number of tile subdivisions on x and y.</param>
		public TextureInfo( Texture2D texture, Vector2i num_tiles )
		{
			Initialize( texture, num_tiles, TRS.Quad0_1 );
		}

		/// <summary>
		/// TextureInfo constructor.
		/// Note: TextureInfo takes ownership of the Texture2D passed to this constructor, and disposes of it in Dispose.
		/// </summary>
		/// <param name="texture">The source texture.</param>
		/// <param name="num_tiles">The number of tile subdivisions on x and y.</param>
		/// <param name="source_area">The source rectangle, in UV domain, on which we are going to build the tiles (bottom left is 0,0).</param>
		public TextureInfo( Texture2D texture, Vector2i num_tiles, TRS source_area )
		{
			Initialize( texture, num_tiles, source_area );
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
				//System.Console.WriteLine( "TextureInfo Dispose() called!" );
				Common.DisposeAndNullify< Texture2D >( ref Texture );
				m_disposed = true;
			}
		}

		/// <summary>
		/// The actual init function called by TextureInfo constructors.
		/// </summary>
		/// <param name="texture">The source texture.</param>
		/// <param name="num_tiles">The number of tiles/cells, horitonally and vertically.</param>
		/// <param name="source_area">The source rectangle, in UV domain, on which we are going to build the tiles (bottom left is 0,0).</param>
		public void Initialize( Texture2D texture, Vector2i num_tiles, TRS source_area )
		{
			Texture = texture;
			TileSizeInUV = source_area.S / num_tiles.Vector2(); 
			NumTiles = num_tiles;
			m_tiles_uvs = new CachedTileData[ num_tiles.Product() ];
			
			for ( int y=0; y < NumTiles.Y; ++y )
			{
				for ( int x=0; x < NumTiles.X; ++x )
				{
					Vector2i tile_index = new Vector2i( x, y );
					TRS tile = TRS.Tile( NumTiles, tile_index, source_area ); // lots of calculation duplicated, but this is an init function
					
					int index = tile_index.X + tile_index.Y * NumTiles.X;
					
					m_tiles_uvs[ index ] = new CachedTileData()
					{
						UV_00 = tile.Point00 ,
						UV_10 = tile.Point10 ,
						UV_01 = tile.Point01 ,
						UV_11 = tile.Point11 
					};
				}
			}
		}
	}
}

