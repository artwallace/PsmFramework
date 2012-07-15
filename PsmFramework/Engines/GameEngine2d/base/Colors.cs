/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics; // BlendMode
using Sce.PlayStation.Core.Imaging; // Font

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
	/// <summary>
	/// Some named colored constants.
	/// </summary>
	public static class Colors
	{
		/// <summary>0,0,0,1</summary>
		public static Vector4 Black = new Vector4(0,0,0,1);
		/// <summary>1,0,0,1</summary>
		public static Vector4 Red = new Vector4(1,0,0,1);
		/// <summary>0,1,0,1</summary>
		public static Vector4 Green = new Vector4(0,1,0,1);
		/// <summary>1,1,0,1</summary>
		public static Vector4 Yellow = new Vector4(1,1,0,1);
		/// <summary>0,0,1,1</summary>
		public static Vector4 Blue = new Vector4(0,0,1,1);
		/// <summary>1,0,1,1</summary>
		public static Vector4 Magenta = new Vector4(1,0,1,1);
		/// <summary>0,1,1,1</summary>
		public static Vector4 Cyan = new Vector4(0,1,1,1);
		/// <summary>1,1,1,1</summary>
		public static Vector4 White = new Vector4(1,1,1,1);

		/// <summary>0.5,1,0,1</summary>
		public static Vector4 Lime = new Vector4(0.5f,1.0f,0.0f,1.0f);
		/// <summary>0,0.5,1,1</summary>
		public static Vector4 LightBlue = new Vector4(0.0f,0.5f,1.0f,1.0f);
		/// <summary>1,0,0.5,1</summary>
		public static Vector4 Pink = new Vector4(1.0f,0.0f,0.5f,1.0f);
		/// <summary>1,0.5,0,1</summary>
		public static Vector4 Orange = new Vector4(1.0f,0.5f,0.0f,1.0f);
		/// <summary>0,1,0.5,1</summary>
		public static Vector4 LightCyan = new Vector4(0.0f,1.0f,0.5f,1.0f);
		/// <summary>0.5,0,1,1</summary>
		public static Vector4 Purple = new Vector4(0.5f,0.0f,1.0f,1.0f);

		/// <summary>0.05,0.05,0.05,1</summary>
		public static Vector4 Grey05 = new Vector4(0.05f,0.05f,0.05f,1.0f);
		/// <summary>0.1,0.1,0.1,1</summary>
		public static Vector4 Grey10 = new Vector4(0.1f,0.1f,0.1f,1.0f);
		/// <summary>0.2,0.2,0.2,1</summary>
		public static Vector4 Grey20 = new Vector4(0.2f,0.2f,0.2f,1.0f);
		/// <summary>0.3,0.3,0.3,1</summary>
		public static Vector4 Grey30 = new Vector4(0.3f,0.3f,0.3f,1.0f);
		/// <summary>0.4,0.4,0.4,1</summary>
		public static Vector4 Grey40 = new Vector4(0.4f,0.4f,0.4f,1.0f);
		/// <summary>0.5,0.5,0.5,1</summary>
		public static Vector4 Grey50 = new Vector4(0.5f,0.5f,0.5f,1.0f);
		/// <summary>0.6,0.6,0.6,1</summary>
		public static Vector4 Grey60 = new Vector4(0.6f,0.6f,0.6f,1.0f);
		/// <summary>0.7,0.7,0.7,1</summary>
		public static Vector4 Grey70 = new Vector4(0.7f,0.7f,0.7f,1.0f);
		/// <summary>0.8,0.8,0.8,1</summary>
		public static Vector4 Grey80 = new Vector4(0.8f,0.8f,0.8f,1.0f);
		/// <summary>0.9,0.9,0.9,1</summary>
		public static Vector4 Grey90 = new Vector4(0.9f,0.9f,0.9f,1.0f);
	}
}
