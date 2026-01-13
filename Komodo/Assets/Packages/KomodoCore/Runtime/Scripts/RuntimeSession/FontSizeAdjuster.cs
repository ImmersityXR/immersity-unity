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
        private readonly int[] _sizes = new int[] {
            10,
            11,
            12,
            14,
            16, // default
            18,
            20,
            24,
            28,
            32,
            36,
            40,
            44,
            58,
            64,
            70,
        };

        private static int _currentIndex = 8; // size 16

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

            foreach (var text in GetComponentsInChildren<AdjustableText>())
            {
                text.fontSize =  _sizes[_currentIndex];
            }
            
            foreach (var group in GetComponentsInChildren<HorizontalOrVerticalLayoutGroup>())
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
    }
}