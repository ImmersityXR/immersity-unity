using System;
using UnityEngine;
using UnityEngine.UI;

namespace Komodo.Runtime.Packages.KomodoCore.Runtime.Scripts.RuntimeSession
{
    public class AdjustableText : Text
    {
        [Serializable]
        enum Style
        {
            Title,
            Subtitle,
            H1,
            H2,
            H3,
            Body,
            SmallText,
            MonospaceText,
        }

        [SerializeField] private Style style;
    }
}
