/* SCE CONFIDENTIAL
 * PlayStation(R)Suite SDK 0.98.2
 * Copyright (C) 2012 Sony Computer Entertainment Inc.
 * All Rights Reserved.
 */

using Sce.PlayStation.Core;

namespace Sce.PlayStation.HighLevel.GameEngine2D.Base
{
/// <summary>
/// A common interface for Camera2D and Camera3D.
/// </summary>
public interface ICamera
{
	/// <summary>
	/// Read aspect ratio from viewport and update camera projection data accordingly.
	/// </summary>
	void SetAspectFromViewport();
	/// <summary>
	/// Push all matrices on the stack, and set Projection and View.
	/// </summary>
	void Push();
	/// <summary>
	/// Pop all matrices from the stack.
	/// </summary>
	void Pop();
	/// <summary>
	/// Return the camera transform matrix (orthonormal positioning matrix), as a Matrix4.
	/// GetTransform().InverseOrthonormal() is what you push on the view matrix stack.
	/// </summary>
	Matrix4 GetTransform();
	/// <summary>
	/// Draw a world grid and the world coordinate system, for debug.
	/// Note that DebugDraw() doesn't call Push()/Pop() internally. It is your responsability to call it between this Camera's Push()/Pop().
	/// </summary>
	void DebugDraw( float step );
	/// <summary>
	/// Process input for debug navigation.
	/// </summary>
	void Navigate( int control );
	/// <summary>
	/// Set a camera view so that the bottom left of the screen matches world point (0,0) and 
	/// the top right of the screen matches world point (screen width, sreen height).
	/// </summary>
	void SetViewFromViewport();
	/// <summary>
	/// Given a point in normalized screen coordinates (-1->1), return its corresponding world position.
	/// </summary>
	Vector2 NormalizedToWorld( Vector2 bottom_left_minus_1_minus_1_top_left_1_1_normalized_screen_pos );
	/// <summary>
	/// Return the 'nth' touch position in world coordinates.
	/// The 'prev' flag is for internal use only.
	/// </summary>
	Vector2 GetTouchPos( int nth = 0, bool prev = false );
	/// <summary>
	/// Calculate the world bounds currently visible on screen.
	/// This function is 2D only, somehow extended to 3D.
	/// </summary>
	Bounds2 CalcBounds();
	/// <summary>
	/// Based on current viewport size, get the size of a "screen pixel" in world coordinates.
	/// Can be used to determine scale factor needed to draw sprites 1:1 for example.
	/// 2D only, somehow extended to 3D.
	/// </summary>
	float GetPixelSize();
	/// <summary>
	/// The the orientation of the 3D plane that should be used by GetTouchPos(). 
	/// 3D only.
	/// </summary>
	void SetTouchPlaneMatrix( Matrix4 mat );
}

}

