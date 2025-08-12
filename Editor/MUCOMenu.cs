using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using System.IO;
using System.Linq;
using System.Collections;

namespace Muco
{
    public class MUCOMenu : EditorWindow
    {
        [MenuItem("MUCO/Menu")]
        public static void ShowWindow()
        {
            GetWindow<MUCOMenu>("MUCO Menu");
        }

        

        private void OnGUI()
        {

            GUILayout.Label("Recommended Project Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUIStyle styleRed = new GUIStyle(GUI.skin.button);
            styleRed.normal.textColor = Color.red;

            GUIStyle styleGreen = new GUIStyle(GUI.skin.button);
            styleGreen.normal.textColor = Color.green;

            var lineHeight = 19;
            
            var biggerLineHeight = new GUILayoutOption[] { GUILayout.Height(lineHeight) };
            var labelStyleNextToButton = new GUIStyle(GUI.skin.label);

            // Existing settings
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Allow 'unsafe' Code: " + (PlayerSettings.allowUnsafeCode ? "Enabled" : "Disabled"));
            GUILayout.Label("Color Space: " + PlayerSettings.colorSpace);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            if (PlayerSettings.allowUnsafeCode)
                GUILayout.Label("OK", styleGreen);
            else
            {
                if (GUILayout.Button("Enable 'unsafe' Code"))
                {
                    PlayerSettings.allowUnsafeCode = true;
                }
            }

            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                GUILayout.Label("OK", styleGreen);
            else
            {
                if (GUILayout.Button("Set Color Space to Linear"))
                {
                    PlayerSettings.colorSpace = ColorSpace.Linear;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            // New section for build settings
            // Set Build Target to Android
            GUILayout.Label("Recommended Build Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Build Target: " + EditorUserBuildSettings.activeBuildTarget, labelStyleNextToButton, biggerLineHeight);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                GUILayout.Label("OK", styleGreen);
            }
            else
            {
                if (GUILayout.Button("Set Build Target to Android"))
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // Texture Compression Format: ASTC (only if Android)
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                string compression = EditorUserBuildSettings.androidBuildSubtarget.ToString();
                GUILayout.Label("Texture Compression Format: " + compression, labelStyleNextToButton, biggerLineHeight);
            }
            else
            {
                GUILayout.Label("Texture Compression Format: Android Only", styleRed, biggerLineHeight);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                if (EditorUserBuildSettings.androidBuildSubtarget == MobileTextureSubtarget.ASTC)
                {
                    GUILayout.Label("OK", styleGreen);
                }
                else
                {
                    if (GUILayout.Button("Set to ASTC"))
                    {
                        SetAndroidTextureCompressionToASTC();
                    }
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("Set to ASTC");
                GUI.enabled = true;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();



            GUILayout.Space(20);

            // Required Packages section
            GUILayout.Label("Required Packages", EditorStyles.boldLabel);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("com.unity.addressables@1.21.19", labelStyleNextToButton, biggerLineHeight);
            GUILayout.Label("com.antilatency.alt-tracking-xr@1.0.1", labelStyleNextToButton, biggerLineHeight);
            GUILayout.Label("com.antilatency.sdk@4.5.0", labelStyleNextToButton, biggerLineHeight);
            GUILayout.Label("com.unity.xr.openxr@1.8.2", labelStyleNextToButton, biggerLineHeight);
            GUILayout.Label("com.unity.xr.management@4.4.0", labelStyleNextToButton, biggerLineHeight);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (IsPackageInstalled("com.unity.addressables"))
            {
                GUILayout.Label("OK", styleGreen);
            }
            else
            {
                if (GUILayout.Button("Install Package"))
                {
                    AddPackage("com.unity.addressables@1.21.19");
                }
            }

            if (IsPackageInstalled("com.antilatency.alt-tracking-xr"))
            {
                GUILayout.Label("OK", styleGreen);
            }
            else
            {
                if (GUILayout.Button("Install Package"))
                {
                    AddPackage("com.antilatency.alt-tracking-xr@1.0.1");
                }
            }

            if (IsPackageInstalled("com.antilatency.sdk"))
            {
                GUILayout.Label("OK", styleGreen);
            }
            else
            {
                if (GUILayout.Button("Install Package"))
                {
                    AddPackage("com.antilatency.sdk@4.5.0");
                }
            }

            if (IsPackageInstalled("com.unity.xr.openxr"))
            {
                GUILayout.Label("OK", styleGreen);
            }
            else
            {
                if (GUILayout.Button("Install Package"))
                {
                    AddPackage("com.unity.xr.openxr@1.8.2");
                }
            }

            if (IsPackageInstalled("com.unity.xr.management"))
            {
                GUILayout.Label("OK", styleGreen);
            }
            else
            {
                if (GUILayout.Button("Install Package"))
                {
                    AddPackage("com.unity.xr.management@4.4.0");
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public static bool IsPackageInstalled(string packageId)
        {
            if (!File.Exists("Packages/manifest.json"))
                return false;
            string jsonText = File.ReadAllText("Packages/manifest.json");
            return jsonText.Contains(packageId);
        }

        static AddRequest Request;
        static void AddPackage(string name)
        {
            Request = Client.Add(name);
        }
        
        private static void SetAndroidTextureCompressionToASTC()
        {
            if (EditorUserBuildSettings.androidBuildSubtarget != MobileTextureSubtarget.ASTC)
            {
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;
                Debug.Log("Android texture compression format set to ASTC.");
            }
            else
            {
                Debug.Log("Android texture compression format is already set to ASTC.");
            }
        }

    }
}
