using UnityEngine;
using UnityEditor;
using System;

namespace ShaderControl
{
	public class SCMenu : Editor
	{
		[MenuItem ("Window/Browse Shaders...", false, 200)]
		static void BrowseShaders (MenuCommand command)
		{
			SCWindow.ShowWindow();
		}

	}
}
