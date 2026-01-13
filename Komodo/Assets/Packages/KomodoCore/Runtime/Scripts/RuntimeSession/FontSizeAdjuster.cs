using System;
using System.Collections;
using System.Collections.Generic;
using Komodo.Runtime.Packages.KomodoCore.Runtime.Scripts.RuntimeSession;
using UnityEngine;
using UnityEngine.UI;

namespace Komodo.Runtime
{
    public class FontSizeAdjuster : MonoBehaviour
    {
        enum SymbolDisplayMode
        {
            IconsAndText,
            TextOnly,
            IconsOnly,
        }

        private SymbolDisplayMode _symbolDisplayMode;
        
        private readonly int[] _sizes = new int[] {
            10,
            11,
            12,
            14,
            16,
            18,
            20,
            24,
            28, // default
            32,
            36, 
            40,
            44,
            58,
            64,
            70,
        };

        private static int _currentIndex = 8; // size 28

        public void Start()
        {
            _Apply();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadEquals))
            {
                Increase();
            }
            
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Decrease();
            }

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                CycleSymbolDisplayMode();
            }
        }

        private void CycleSymbolDisplayMode()
        {
            void disableIcons()
            {
                foreach (var icon in Resources.FindObjectsOfTypeAll<AdjustableIcon>())
                {
                    icon.gameObject.SetActive(false);
                }
            }

            void enableIcons()
            {
                foreach (var icon in Resources.FindObjectsOfTypeAll<AdjustableIcon>())
                {
                    icon.gameObject.SetActive(true);
                }
            }

            void disableText()
            {
                foreach (var text in Resources.FindObjectsOfTypeAll<AdjustableText>())
                {
                    text.gameObject.SetActive(false);
                }
            }

            void enableText()
            {
                foreach (var text in Resources.FindObjectsOfTypeAll<AdjustableText>())
                {
                    text.gameObject.SetActive(true);
                }
            }
            
            switch (_symbolDisplayMode)
            {
                case SymbolDisplayMode.IconsAndText:
                    _symbolDisplayMode = SymbolDisplayMode.TextOnly;
                    disableIcons();
                    // text will already be enabled
                    break;
                case SymbolDisplayMode.TextOnly:
                    _symbolDisplayMode = SymbolDisplayMode.IconsOnly;
                    disableText();
                    enableIcons();
                    break;
                case SymbolDisplayMode.IconsOnly:
                    _symbolDisplayMode = SymbolDisplayMode.IconsAndText;
                    enableText();
                    // icons will already be enabled
                    break;
            }
        }

        [ContextMenu("Increase")]
        public void Increase ()
        {
            if (_currentIndex < _sizes.Length - 1)
            {
                _currentIndex += 1;
            }

            _Apply();
        }

        [ContextMenu("Decrease")]
        public void Decrease ()
        {
            if (_currentIndex > 0)
            {
                _currentIndex -= 1;
            }

            _Apply();
        }

        private void _Apply ()
        {
            Debug.Log($"Font size is now {_sizes[_currentIndex]}");

            foreach (var text in Resources.FindObjectsOfTypeAll<AdjustableText>())
            {
                text.fontSize =  _sizes[_currentIndex];
            }
            
            foreach (var group in Resources.FindObjectsOfTypeAll<AdjustableVerticalLayoutGroup>())
            {
                group.spacing =  _sizes[_currentIndex] / 4.0f;
            }
            
            foreach (var group in Resources.FindObjectsOfTypeAll<AdjustableHorizontalLayoutGroup>())
            {
                group.spacing =  _sizes[_currentIndex] / 4.0f;
            }
            
            foreach (var element in GetComponentsInChildren<LayoutElement>())
            {
                if (element.preferredWidth - -1.0f > 0.001f)
                {
                    element.preferredWidth =  _sizes[_currentIndex] * 2.0f;
                    element.preferredHeight =  _sizes[_currentIndex] * 2.0f;
                }
            }
        }

        /// <summary>
        /// Reset the font to the default so that prefabs are not dirtied during development testing.
        /// </summary>
        void OnApplicationQuit()
        {
            _currentIndex = 8;
            _Apply();
        }
    }
}