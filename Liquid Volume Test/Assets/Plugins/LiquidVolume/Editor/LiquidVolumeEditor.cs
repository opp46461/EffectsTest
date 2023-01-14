using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace LiquidVolumeFX
{
    [CustomEditor(typeof(LiquidVolume)), CanEditMultipleObjects]
    public class LiquidVolumeEditor : Editor
    {

        static GUIStyle titleLabelStyle, sectionHeaderStyle;
        static Color titleColor;
        static bool[] expandSection = new bool[6];
        const string SECTION_PREFS = "LiquidVolumeExpandSection";
        static string[] sectionNames = new string[] {
                                                "Liquid Settings",
                                                "Foam Settings",
                                                "Smoke Settings",
                                                "Flask Settings",
                                                "Physics",
                                                "Advanced"
                                };
        const int LIQUID_SETTINGS = 0;
        const int FOAM_SETTINGS = 1;
        const int SMOKE_SETTINGS = 2;
        const int FLASK_SETTINGS = 3;
        const int PHYSICS_SETTINGS = 4;
        const int ADVANCED_SETTINGS = 5;
        SerializedProperty topology, detail, depthAware, depthAwareCustomPass, ignoreGravity, reactToForces, doubleSidedBias, rotationLevelBias;
        SerializedProperty level, liquidColor1, liquidColor2, liquidScale1, liquidScale2, alpha, emissionColor, emissionBrightness;
        SerializedProperty ditherShadows, murkiness, turbulence1, turbulence2, frecuency, speed;
        SerializedProperty sparklingIntensity, sparklingAmount, deepObscurance;
        SerializedProperty foamColor, foamScale, foamThickness, foamDensity, foamWeight, foamVisibleFromBottom, foamTurbulence;
        SerializedProperty smokeEnabled, smokeColor, smokeScale, smokeBaseObscurance, smokeSpeed;
        SerializedProperty flaskTint, flaskThickness, flaskGlossinessExternal, flaskGlossinessInternal, refractionBlur, blurIntensity;
        SerializedProperty scatteringEnabled, scatteringPower, scatteringAmount;
        SerializedProperty liquidRaySteps, foamRaySteps, smokeRaySteps, extentsScale, upperLimit, noiseVariation, allowViewFromInside;
        SerializedProperty bumpMap, bumpDistortionScale, bumpDistortionOffset, distortionMap, texture, textureAlpha, textureScale, textureOffset;
        SerializedProperty distortionAmount, renderQueue;
        SerializedProperty reflectionTexture;
        SerializedProperty physicsMass, physicsAngularDamp;
        SerializedProperty debugSpillPoint;
        SerializedProperty fixedSpillPoint;
        SerializedProperty limitLevel;
        MeshRenderer mr;

        void OnEnable()
        {
            titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
            for (int k = 0; k < expandSection.Length; k++)
            {
                expandSection[k] = EditorPrefs.GetBool(SECTION_PREFS + k, false);
            }
            topology = serializedObject.FindProperty("_topology");
            detail = serializedObject.FindProperty("_detail");
            depthAware = serializedObject.FindProperty("_depthAware");
            depthAwareCustomPass = serializedObject.FindProperty("_depthAwareCustomPass");

            level = serializedObject.FindProperty("_level");
            liquidColor1 = serializedObject.FindProperty("_liquidColor1");
            liquidColor2 = serializedObject.FindProperty("_liquidColor2");
            liquidScale1 = serializedObject.FindProperty("_liquidScale1");
            liquidScale2 = serializedObject.FindProperty("_liquidScale2");
            alpha = serializedObject.FindProperty("_alpha");
            emissionColor = serializedObject.FindProperty("_emissionColor");
            emissionBrightness = serializedObject.FindProperty("_emissionBrightness");
            ditherShadows = serializedObject.FindProperty("_ditherShadows");
            murkiness = serializedObject.FindProperty("_murkiness");
            turbulence1 = serializedObject.FindProperty("_turbulence1");
            turbulence2 = serializedObject.FindProperty("_turbulence2");
            frecuency = serializedObject.FindProperty("_frecuency");
            speed = serializedObject.FindProperty("_speed");
            sparklingIntensity = serializedObject.FindProperty("_sparklingIntensity");
            sparklingAmount = serializedObject.FindProperty("_sparklingAmount");
            deepObscurance = serializedObject.FindProperty("_deepObscurance");
            scatteringEnabled = serializedObject.FindProperty("_scatteringEnabled");
            scatteringPower = serializedObject.FindProperty("_scatteringPower");
            scatteringAmount = serializedObject.FindProperty("_scatteringAmount");

            foamColor = serializedObject.FindProperty("_foamColor");
            foamScale = serializedObject.FindProperty("_foamScale");
            foamThickness = serializedObject.FindProperty("_foamThickness");
            foamDensity = serializedObject.FindProperty("_foamDensity");
            foamWeight = serializedObject.FindProperty("_foamWeight");
            foamTurbulence = serializedObject.FindProperty("_foamTurbulence");
            foamVisibleFromBottom = serializedObject.FindProperty("_foamVisibleFromBottom");

            smokeEnabled = serializedObject.FindProperty("_smokeEnabled");
            smokeColor = serializedObject.FindProperty("_smokeColor");
            smokeScale = serializedObject.FindProperty("_smokeScale");
            smokeBaseObscurance = serializedObject.FindProperty("_smokeBaseObscurance");
            smokeSpeed = serializedObject.FindProperty("_smokeSpeed");

            flaskTint = serializedObject.FindProperty("_flaskTint");
            flaskThickness = serializedObject.FindProperty("_flaskThickness");
            flaskGlossinessExternal = serializedObject.FindProperty("_flaskGlossinessExternal");
            flaskGlossinessInternal = serializedObject.FindProperty("_flaskGlossinessInternal");
            refractionBlur = serializedObject.FindProperty("_refractionBlur");
            blurIntensity = serializedObject.FindProperty("_blurIntensity");

            liquidRaySteps = serializedObject.FindProperty("_liquidRaySteps");
            foamRaySteps = serializedObject.FindProperty("_foamRaySteps");
            smokeRaySteps = serializedObject.FindProperty("_smokeRaySteps");
            extentsScale = serializedObject.FindProperty("_extentsScale");
            upperLimit = serializedObject.FindProperty("_upperLimit");
            noiseVariation = serializedObject.FindProperty("_noiseVariation");
            allowViewFromInside = serializedObject.FindProperty("_allowViewFromInside");
            renderQueue = serializedObject.FindProperty("_renderQueue");

            bumpMap = serializedObject.FindProperty("_bumpMap");
            bumpDistortionScale = serializedObject.FindProperty("_bumpDistortionScale");
            bumpDistortionOffset = serializedObject.FindProperty("_bumpDistortionOffset");
            distortionMap = serializedObject.FindProperty("_distortionMap");
            distortionAmount = serializedObject.FindProperty("_distortionAmount");
            texture = serializedObject.FindProperty("_texture");
            textureAlpha = serializedObject.FindProperty("_textureAlpha");
            textureScale = serializedObject.FindProperty("_textureScale");
            textureOffset = serializedObject.FindProperty("_textureOffset");

            reflectionTexture = serializedObject.FindProperty("_reflectionTexture");
            reactToForces = serializedObject.FindProperty("_reactToForces");
            ignoreGravity = serializedObject.FindProperty("_ignoreGravity");
            physicsMass = serializedObject.FindProperty("_physicsMass");
            physicsAngularDamp = serializedObject.FindProperty("_physicsAngularDamp");

            debugSpillPoint = serializedObject.FindProperty("_debugSpillPoint");
            fixedSpillPoint = serializedObject.FindProperty("_fixedSpillPoint");
            limitLevel = serializedObject.FindProperty("_limitLevel");
            doubleSidedBias = serializedObject.FindProperty("_doubleSidedBias");
            rotationLevelBias = serializedObject.FindProperty("_rotationLevelBias");
        }

        void OnDestroy()
        {
            // Save folding sections state
            for (int k = 0; k < expandSection.Length; k++)
            {
                EditorPrefs.SetBool(SECTION_PREFS + k, expandSection[k]);
            }
        }

        public override void OnInspectorGUI()
        {
#if UNITY_5_6_OR_NEWER
            serializedObject.UpdateIfRequiredOrScript();
#else
												serializedObject.UpdateIfDirtyOrScript ();
#endif

            if (sectionHeaderStyle == null)
            {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldout);
            }
            sectionHeaderStyle.normal.textColor = titleColor;
            sectionHeaderStyle.margin = new RectOffset(12, 0, 0, 0);
            sectionHeaderStyle.fontStyle = FontStyle.Bold;

            if (titleLabelStyle == null)
            {
                titleLabelStyle = new GUIStyle(EditorStyles.label);
            }
            titleLabelStyle.normal.textColor = titleColor;
            titleLabelStyle.fontStyle = FontStyle.Bold;


            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("通用设置", titleLabelStyle);
            if (GUILayout.Button("帮助", GUILayout.Width(40)))
            {
                if (!EditorUtility.DisplayDialog("Liquid Volume", "To learn more about a property in this inspector move the mouse over the label for a quick description (tooltip).\n\nPlease check README file in the root of the asset for details and contact support.\n\nIf you like Liquid Volume, please rate it on the Asset Store. For feedback and suggestions visit our support forum on kronnect.com.", "Close", "Visit Support Forum"))
                {
                    Application.OpenURL("http://kronnect.com/taptapgo");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(topology, new GUIContent("网络布局类型", "体积形状."));
            EditorGUILayout.PropertyField(detail, new GUIContent("细节", "液体效果的细节量。“Simple”设置不使用3D纹理，这使得它与移动设备兼容."));
            EditorGUILayout.PropertyField(depthAware, new GUIContent("深度", "在液体体积内启用z-testing。如果体积中含有液体以外的其他物体，则使用，否则不要启用。液体体积内的2D对象需要使用一个不透明的切割着色器写入z-buffer(标准着色器切割模式是一个很好的选择)."));
            if (target != null)
            {
                LiquidVolume lv = (LiquidVolume)target;
                if (lv.transform.parent == null)
                    GUI.enabled = false;
            }
            EditorGUILayout.PropertyField(depthAwareCustomPass, new GUIContent("父物体", "使用父几何图形作为当前液体的边界。仅当液体体积位于另一个物体内部时，才在不规则拓扑结构下使用，并防止透过容器看到背景中的其他液体。如果看不到任何伪影，请不要启用此选项，因为它会为液体容器添加额外的渲染通道."));
            GUI.enabled = true;

            EditorGUILayout.PropertyField(alpha, new GUIContent("总体透明度", "液体、烟雾和泡沫的全球透明度。您还可以将其与液体，烟雾和泡沫颜色的阿尔法相结合."));

            int detailed = detail.intValue;

            if (detailed != (int)DETAIL.Smoke)
            {
                EditorGUILayout.Separator();
                expandSection[LIQUID_SETTINGS] = EditorGUILayout.Foldout(expandSection[LIQUID_SETTINGS], "液体的设置", sectionHeaderStyle);

                if (expandSection[LIQUID_SETTINGS])
                {
                    EditorGUILayout.PropertyField(level, new GUIContent("水平", "体积的填充水平."));

                    EditorGUILayout.PropertyField(liquidColor1, new GUIContent("液体颜色1"));
                    if (detailed >= 10)
                    {
                        EditorGUILayout.PropertyField(liquidScale1, new GUIContent("比例1", "应用于液体第一纹理的刻度."));
                        EditorGUILayout.PropertyField(liquidColor2, new GUIContent("液体颜色2"));
                        EditorGUILayout.PropertyField(liquidScale2, new GUIContent("比例2", "Scale applied to the 2nd texture of the liquid."));
                        EditorGUILayout.PropertyField(murkiness, new GUIContent("表面朦胧", "液体的纯度。0 = 晶莹剔透，1 = 充满泥浆/污垢."));
                    }

                    EditorGUILayout.PropertyField(emissionColor, new GUIContent("自发光颜色"));
                    EditorGUILayout.PropertyField(emissionBrightness, new GUIContent("自发光亮度"));
                    EditorGUILayout.PropertyField(ditherShadows, new GUIContent("抖动阴影", "启用此选项可对液体阴影应用抖动，模拟部分透明的阴影。为了获得最佳效果，可在质量设置中启用柔和阴影."));
                    EditorGUILayout.PropertyField(turbulence1, new GUIContent("低振幅湍流", "低振幅湍流."));
                    EditorGUILayout.PropertyField(turbulence2, new GUIContent("高振幅湍流", "高振幅湍流."));
                    EditorGUILayout.PropertyField(frecuency, new GUIContent("频率", "湍流的频繁性。增加以产生更短的波浪."));
                    EditorGUILayout.PropertyField(speed, new GUIContent("速度", "湍流动画的速度."));

                    if (detailed >= 10)
                    {
                        EditorGUILayout.PropertyField(sparklingIntensity, new GUIContent("闪闪发光的强度", "闪光粒子的亮度."));
                        EditorGUILayout.PropertyField(sparklingAmount, new GUIContent("起泡量", "闪闪发光/闪光颗粒的数量."));
                    }

                    EditorGUILayout.PropertyField(deepObscurance, new GUIContent("深度遮挡", "使液体的底部变暗."));
                    EditorGUILayout.PropertyField(scatteringEnabled, new GUIContent("光散射", "使背光穿过液体产生光扩散效果."));
                    if (scatteringEnabled.boolValue)
                    {
                        EditorGUILayout.PropertyField(scatteringPower, new GUIContent("   力度", "光散射方程的功率（指数）."));
                        EditorGUILayout.PropertyField(scatteringAmount, new GUIContent("   量", "光散射效应的最终乘法器或衰减."));
                    }

                    if (detailed == 0)
                    {
                        EditorGUILayout.PropertyField(foamVisibleFromBottom, new GUIContent("从底部可见", "如果从底部观察容器时，通过液体可以看到泡沫."));
                    }

                }

                if (detailed >= 10)
                {
                    EditorGUILayout.Separator();
                    expandSection[FOAM_SETTINGS] = EditorGUILayout.Foldout(expandSection[FOAM_SETTINGS], "泡沫的设置", sectionHeaderStyle);

                    if (expandSection[FOAM_SETTINGS])
                    {
                        EditorGUILayout.PropertyField(foamColor, new GUIContent("颜色"));
                        EditorGUILayout.PropertyField(foamScale, new GUIContent("比例", "应用于泡沫纹理的刻度."));
                        EditorGUILayout.PropertyField(foamThickness, new GUIContent("厚度"));
                        EditorGUILayout.PropertyField(foamDensity, new GUIContent("密度"));
                        EditorGUILayout.PropertyField(foamWeight, new GUIContent("重量", "值越大，泡沫与液体的底线密度越大."));
                        EditorGUILayout.PropertyField(foamTurbulence, new GUIContent("湍流", "影响泡沫的液体湍流的乘数。将其设置为零以产生静电泡沫."));
                        EditorGUILayout.PropertyField(foamVisibleFromBottom, new GUIContent("从底部可见", "如果从底部观察容器时，通过液体可以看到泡沫."));
                    }
                }
            }

            EditorGUILayout.Separator();
            expandSection[SMOKE_SETTINGS] = EditorGUILayout.Foldout(expandSection[SMOKE_SETTINGS], "烟雾的设置", sectionHeaderStyle);

            if (expandSection[SMOKE_SETTINGS])
            {
                EditorGUILayout.PropertyField(smokeEnabled, new GUIContent("可见"));
                if (smokeEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(smokeColor, new GUIContent("颜色"));
                    if (detailed >= 10)
                    {
                        EditorGUILayout.PropertyField(smokeScale, new GUIContent("比例", "应用于用于烟雾的纹理的刻度."));
                        EditorGUILayout.PropertyField(smokeSpeed, new GUIContent("速度"));
                    }
                    EditorGUILayout.PropertyField(smokeBaseObscurance, new GUIContent("基础遮挡", "使底部的烟雾变暗."));
                }
            }

            if (detailed != (int)DETAIL.DefaultNoFlask && detailed != (int)DETAIL.Smoke)
            {
                EditorGUILayout.Separator();
                expandSection[FLASK_SETTINGS] = EditorGUILayout.Foldout(expandSection[FLASK_SETTINGS], "瓶子的设置", sectionHeaderStyle);

                if (expandSection[FLASK_SETTINGS])
                {
                    EditorGUILayout.PropertyField(flaskTint, new GUIContent("色调", "应用于晶体的色调颜色."));
                    EditorGUILayout.PropertyField(flaskThickness, new GUIContent("厚度", "水晶思维."));
                    EditorGUILayout.PropertyField(flaskGlossinessExternal, new GUIContent("光泽度 外在", "晶体外表面的光泽度."));
                    if (detailed != 30)
                    {
                        EditorGUILayout.PropertyField(flaskGlossinessInternal, new GUIContent("光泽度 内部", "晶体内表面的光泽度."));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(reflectionTexture, new GUIContent("反射", "为反射效果指定立方体贴图纹理."));
                        EditorGUILayout.PropertyField(textureAlpha, new GUIContent("透明度"));
                    }
                    if (detailed == 20)
                    {
                        EditorGUILayout.PropertyField(texture, new GUIContent("纹理", "为液体容器指定纹理."));
                        EditorGUILayout.PropertyField(textureAlpha, new GUIContent("   透明度"));
                        EditorGUILayout.PropertyField(textureScale, new GUIContent("   比例"));
                        EditorGUILayout.PropertyField(textureOffset, new GUIContent("   偏移量"));
                        EditorGUILayout.PropertyField(bumpMap, new GUIContent("法线贴图", "为液体容器指定法线贴图."));
                    }
                    EditorGUILayout.PropertyField(refractionBlur, new GUIContent("反射模糊", "通过瓶子可见的模糊背景."));
                    if (refractionBlur.boolValue)
                    {
                        EditorGUILayout.PropertyField(blurIntensity, new GUIContent("   强度"));
                        EditorGUILayout.PropertyField(distortionMap, new GUIContent("   失真图", "在此插槽中为晶体失真指定置换图."));
                        EditorGUILayout.PropertyField(distortionAmount, new GUIContent("   失真量"));
                    }

                    EditorGUILayout.PropertyField(bumpDistortionScale, new GUIContent("凹凸/失真比例", "凹凸和失真贴图纹理的纹理比例."));
                    EditorGUILayout.PropertyField(bumpDistortionOffset, new GUIContent("凹凸/失真偏移", "凹凸和失真贴图纹理的纹理偏移."));

                }
            }

            EditorGUILayout.Separator();
            expandSection[PHYSICS_SETTINGS] = EditorGUILayout.Foldout(expandSection[PHYSICS_SETTINGS], "物理设置", sectionHeaderStyle);
            if (expandSection[PHYSICS_SETTINGS])
            {
                EditorGUILayout.PropertyField(reactToForces, new GUIContent("对力做出反应", "启用后，液体将在烧瓶内移动，试图反射外力."));
                GUI.enabled = reactToForces.boolValue;
                EditorGUILayout.PropertyField(physicsMass, new GUIContent("质量", "更大的质量将使液体更加静态."));
                EditorGUILayout.PropertyField(physicsAngularDamp, new GUIContent("角度潮湿", "液体与烧瓶的摩擦量，它决定了液体在施加力后返回正常位置的速度."));
                GUI.enabled = !reactToForces.boolValue;
                EditorGUILayout.PropertyField(ignoreGravity, new GUIContent("忽略重力", "启用后，液体将随瓶子旋转。默认情况下为 false，这意味着液体将停留在烧瓶的底部."));
                GUI.enabled = true;
            }

            EditorGUILayout.Separator();
            expandSection[ADVANCED_SETTINGS] = EditorGUILayout.Foldout(expandSection[ADVANCED_SETTINGS], "高级设置", sectionHeaderStyle);

            if (expandSection[ADVANCED_SETTINGS])
            {
                EditorGUILayout.PropertyField(smokeRaySteps, new GUIContent("烟雾射线", "用于构建烟色的每个像素的样本数."));
                if (detailed != (int)DETAIL.Smoke)
                {
                    EditorGUILayout.PropertyField(liquidRaySteps, new GUIContent("液体射线", "用于构建液体颜色的每个像素的样本数."));
                }
                if (detailed >= 1)
                {
                    if (detailed != (int)DETAIL.Smoke)
                    {
                        EditorGUILayout.PropertyField(foamRaySteps, new GUIContent("泡沫射线", "用于构建泡沫颜色的每个像素的样本数."));
                    }
                    EditorGUILayout.PropertyField(noiseVariation, new GUIContent("噪点变化", "在 3 种不同的 3D 纹理之间进行选择."));
                }
                EditorGUILayout.PropertyField(upperLimit, new GUIContent("上限", "液体、泡沫和烟雾相对于瓶子尺寸的上限."));
                EditorGUILayout.PropertyField(extentsScale, new GUIContent("范围缩放", "可选和附加乘数应用于网格的当前大小。用于在需要此功能的特定型号上调整电平."));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("烘焙当前Transform"))
                {
                    if (EditorUtility.DisplayDialog("烘焙当前Transform", "当前变换（旋转和缩放）将转移到网格本身。是否要继续？", "好的", "取消"))
                    {
                        foreach (LiquidVolume lv in targets)
                        {
                            BakeRotation(lv);
                        }
                    }
                }
                if (GUILayout.Button("中心锚点"))
                {
                    if (EditorUtility.DisplayDialog("中心锚点", "顶点将被移位，因此枢轴将重新定位在其中心。是否要继续？", "好的", "取消"))
                    {
                        foreach (LiquidVolume lv in targets)
                        {
                            CenterPivot(lv);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(allowViewFromInside, new GUIContent("允许从内部查看", "当相机进入容器时，液体是可见的。这是一项实验性功能，当相机位于容器内时，某些选项（如湍流）不可用."));
                EditorGUILayout.PropertyField(doubleSidedBias, new GUIContent("双面偏置", "可与不规则网格一起使用，以改善双面网格的渲染（即无盖眼镜具有两面，玻璃的外表面和内部面）。输入少量，该量将被减去到内部面的深度，这应该将它们排除在液体效应之外."));
                EditorGUILayout.PropertyField(rotationLevelBias, new GUIContent("旋转水平偏差", "使用更准确的算法来计算填充面积。如果液体在某些旋转下似乎消失了，请增加此值."));
                EditorGUILayout.PropertyField(debugSpillPoint, new GUIContent("调试溢出点", "旋转瓶子时，它会在液体应开始倾倒的点上显示一个小球体."));
                EditorGUILayout.PropertyField(fixedSpillPoint, new GUIContent("固定溢出点", "旋转瓶子时，液体流出位置不会改变"));
                EditorGUILayout.PropertyField(limitLevel, new GUIContent("最低水平", "最低水平,低于这个水平将不再减少."));
                EditorGUILayout.PropertyField(renderQueue, new GUIContent("渲染队列", "液体体积在 Transparent+1 队列（等于 3001）处渲染。您可以将其更改为 3000 以呈现为普通透明对象，或者根据需要使用其他值."));
            }


            EditorGUILayout.Separator();


            if (serializedObject.ApplyModifiedProperties() || (Event.current.type == EventType.ExecuteCommand &&
                Event.current.commandName == "UndoRedoPerformed"))
            {
                foreach (LiquidVolume lv in targets)
                {
                    lv.UpdateMaterialProperties();
                }
            }
        }

        #region Mesh tools

        public void BakeRotation(LiquidVolume lv)
        {

            if (PrefabUtility.GetPrefabObject(lv.gameObject) != null)
            {
                PrefabUtility.DisconnectPrefabInstance(lv.gameObject);
            }

            MeshFilter mf = lv.GetComponent<MeshFilter>();
            string meshPath = AssetDatabase.GetAssetPath(mf.sharedMesh);

            Mesh mesh = Instantiate<Mesh>(mf.sharedMesh) as Mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3 scale = lv.transform.localScale;
            lv.transform.localScale = Vector3.one;

            for (int k = 0; k < vertices.Length; k++)
            {
                vertices[k] = lv.transform.TransformVector(vertices[k]);
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mf.sharedMesh = mesh;

            SaveMeshAsset(mesh, meshPath);

            lv.transform.localRotation = Quaternion.Euler(0, 0, 0);
            lv.transform.localScale = scale;

            RefreshCollider(lv);
        }

        public void CenterPivot(LiquidVolume lv)
        {

            if (PrefabUtility.GetPrefabObject(lv.gameObject) != null)
            {
                PrefabUtility.DisconnectPrefabInstance(lv.gameObject);
            }

            MeshFilter mf = lv.GetComponent<MeshFilter>();
            string meshPath = AssetDatabase.GetAssetPath(mf.sharedMesh);

            Mesh mesh = Instantiate<Mesh>(mf.sharedMesh) as Mesh;
            Vector3[] vertices = mesh.vertices;

            Vector3 midPoint = Vector3.zero;
            for (int k = 0; k < vertices.Length; k++)
            {
                midPoint += vertices[k];
            }
            midPoint /= vertices.Length;
            for (int k = 0; k < vertices.Length; k++)
            {
                vertices[k] -= midPoint;
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mf.sharedMesh = mesh;

            SaveMeshAsset(mesh, meshPath);

            lv.transform.localPosition += midPoint;

            RefreshCollider(lv);
        }

        void SaveMeshAsset(Mesh mesh, string originalMeshPath)
        {
            if (originalMeshPath == null)
                return;
            string newPath = Path.ChangeExtension(originalMeshPath, null);
            AssetDatabase.CreateAsset(mesh, newPath + "_edited");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void RefreshCollider(LiquidVolume lv)
        {
            MeshCollider mc = lv.GetComponent<MeshCollider>();
            if (mc != null)
            {
                Mesh oldMesh = mc.sharedMesh;
                mc.sharedMesh = null;
                mc.sharedMesh = oldMesh;
            }
        }

        #endregion


    }

}
