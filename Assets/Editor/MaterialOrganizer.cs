using UnityEditor;
using UnityEngine;
using System.IO;

public class MaterialOrganizer : EditorWindow
{
    [MenuItem("Tools/Organize Materials and Textures")]
    public static void ShowWindow()
    {
        GetWindow<MaterialOrganizer>("Material Organizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Organize Materials and Their Textures", EditorStyles.boldLabel);

        if (GUILayout.Button("Organize Materials"))
        {
            OrganizeMaterials();
            EditorUtility.DisplayDialog("Material Organization", "Materials and textures have been organized successfully!", "OK");
        }
    }

    private static void OrganizeMaterials()
    {
        string rootFolder = "Assets/OrganizedMaterials";
        CreateFolder(rootFolder);

        // Find all materials in the project
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        foreach (string guid in materialGuids)
        {
            string materialPath = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

            if (material != null)
            {
                string materialName = Path.GetFileNameWithoutExtension(materialPath);
                string materialFolder = $"{rootFolder}/{materialName}";
                CreateFolder(materialFolder);

                // Move the material to its folder
                string newMaterialPath = $"{materialFolder}/{materialName}.mat";
                AssetDatabase.MoveAsset(materialPath, newMaterialPath);

                // Find and move associated textures
                foreach (var texturePropertyName in material.GetTexturePropertyNames())
                {
                    Texture texture = material.GetTexture(texturePropertyName);
                    if (texture != null)
                    {
                        string texturePath = AssetDatabase.GetAssetPath(texture);
                        string textureName = Path.GetFileName(texturePath);
                        string newTexturePath = $"{materialFolder}/{textureName}";

                        if (!string.IsNullOrEmpty(texturePath))
                        {
                            AssetDatabase.MoveAsset(texturePath, newTexturePath);
                        }
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }

    private static void CreateFolder(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            string parentFolder = Path.GetDirectoryName(folderPath).Replace("\\", "/");
            string newFolderName = Path.GetFileName(folderPath);
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }
    }
}