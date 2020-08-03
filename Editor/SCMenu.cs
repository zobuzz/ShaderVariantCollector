using UnityEngine;
using UnityEditor;
using System;

namespace ShaderControl
{
	public class SCMenu : Editor
	{
		[MenuItem ("Window/Manage Shaders...", false, 200)]
		static void ManageShaders (MenuCommand command)
		{
			SCWindow.ShowWindow();
		}

	}
}
