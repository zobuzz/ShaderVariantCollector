/// <summary>
/// Shader Control - (C) Copyright 2016-2020 Ramiro Oliva (Kronnect)
/// </summary>
/// 
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.SceneManagement;
using Unity.EditorCoroutines.Editor;

namespace ShaderControl {

    public partial class SCWindow : EditorWindow {


        internal static readonly string MAT_EXT = ".mat";

        SCEnvSettings envSettingsInfo;

        public void Init()
        {
            if(null == envSettingsInfo)
            {
                string filename = GetSettingsStorePath();
                envSettingsInfo = AssetDatabase.LoadAssetAtPath<SCEnvSettings>(filename);
            }
        }

        public static SCEnvSettings CheckEnvSettingsStore(SCEnvSettings envsettings)
        {
            if (envsettings == null)
            {
                string filename = GetSettingsStorePath();
                envsettings = AssetDatabase.LoadAssetAtPath<SCEnvSettings>(filename);
                if (envsettings != null)
                {
                    return envsettings;
                }
            }
            // Check if scriptable object exists
            string path = GetSettingsStorePath();
            if (!File.Exists(path))
            {
                string dir = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dir);
                envsettings = ScriptableObject.CreateInstance<SCEnvSettings>();
                AssetDatabase.CreateAsset(envsettings, path);
                AssetDatabase.SaveAssets();
            }
            return envsettings;
        }

        static string GetSettingsStorePath()
        {
            // Locate shader control path
            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int k = 0; k < paths.Length; k++)
            {
                if (paths[k].EndsWith("/ShaderControl/Editor", StringComparison.InvariantCultureIgnoreCase))
                {
                    return paths[k] + "/Resources/SCEnvSettings.asset";
                }
            }
            return null;
        }


        void DrawPrafabScanUI()
        {
            envSettingsInfo = CheckEnvSettingsStore(envSettingsInfo);
            if (null == envSettingsInfo)
                return;
            if (null == envSettingsInfo.m_prefabFolders)
                envSettingsInfo.m_prefabFolders = new List<string>();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Set Prefa Scan Folders:");
            }
            EditorGUILayout.EndHorizontal();

            for (int i=0; i<envSettingsInfo.m_prefabFolders.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.TextField(string.Format("Scan Folder {0}:", (i + 1)), envSettingsInfo.m_prefabFolders[i]);
                    if (GUILayout.Button("¡ª", GUILayout.Width(30)))
                    {
                        //delete in loop
                        envSettingsInfo.m_prefabFolders.RemoveAt(i);
                        i--;
                    }
                    if (GUILayout.Button("Setting", GUILayout.Width(80)))
                    {
                        envSettingsInfo.m_prefabFolders[i] = EditorUtility.OpenFolderPanel("prefab folder", "", "");
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(envSettingsInfo);
                    AssetDatabase.SaveAssets();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginVertical(blackStyle);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                {
                    if (GUILayout.Button(new GUIContent("Add Prefab Scan Folder")))
                    {
                        envSettingsInfo.m_prefabFolders.Add("");
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(envSettingsInfo);
                    AssetDatabase.SaveAssets();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        IEnumerator LoadAllEditorBuildScenes()
        {
            //close shader async compile 
            ShaderUtil.allowAsyncCompilation = false;

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                if (!scenes[i].enabled)
                    continue;
                Debug.Log(string.Format("Start Collect Material in EditorBuildSettings.scenes index:{0} name:{1}", i, scenes[i].path));

                float ratio = ((float)(i + 1)) / (float)scenes.Length;
                EditorUtility.DisplayProgressBar("Scene SVC Collect", scenes[i].path, ratio);

                EditorSceneManager.OpenScene(scenes[i].path);

                yield return new EditorWaitForSeconds(5.0f);
            }

            //enable shader async compile 
            ShaderUtil.allowAsyncCompilation = true;

            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayDialog("Finished", string.Format("Currently tracked: {0} shaders {1} total variants",
                SVCTool.ShaderUtils.GetCurrentShaderVariantCollectionShaderCount(), SVCTool.ShaderUtils.GetCurrentShaderVariantCollectionVariantCount()), "ok");

            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            yield return null;
        }


        void ClearCurrentSVC()
        {
            //switch to a EmptyScene scene
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SVCTool.ShaderUtils.ClearCurrentShaderVariantCollection();
        }

        void LoadAllScenesCollectMaterial()
        {
            //save a backup copy
            SVCTool.ShaderUtils.SaveCurrentShaderVariantCollection("Assets/globalsvc_backup.shadervariants");

            SVCTool.ShaderUtils.ClearCurrentShaderVariantCollection();

            if (EditorApplication.isPlaying)
                EditorApplication.ExitPlaymode();

            EditorCoroutineUtility.StartCoroutine(LoadAllEditorBuildScenes(), this);
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

    }
}