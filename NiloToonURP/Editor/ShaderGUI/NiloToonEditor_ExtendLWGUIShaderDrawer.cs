using UnityEditor;
using UnityEngine;
using JTRP.ShaderDrawer;

namespace NiloToon.NiloToonURP
{
    /// <summary>
    /// Draw a min max slider inside a group (NiloToon extension)
    /// </summary>
    public class MinMaxSliderDrawer : SubDrawer
    {
        string minPropName;
        string maxPropName;
        public MinMaxSliderDrawer(string group, string minPropName, string maxPropName)
        {
            this.group = group;

            this.minPropName = minPropName;
            this.maxPropName = maxPropName;
        }

        protected override bool matchPropType => prop.type == MaterialProperty.PropType.Range;

        public override void DrawProp(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
        {
            // read min max
            MaterialProperty min = LWGUI.FindProp(minPropName, props, true);
            MaterialProperty max = LWGUI.FindProp(maxPropName, props, true);
            float minf = min.floatValue;
            float maxf = max.floatValue;

            // define draw area
            Rect controlRect = EditorGUILayout.GetControlRect(); // this is the full length rect area
            var w = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0;
            Rect inputRect = MaterialEditor.GetRectAfterLabelWidth(controlRect); // this is the remaining rect area after label's area
            EditorGUIUtility.labelWidth = w;

            // draw label
            EditorGUI.LabelField(controlRect, label);

            // draw min max slider
            Rect[] splittedRect = SplitRect(inputRect, 3);

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = min.hasMixedValue;
            EditorGUI.FloatField(splittedRect[0], minf);
            EditorGUI.showMixedValue = max.hasMixedValue;
            EditorGUI.FloatField(splittedRect[2], maxf);
            EditorGUI.showMixedValue = prop.hasMixedValue;
            EditorGUI.MinMaxSlider(splittedRect[1], ref minf, ref maxf, prop.rangeLimits.x, prop.rangeLimits.y);
            EditorGUI.showMixedValue = false;

            // write back min max if changed
            if (EditorGUI.EndChangeCheck())
            {
                min.floatValue = minf;
                max.floatValue = maxf;
            }
        }

        /// <summary>
        /// Draw a R/G/B/A drop menu inside a group (NiloToon extension)
        /// </summary>
        public class RGBAChannelMaskToVec4Drawer : SubDrawer
        {
            string[] names = new string[] { "R", "G", "B", "A", "RGB Average", "RGB Luminance" };
            int[] values = new int[] { 0, 1, 2, 3, 4, 5};

            public RGBAChannelMaskToVec4Drawer(string group)
            {
                this.group = group;
            }

            protected override bool matchPropType => prop.type == MaterialProperty.PropType.Vector;

            public override void DrawProp(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
            {
                // define all drop list
                Vector4 R = new Vector4(1, 0, 0, 0);
                Vector4 G = new Vector4(0, 1, 0, 0);
                Vector4 B = new Vector4(0, 0, 1, 0);
                Vector4 A = new Vector4(0, 0, 0, 1);
                Vector4 RGBAverage = new Vector4(1f/3f,1f/3f,1f/3f,0);
                Vector4 RGBLuminance = new Vector4(0.2126f, 0.7152f, 0.0722f, 0);

                var rect = EditorGUILayout.GetControlRect();
                int index;
                if (prop.vectorValue == R)
                    index = 0;
                else
                if (prop.vectorValue == G)
                    index = 1;
                else
                if (prop.vectorValue == B)
                    index = 2;
                else
                if (prop.vectorValue == A)
                    index = 3;
                else
                if (prop.vectorValue == RGBAverage)
                    index = 4;
                else
                if (prop.vectorValue == RGBLuminance)
                    index = 5;
                else
                {
                    Debug.LogError("RGBAChannelMaskToVec4Drawer invalid vector found, reset to a");
                    prop.vectorValue = A;
                    index = 3;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = prop.hasMixedValue;
                int num = EditorGUI.IntPopup(rect, label.text, index, names, values);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    Vector4 setValue;
                    switch (num)
                    {
                        case 0: setValue = R; break;
                        case 1: setValue = G; break;
                        case 2: setValue = B; break;
                        case 3: setValue = A; break;
                        case 4: setValue = RGBAverage; break;
                        case 5: setValue = RGBLuminance; break;
                        default:
                            throw new System.NotImplementedException();
                    }
                    prop.vectorValue = setValue;
                }
            }
        }

        // copy and edit of https://github.com/GucioDevs/SimpleMinMaxSlider/blob/master/Assets/SimpleMinMaxSlider/Scripts/Editor/MinMaxSliderDrawer.cs
        Rect[] SplitRect(Rect rectToSplit, int n)
        {
            Rect[] rects = new Rect[n];

            for (int i = 0; i < n; i++)
            {
                rects[i] = new Rect(rectToSplit.position.x + (i * rectToSplit.width / n), rectToSplit.position.y, rectToSplit.width / n, rectToSplit.height);
            }

            int padding = (int)rects[0].width - 50; // use 50, enough to show 0.xx (2 digits)
            int space = 5;

            rects[0].width -= padding + space;
            rects[2].width -= padding + space;

            rects[1].x -= padding;
            rects[1].width += padding * 2;

            rects[2].x += padding + space;

            return rects;
        }
    }
}
