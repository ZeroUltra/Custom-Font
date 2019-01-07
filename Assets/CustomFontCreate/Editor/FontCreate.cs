using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;
public class FontCreate : EditorWindow
{
    private string fontName = "";
    private TextAsset fontTextAsset;
    private Texture fontTexture;
    private Font myFont;
    private int row, column;
    private float width, height;

    private string savePath;
    [MenuItem("Window/Font Creator")]
    static void Init()
    {
        FontCreate window = (FontCreate)EditorWindow.GetWindow(typeof(FontCreate));
        window.Show();
        window.position = new Rect(100, 80, 500, 400);
        window.titleContent = new GUIContent("Font Creator");
    }
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUIStyle gUIStyle = new GUIStyle();
        gUIStyle.fontSize = 24;
        gUIStyle.fontStyle = FontStyle.Bold;
        gUIStyle.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("Font Creator", gUIStyle);

        GUILayout.Space(10);
        fontName = EditorGUILayout.TextField("Font Name", fontName);

        GUILayout.Space(10);
        fontTextAsset = (TextAsset)EditorGUILayout.ObjectField("Font TextAssets", fontTextAsset, typeof(TextAsset), false);

        GUILayout.Space(20);
        fontTexture = (Texture)EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture), false);

        EditorGUILayout.LabelField("The ranks of the picture font", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        row = EditorGUILayout.IntField("row", row);
        column = EditorGUILayout.IntField("column", column);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        EditorGUILayout.LabelField("The width and height of a single font", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        width = EditorGUILayout.FloatField("width", width);
        height = EditorGUILayout.FloatField("height", height);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.Label("Save Path", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(savePath, GUILayout.ExpandWidth(false));
        if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
        {
            savePath = EditorUtility.SaveFolderPanel("savePath", savePath, Application.dataPath);   
            savePath = savePath.Remove(0, Application.dataPath.Length - "Assets".Length);
        }
        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("Generate Font"))
        {
            if (fontName == "") { Debug.LogError("font name is null");return; }
            if (fontTextAsset == null) { Debug.LogError("fontTextAsset is null");return; }
            if (fontTexture == null) { Debug.LogError("fontTexture is null");return; }
            if (savePath == null) savePath = "Assets";
            GenerateFont();
        }
    }
    private void GenerateFont()
    {
        if (myFont == null)
        {
            myFont = new Font("myfont");
            Material mat = new Material(Shader.Find("GUI/Text Shader"));
            mat.SetTexture("_MainTex", fontTexture);
            AssetDatabase.CreateAsset(mat, savePath + "/" + fontName + ".mat");

            SerializedObject SO = new SerializedObject(myFont);
            SerializedProperty p = SO.FindProperty("m_LineSpacing");
            p.floatValue = height;
            SO.ApplyModifiedProperties();

            AssetDatabase.CreateAsset(myFont, savePath + "/" + fontName + ".fontsettings");
            myFont.material = mat;
            myFont.characterInfo = SetCharacterInfo();


            EditorUtility.SetDirty(myFont);
            EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Font Create Ok!!!", "font save as the:" + savePath, "ok");

        }
    }

    private CharacterInfo[] SetCharacterInfo()
    {
        string content = fontTextAsset.text;
        
        CharacterInfo[] info = new CharacterInfo[content.Length];
        for (int i = 0; i < content.Length; i++)
        {
            var bytes = Encoding.Unicode.GetBytes(content[i].ToString());
            var stringBuilder = new StringBuilder();
            for (var j = 0; j < bytes.Length; j += 2)
            {
                stringBuilder.AppendFormat("{0:x2}{1:x2}", bytes[j + 1], bytes[j]);
            }
            int index = Convert.ToInt32(stringBuilder.ToString(), 16);
            info[i].index = index;
            float x = (i % column) * (1f / column);
            float y = 1f - (i / column + 1) * (1f / row);

            #region MyRegion
            //info[i].uvBottomLeft = new Vector2(x, 1 - y);
            //info[i].uvBottomRight = new Vector2(y, -1f / row);
            //Debug.Log(string.Format("x:{0}_y:{1}_row:{2}_column:{3}", x, y, 1f / row, 1f / column));

            //info[i].minX = 0;   //vert X
            //info[i].maxX = (int)width;  //vert W
            //info[i].minY = (int)-height;  //vert H   Vert Y must be negative.
            //info[i].maxY = 0;      //vert Y   
            #endregion

            info[i].uv = new Rect(x, y, 1f / column, 1f / row);
            info[i].vert = new Rect(0, 0, width, -height);
            info[i].advance = (int)width;
        }
        return info;
    }

}
