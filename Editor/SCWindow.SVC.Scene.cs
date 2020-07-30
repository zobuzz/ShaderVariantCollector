/// <summary>
/// Shader Control - (C) Copyright 2016-2020 Ramiro Oliva (Kronnect)
/// </summary>
/// 
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.SceneManagement;
using Unity.EditorCoroutines.Editor;

namespace ShaderControl {

    public partial class SCWindow : EditorWindow {

        //enum SortType {
        //    VariantsCount = 0,
        //    EnabledKeywordsCount = 1,
        //    ShaderFileName = 2,
        //    Keyword = 3,
        //    Material = 4
        //}

        //enum KeywordScopeFilter {
        //    Any = 0,
        //    GlobalKeywords = 1,
        //    LocalKeywords = 2
        //}

        //enum PragmaTypeFilter {
        //    Any = 0,
        //    MultiCompile = 1,
        //    ShaderFeature = 2
        //}

        //enum ModifiedStatus {
        //    Any = 0,
        //    OnlyModified = 1,
        //    NonModified = 2
        //}

        //SortType sortType = SortType.VariantsCount;
        //GUIStyle blackStyle, commentStyle, disabledStyle, foldoutBold, foldoutNormal, foldoutDim, foldoutRTF, titleStyle;
        //Vector2 scrollViewPosProject;
        //bool firstTime;
        //GUIContent matIcon, shaderIcon;
        //bool scanAllShaders;
        //string keywordFilter;
        //KeywordScopeFilter keywordScopeFilter;
        //PragmaTypeFilter pragmaTypeFilter;
        //ModifiedStatus modifiedStatus;
        //bool notIncludedInBuild;
        //string projectShaderNameFilter;

        List<Shader> ignoreShaders;

        internal static readonly string MAT_EXT = ".mat";

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
    }

}