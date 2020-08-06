using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderControl
{
    [Serializable, ExecuteInEditMode]
    public class SCSVCSettings : ScriptableObject
    {
        public List<int> usage = new List<int>();
        public List<ShaderVariantCollection> collection = new List<ShaderVariantCollection>();
    }
}