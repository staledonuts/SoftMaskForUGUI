using System;
using UnityEngine;

namespace Coffee.UISoftMask.Demos
{
    public class SoftMask_Demo_SoftMask : MonoBehaviour
    {
        [SerializeField] private SoftMask m_SoftMask;

        public void SetMaskingMode(int value)
        {
            m_SoftMask.maskingMode = (SoftMask.MaskingMode)value;
        }

        public void SetDownSamplingRate(int value)
        {
            m_SoftMask.downSamplingRate = (SoftMask.DownSamplingRate)value;
        }

        public void SetTransformSensitivity(int index)
        {
            var values =
                (UISoftMaskProjectSettings.TransformSensitivity[])Enum.GetValues(
                    typeof(UISoftMaskProjectSettings.TransformSensitivity));
            UISoftMaskProjectSettings.transformSensitivity = values[index];
        }

        public void SetSoftMaskingRange(MinMax01 value)
        {
            m_SoftMask.softMaskingRange = value;
        }
    }
}
