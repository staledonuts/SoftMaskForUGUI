﻿using UnityEditor;
using UnityEngine;

namespace Coffee.UISoftMask
{
    [CustomEditor(typeof(MaskingShape), true)]
    [CanEditMultipleObjects]
    public class MaskingShapeEditor : Editor
    {
        private SerializedProperty _alphaHitTest;
        private SerializedProperty _antiAliasingThreshold;
        private SerializedProperty _maskingMethod;
        private SerializedProperty _showMaskGraphic;
        private SerializedProperty _softnessRange;

        protected void OnEnable()
        {
            _maskingMethod = serializedObject.FindProperty("m_MaskingMethod");
            _showMaskGraphic = serializedObject.FindProperty("m_ShowMaskGraphic");
            _alphaHitTest = serializedObject.FindProperty("m_AlphaHitTest");
            _antiAliasingThreshold = serializedObject.FindProperty("m_AntiAliasingThreshold");
            _softnessRange = serializedObject.FindProperty("m_SoftnessRange");
        }

        public override void OnInspectorGUI()
        {
            var current = target as MaskingShape;
            if (!current) return;

            Utils.GetStencilDepthAndMask(current.transform, false, out var mask);
            var maskingMode = mask is SoftMask softMask ? softMask.GetActualMaskingMode() : SoftMask.MaskingMode.Normal;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup(new GUIContent("Masking Mode"), maskingMode);
            EditorGUI.EndDisabledGroup();


            EditorGUILayout.PropertyField(_maskingMethod);
            // EditorGUILayout.PropertyField(_showMaskGraphic);
            // EditorGUILayout.PropertyField(_alphaHitTest);

            switch (maskingMode)
            {
                case SoftMask.MaskingMode.SoftMasking:
                    EditorGUILayout.PropertyField(_showMaskGraphic);
                    EditorGUILayout.PropertyField(_alphaHitTest);
                    EditorGUILayout.PropertyField(_softnessRange);
                    break;
                case SoftMask.MaskingMode.AntiAliasing:
                    EditorGUILayout.PropertyField(_alphaHitTest);
                    EditorGUILayout.PropertyField(_antiAliasingThreshold);
                    break;
                case SoftMask.MaskingMode.Normal:
                    EditorGUILayout.PropertyField(_showMaskGraphic);
                    break;
            }

            // AntiAliasing is only available in Mask
            // EditorGUI.BeginDisabledGroup(mask is SoftMask);
            // {
            //     EditorGUILayout.PropertyField(_antiAliasing);
            //     EditorGUI.BeginDisabledGroup(!_antiAliasing.boolValue);
            //     {
            //         EditorGUILayout.PropertyField(_antiAliasingThreshold);
            //     }
            // }
            // EditorGUI.EndDisabledGroup();
            //
            // // Softness is only available in SoftMask
            // EditorGUI.BeginDisabledGroup(!(mask is SoftMask));
            // {
            //     EditorGUILayout.PropertyField(_softnessRange);
            // }
            // EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();

            // Draw alpha hit test warning
            if (current.alphaHitTest)
            {
                SoftMaskEditor.DrawAlphaHitTestWarning(current.graphic);
            }
        }
    }
}
