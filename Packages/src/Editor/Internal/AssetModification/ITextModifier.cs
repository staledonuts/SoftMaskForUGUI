using System.Text;

namespace Coffee.UISoftMask.Internal.AssetModification
{
    internal interface ITextModifier
    {
        bool ModifyText(StringBuilder sb, string text);
    }
}
