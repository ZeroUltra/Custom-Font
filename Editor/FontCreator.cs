#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text;
using System;
using System.Reflection;

/// <summary>
/// UNITY 2019 or new
/// </summary>
public class FontCreator : EditorWindow
{
    private string fontName = "";
    private TextAsset fontTextAsset;
    private Texture fontTexture;
    private Font myFont;
    private int row, column;
    private float width, height;

    private string savePath;

    private static MethodInfo loadIconMethodInfo, findTextureMethodInfo;

    private GUIStyle guiStyle;

    [MenuItem("Tools/Font Creator")]
    static void Init()
    {
        FontCreator window = GetWindow<FontCreator>("FontCreator");
        window.position = new Rect(500, 600, 600, 400);

        //UNITY 2019 or new
        window.titleContent = new GUIContent("Font Creator", EditorGUIUtility.IconContent("Font Icon").image);

        //UNITY 2018 ....
        //loadIconMethodInfo = typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic);
        //findTextureMethodInfo = typeof(EditorGUIUtility).GetMethod("FindTexture", BindingFlags.Static | BindingFlags.Public);
        //window.titleContent = EditorGUIUtility.TrTextContentWithIcon("Font Creator", loadIconMethodInfo.Invoke(null, new object[] { "Font Icon" }) as Texture);

        window.Show();
    }
    private void OnEnable()
    {
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 24;
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.normal.textColor = Color.cyan;
    }
    void OnGUI()
    {
        //EditorGUILayout.LabelField("Font Creator", gUIStyle);
        GUILayout.Label("Font Creator", guiStyle);


        GUILayout.Space(10);
        fontName = EditorGUILayout.TextField("Font Name", fontName);

        GUILayout.Space(10);
        fontTextAsset = (TextAsset)EditorGUILayout.ObjectField("Font TextAssets", fontTextAsset, typeof(TextAsset), false);

        GUILayout.Space(20);
        fontTexture = (Texture)EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture), false);

        #region The ranks of the picture font
        EditorGUILayout.LabelField("The ranks of the picture font", EditorStyles.boldLabel);
        using (var h = new EditorGUILayout.HorizontalScope())
        {
            //row = EditorGUILayout.IntField("row", row);
            //column = EditorGUILayout.IntField("column", column);
            GUILayout.Label("row", GUILayout.Width(80f));
            row = EditorGUILayout.IntField(row, GUILayout.Width(120f));
            GUILayout.Space(10);
            GUILayout.Label("column", GUILayout.Width(80f));
            column = EditorGUILayout.IntField(column, GUILayout.Width(120f));
        }
        #endregion

        GUILayout.Space(5);

        #region The width and height of a single font
        EditorGUILayout.LabelField("The width and height of a single font", EditorStyles.boldLabel);
        using (var h = new EditorGUILayout.HorizontalScope())
        {
            //width = EditorGUILayout.FloatField("width", width);  //间隔太大
            //EditorGUILayout.PrefixLabel();  //  shit
            GUILayout.Label("width", GUILayout.Width(80f));
            width = EditorGUILayout.FloatField(width, GUILayout.Width(120f));
            GUILayout.Space(10);
            GUILayout.Label("height", GUILayout.Width(80f));
            height = EditorGUILayout.FloatField(height, GUILayout.Width(120f));
        }
        #endregion

        GUILayout.Space(10);
        GUILayout.Label("Save Path", EditorStyles.boldLabel);
        using (var h = new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.TextField(savePath, GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
            {
                savePath = EditorUtility.SaveFolderPanel("savePath", savePath, Application.dataPath);
                savePath = savePath.Remove(0, Application.dataPath.Length - "Assets".Length);
            }
        }
        if (GUILayout.Button("Generate Font"))
        {
            if (fontName == "") { Debug.LogError("font name is null"); return; }
            if (fontTextAsset == null) { Debug.LogError("fontTextAsset is null"); return; }
            if (fontTexture == null) { Debug.LogError("fontTexture is null"); return; }
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
            EditorUtility.DisplayDialog("Font Created Successfully!!!", "Save as the:" + savePath + "/" + fontName, "Ok");

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

            #region Old
            // info[i].uv = new Rect(x, y, 1f / column, 1f / row);
            //info[i].vert = new Rect(0, 0, width, -height);
            #endregion

            #region New
            info[i].uvBottomLeft = new Vector2(x, y);
            info[i].uvBottomRight = new Vector2(x + 1f / column, y);
            info[i].uvTopLeft = new Vector2(x, y + 1f / row);
            info[i].uvTopRight = new Vector2(1f / column + x, y + 1f / row);

            info[i].minX = 0;
            info[i].minY = (int)-height;
            info[i].maxX = (int)width;
            info[i].maxY = 0;

            #endregion
            info[i].advance = (int)width;
        }
        return info;
    }

}
#endif
