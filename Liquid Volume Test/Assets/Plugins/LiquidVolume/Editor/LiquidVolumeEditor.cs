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
            EditorGUILayout.LabelField("ͨ������", titleLabelStyle);
            if (GUILayout.Button("����", GUILayout.Width(40)))
            {
                if (!EditorUtility.DisplayDialog("Liquid Volume", "To learn more about a property in this inspector move the mouse over the label for a quick description (tooltip).\n\nPlease check README file in the root of the asset for details and contact support.\n\nIf you like Liquid Volume, please rate it on the Asset Store. For feedback and suggestions visit our support forum on kronnect.com.", "Close", "Visit Support Forum"))
                {
                    Application.OpenURL("http://kronnect.com/taptapgo");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(topology, new GUIContent("���粼������", "�����״."));
            EditorGUILayout.PropertyField(detail, new GUIContent("ϸ��", "Һ��Ч����ϸ��������Simple�����ò�ʹ��3D������ʹ�������ƶ��豸����."));
            EditorGUILayout.PropertyField(depthAware, new GUIContent("���", "��Һ�����������z-testing���������к���Һ��������������壬��ʹ�ã�����Ҫ���á�Һ������ڵ�2D������Ҫʹ��һ����͸�����и���ɫ��д��z-buffer(��׼��ɫ���и�ģʽ��һ���ܺõ�ѡ��)."));
            if (target != null)
            {
                LiquidVolume lv = (LiquidVolume)target;
                if (lv.transform.parent == null)
                    GUI.enabled = false;
            }
            EditorGUILayout.PropertyField(depthAwareCustomPass, new GUIContent("������", "ʹ�ø�����ͼ����Ϊ��ǰҺ��ı߽硣����Һ�����λ����һ�������ڲ�ʱ�����ڲ��������˽ṹ��ʹ�ã�����ֹ͸���������������е�����Һ�塣����������κ�αӰ���벻Ҫ���ô�ѡ���Ϊ����ΪҺ��������Ӷ������Ⱦͨ��."));
            GUI.enabled = true;

            EditorGUILayout.PropertyField(alpha, new GUIContent("����͸����", "Һ�塢�������ĭ��ȫ��͸���ȡ��������Խ�����Һ�壬�������ĭ��ɫ�İ���������."));

            int detailed = detail.intValue;

            if (detailed != (int)DETAIL.Smoke)
            {
                EditorGUILayout.Separator();
                expandSection[LIQUID_SETTINGS] = EditorGUILayout.Foldout(expandSection[LIQUID_SETTINGS], "Һ�������", sectionHeaderStyle);

                if (expandSection[LIQUID_SETTINGS])
                {
                    EditorGUILayout.PropertyField(level, new GUIContent("ˮƽ", "��������ˮƽ."));

                    EditorGUILayout.PropertyField(liquidColor1, new GUIContent("Һ����ɫ1"));
                    if (detailed >= 10)
                    {
                        EditorGUILayout.PropertyField(liquidScale1, new GUIContent("����1", "Ӧ����Һ���һ����Ŀ̶�."));
                        EditorGUILayout.PropertyField(liquidColor2, new GUIContent("Һ����ɫ2"));
                        EditorGUILayout.PropertyField(liquidScale2, new GUIContent("����2", "Scale applied to the 2nd texture of the liquid."));
                        EditorGUILayout.PropertyField(murkiness, new GUIContent("��������", "Һ��Ĵ��ȡ�0 = ��Ө��͸��1 = �����ཬ/�۹�."));
                    }

                    EditorGUILayout.PropertyField(emissionColor, new GUIContent("�Է�����ɫ"));
                    EditorGUILayout.PropertyField(emissionBrightness, new GUIContent("�Է�������"));
                    EditorGUILayout.PropertyField(ditherShadows, new GUIContent("������Ӱ", "���ô�ѡ��ɶ�Һ����ӰӦ�ö�����ģ�ⲿ��͸������Ӱ��Ϊ�˻�����Ч���������������������������Ӱ."));
                    EditorGUILayout.PropertyField(turbulence1, new GUIContent("���������", "���������."));
                    EditorGUILayout.PropertyField(turbulence2, new GUIContent("���������", "���������."));
                    EditorGUILayout.PropertyField(frecuency, new GUIContent("Ƶ��", "������Ƶ���ԡ������Բ������̵Ĳ���."));
                    EditorGUILayout.PropertyField(speed, new GUIContent("�ٶ�", "�����������ٶ�."));

                    if (detailed >= 10)
                    {
                        EditorGUILayout.PropertyField(sparklingIntensity, new GUIContent("���������ǿ��", "�������ӵ�����."));
                        EditorGUILayout.PropertyField(sparklingAmount, new GUIContent("������", "��������/�������������."));
                    }

                    EditorGUILayout.PropertyField(deepObscurance, new GUIContent("����ڵ�", "ʹҺ��ĵײ��䰵."));
                    EditorGUILayout.PropertyField(scatteringEnabled, new GUIContent("��ɢ��", "ʹ���⴩��Һ���������ɢЧ��."));
                    if (scatteringEnabled.boolValue)
                    {
                        EditorGUILayout.PropertyField(scatteringPower, new GUIContent("   ����", "��ɢ�䷽�̵Ĺ��ʣ�ָ����."));
                        EditorGUILayout.PropertyField(scatteringAmount, new GUIContent("   ��", "��ɢ��ЧӦ�����ճ˷�����˥��."));
                    }

                    if (detailed == 0)
                    {
                        EditorGUILayout.PropertyField(foamVisibleFromBottom, new GUIContent("�ӵײ��ɼ�", "����ӵײ��۲�����ʱ��ͨ��Һ����Կ�����ĭ."));
                    }

                }

                if (detailed >= 10)
                {
                    EditorGUILayout.Separator();
                    expandSection[FOAM_SETTINGS] = EditorGUILayout.Foldout(expandSection[FOAM_SETTINGS], "��ĭ������", sectionHeaderStyle);

                    if (expandSection[FOAM_SETTINGS])
                    {
                        EditorGUILayout.PropertyField(foamColor, new GUIContent("��ɫ"));
                        EditorGUILayout.PropertyField(foamScale, new GUIContent("����", "Ӧ������ĭ����Ŀ̶�."));
                        EditorGUILayout.PropertyField(foamThickness, new GUIContent("���"));
                        EditorGUILayout.PropertyField(foamDensity, new GUIContent("�ܶ�"));
                        EditorGUILayout.PropertyField(foamWeight, new GUIContent("����", "ֵԽ����ĭ��Һ��ĵ����ܶ�Խ��."));
                        EditorGUILayout.PropertyField(foamTurbulence, new GUIContent("����", "Ӱ����ĭ��Һ�������ĳ�������������Ϊ���Բ���������ĭ."));
                        EditorGUILayout.PropertyField(foamVisibleFromBottom, new GUIContent("�ӵײ��ɼ�", "����ӵײ��۲�����ʱ��ͨ��Һ����Կ�����ĭ."));
                    }
                }
            }

            EditorGUILayout.Separator();
            expandSection[SMOKE_SETTINGS] = EditorGUILayout.Foldout(expandSection[SMOKE_SETTINGS], "���������", sectionHeaderStyle);

            if (expandSection[SMOKE_SETTINGS])
            {
                EditorGUILayout.PropertyField(smokeEnabled, new GUIContent("�ɼ�"));
                if (smokeEnabled.boolValue)
                {
                    EditorGUILayout.PropertyField(smokeColor, new GUIContent("��ɫ"));
                    if (detailed >= 10)
                    {
                        EditorGUILayout.PropertyField(smokeScale, new GUIContent("����", "Ӧ�����������������Ŀ̶�."));
                        EditorGUILayout.PropertyField(smokeSpeed, new GUIContent("�ٶ�"));
                    }
                    EditorGUILayout.PropertyField(smokeBaseObscurance, new GUIContent("�����ڵ�", "ʹ�ײ�������䰵."));
                }
            }

            if (detailed != (int)DETAIL.DefaultNoFlask && detailed != (int)DETAIL.Smoke)
            {
                EditorGUILayout.Separator();
                expandSection[FLASK_SETTINGS] = EditorGUILayout.Foldout(expandSection[FLASK_SETTINGS], "ƿ�ӵ�����", sectionHeaderStyle);

                if (expandSection[FLASK_SETTINGS])
                {
                    EditorGUILayout.PropertyField(flaskTint, new GUIContent("ɫ��", "Ӧ���ھ����ɫ����ɫ."));
                    EditorGUILayout.PropertyField(flaskThickness, new GUIContent("���", "ˮ��˼ά."));
                    EditorGUILayout.PropertyField(flaskGlossinessExternal, new GUIContent("����� ����", "���������Ĺ����."));
                    if (detailed != 30)
                    {
                        EditorGUILayout.PropertyField(flaskGlossinessInternal, new GUIContent("����� �ڲ�", "�����ڱ���Ĺ����."));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(reflectionTexture, new GUIContent("����", "Ϊ����Ч��ָ����������ͼ����."));
                        EditorGUILayout.PropertyField(textureAlpha, new GUIContent("͸����"));
                    }
                    if (detailed == 20)
                    {
                        EditorGUILayout.PropertyField(texture, new GUIContent("����", "ΪҺ������ָ������."));
                        EditorGUILayout.PropertyField(textureAlpha, new GUIContent("   ͸����"));
                        EditorGUILayout.PropertyField(textureScale, new GUIContent("   ����"));
                        EditorGUILayout.PropertyField(textureOffset, new GUIContent("   ƫ����"));
                        EditorGUILayout.PropertyField(bumpMap, new GUIContent("������ͼ", "ΪҺ������ָ��������ͼ."));
                    }
                    EditorGUILayout.PropertyField(refractionBlur, new GUIContent("����ģ��", "ͨ��ƿ�ӿɼ���ģ������."));
                    if (refractionBlur.boolValue)
                    {
                        EditorGUILayout.PropertyField(blurIntensity, new GUIContent("   ǿ��"));
                        EditorGUILayout.PropertyField(distortionMap, new GUIContent("   ʧ��ͼ", "�ڴ˲����Ϊ����ʧ��ָ���û�ͼ."));
                        EditorGUILayout.PropertyField(distortionAmount, new GUIContent("   ʧ����"));
                    }

                    EditorGUILayout.PropertyField(bumpDistortionScale, new GUIContent("��͹/ʧ�����", "��͹��ʧ����ͼ������������."));
                    EditorGUILayout.PropertyField(bumpDistortionOffset, new GUIContent("��͹/ʧ��ƫ��", "��͹��ʧ����ͼ���������ƫ��."));

                }
            }

            EditorGUILayout.Separator();
            expandSection[PHYSICS_SETTINGS] = EditorGUILayout.Foldout(expandSection[PHYSICS_SETTINGS], "��������", sectionHeaderStyle);
            if (expandSection[PHYSICS_SETTINGS])
            {
                EditorGUILayout.PropertyField(reactToForces, new GUIContent("����������Ӧ", "���ú�Һ�彫����ƿ���ƶ�����ͼ��������."));
                GUI.enabled = reactToForces.boolValue;
                EditorGUILayout.PropertyField(physicsMass, new GUIContent("����", "�����������ʹҺ����Ӿ�̬."));
                EditorGUILayout.PropertyField(physicsAngularDamp, new GUIContent("�Ƕȳ�ʪ", "Һ������ƿ��Ħ��������������Һ����ʩ�����󷵻�����λ�õ��ٶ�."));
                GUI.enabled = !reactToForces.boolValue;
                EditorGUILayout.PropertyField(ignoreGravity, new GUIContent("��������", "���ú�Һ�彫��ƿ����ת��Ĭ�������Ϊ false������ζ��Һ�彫ͣ������ƿ�ĵײ�."));
                GUI.enabled = true;
            }

            EditorGUILayout.Separator();
            expandSection[ADVANCED_SETTINGS] = EditorGUILayout.Foldout(expandSection[ADVANCED_SETTINGS], "�߼�����", sectionHeaderStyle);

            if (expandSection[ADVANCED_SETTINGS])
            {
                EditorGUILayout.PropertyField(smokeRaySteps, new GUIContent("��������", "���ڹ�����ɫ��ÿ�����ص�������."));
                if (detailed != (int)DETAIL.Smoke)
                {
                    EditorGUILayout.PropertyField(liquidRaySteps, new GUIContent("Һ������", "���ڹ���Һ����ɫ��ÿ�����ص�������."));
                }
                if (detailed >= 1)
                {
                    if (detailed != (int)DETAIL.Smoke)
                    {
                        EditorGUILayout.PropertyField(foamRaySteps, new GUIContent("��ĭ����", "���ڹ�����ĭ��ɫ��ÿ�����ص�������."));
                    }
                    EditorGUILayout.PropertyField(noiseVariation, new GUIContent("���仯", "�� 3 �ֲ�ͬ�� 3D ����֮�����ѡ��."));
                }
                EditorGUILayout.PropertyField(upperLimit, new GUIContent("����", "Һ�塢��ĭ�����������ƿ�ӳߴ������."));
                EditorGUILayout.PropertyField(extentsScale, new GUIContent("��Χ����", "��ѡ�͸��ӳ���Ӧ��������ĵ�ǰ��С����������Ҫ�˹��ܵ��ض��ͺ��ϵ�����ƽ."));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("�決��ǰTransform"))
                {
                    if (EditorUtility.DisplayDialog("�決��ǰTransform", "��ǰ�任����ת�����ţ���ת�Ƶ��������Ƿ�Ҫ������", "�õ�", "ȡ��"))
                    {
                        foreach (LiquidVolume lv in targets)
                        {
                            BakeRotation(lv);
                        }
                    }
                }
                if (GUILayout.Button("����ê��"))
                {
                    if (EditorUtility.DisplayDialog("����ê��", "���㽫����λ��������Ὣ���¶�λ�������ġ��Ƿ�Ҫ������", "�õ�", "ȡ��"))
                    {
                        foreach (LiquidVolume lv in targets)
                        {
                            CenterPivot(lv);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(allowViewFromInside, new GUIContent("������ڲ��鿴", "�������������ʱ��Һ���ǿɼ��ġ�����һ��ʵ���Թ��ܣ������λ��������ʱ��ĳЩѡ���������������."));
                EditorGUILayout.PropertyField(doubleSidedBias, new GUIContent("˫��ƫ��", "���벻��������һ��ʹ�ã��Ը���˫���������Ⱦ�����޸��۾��������棬�������������ڲ��棩����������������������ȥ���ڲ������ȣ���Ӧ�ý������ų���Һ��ЧӦ֮��."));
                EditorGUILayout.PropertyField(rotationLevelBias, new GUIContent("��תˮƽƫ��", "ʹ�ø�׼ȷ���㷨�����������������Һ����ĳЩ��ת���ƺ���ʧ�ˣ������Ӵ�ֵ."));
                EditorGUILayout.PropertyField(debugSpillPoint, new GUIContent("���������", "��תƿ��ʱ��������Һ��Ӧ��ʼ�㵹�ĵ�����ʾһ��С����."));
                EditorGUILayout.PropertyField(fixedSpillPoint, new GUIContent("�̶������", "��תƿ��ʱ��Һ������λ�ò���ı�"));
                EditorGUILayout.PropertyField(limitLevel, new GUIContent("���ˮƽ", "���ˮƽ,�������ˮƽ�����ټ���."));
                EditorGUILayout.PropertyField(renderQueue, new GUIContent("��Ⱦ����", "Һ������� Transparent+1 ���У����� 3001������Ⱦ�������Խ������Ϊ 3000 �Գ���Ϊ��ͨ͸�����󣬻��߸�����Ҫʹ������ֵ."));
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
