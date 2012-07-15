/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using System.Collections.Generic;
using System.Diagnostics; // for [Conditional("DEBUG")]
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Sce.PlayStation.HighLevel.GameEngine2D
{
	/// <summary>
	/// The Plane3D node allows 3d orientations in the scenegraph, to some extent.
	/// That somewhat complicates the way we deal with touch coordinates, so if you have a
	/// Plane3D in your scene hierarchy, make sure you use Node.GetTouchPos() to get points
	/// in the Plane3D space (were all subnodes are), instead of using directly
	/// Director.Instance.CurrentScene.Camera.GetTouchPos().
	/// </summary>
	public class Plane3D
	: Node
	{
		/// <summary>
		/// The "transform matrix" for this plane. A plane is defined by a base point and a normal, but since we also use 
		/// it as a coordinate system (a matrix that we can push on the stack), we store it as a matrix.
		/// The 3D plane is defined as the 'identity' plane z=0 transformed by ModelMatrix, which means that ModelMatrix.ColumnZ is 
		/// the normal vector of the plane, and ModelMatrix.ColumnW is any point on the plane. 
		/// It is assumed that ModelMatrix is set to a right handled, orthonormal coordinate system (no check is performed).
		/// That means that ModelMatrix.ColumnX, ModelMatrix.ColumnY, ModelMatrix.ColumnZ are all perpendicular with each other, 
		/// each of unit length, and the cross product of ModelMatrix.ColumnX and ModelMatrix.ColumnY is in the same direction as
		/// ModelMatrix.ColumnZ. The default value for ModelMatrix is Matrix4.Identity (z=0 plane)
		/// This matrix is used as it is in the transform stack.
		/// <summary>
		public Matrix4 ModelMatrix = Matrix4.Identity;

		/// <summary>
		/// Plane3D constructor.
		/// Defaults to z=0 plane (identity).
		/// Please refer to ModelMatrix's comment for more details.
		/// </summary>
		public Plane3D()
		{
		}

		/// <summary>Plane3D constructor.</summary>
		/// <param name="modelmatrix">The value to set ModelMatrix to. 
		/// Please refer to ModelMatrix's comment for more details.
		/// </param>
		public Plane3D( Matrix4 modelmatrix )
		{
			ModelMatrix = modelmatrix;
		}

		public override void PushTransform()
		{
			Director.Instance.GL.ModelMatrix.Push();
			Director.Instance.GL.ModelMatrix.Mul1( ModelMatrix );
		}

		/// <summary>
		/// Note: FindParentPlane stops at the first encounterd Plane3D.
		/// We assume we can't have several Plane3D nodes along a tree branch.
		/// </summary>
		public override void FindParentPlane( ref Matrix4 mat )
		{
			mat = ModelMatrix;
		}
	}
}

