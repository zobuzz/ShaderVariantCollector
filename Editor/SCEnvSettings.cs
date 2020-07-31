using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderControl
{
    [Serializable, ExecuteInEditMode]
    public class SCEnvSettings : ScriptableObject
    {
        public List<string> m_prefabFolders;

        public List<Shader> m_ignoreShaders;
    }
}