BASIC LOGIC
1. Set up the range circle by defining sight field by the angle and radius (set up layer for target and obstacles)
2. Obtain all targets in the range by overlapShpere function 
3. Using RAYCAST fillter out the target that is in the range but behind the obstacles 
3.1 Mean while setup Unityeditor SC for direct GIZMO line in editor for debugging. 
4. All function is work now for visualisation (including using customised mesh in unity)
5. Shoot bunch of rays in the sight angle from player (Emittor), calculate each angle dir that is produced by all these rays
6. Create a struct for storing many different variables
7. use these data collect from each ray to generate customised mesh

IN ENGINE SET-UP
1. Create two Layer TARGETs and OBSTACLES
2. Create viewGameobject with Component MeshFilter and MeshRenderer for customised MESH (Display visual Field of sight)
3. FOW contains all the functions FOW Editor is for DRAW GIZMO in the editor
