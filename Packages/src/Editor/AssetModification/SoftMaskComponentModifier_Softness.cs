using Coffee.UISoftMask.Internal.AssetModification;
using UnityEditor;

#pragma warning disable CS0612 // Type or member is obsolete

namespace Coffee.UISoftMask
{
    internal class SoftMaskComponentModifier_Softness : ComponentModifier<SoftMask>
    {
        protected override bool ModifyComponent(SoftMask softMask, bool dryRun)
        {
            if (softMask.softness < 0) return false;

            if (!dryRun)
            {
                var go = softMask.gameObject;
                softMask.softMaskingRange = new MinMax01(0, softMask.softness);
                softMask.softness = -1;
                EditorUtility.SetDirty(go);
            }

            return true;
        }

        public override string Report()
        {
            return "  -> SoftMask.softness is obsolete. Use SoftMask.softMaskingRange instead.\n";
        }
    }
}
