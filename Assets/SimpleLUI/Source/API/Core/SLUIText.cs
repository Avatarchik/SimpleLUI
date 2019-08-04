//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API.Core.Math;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleLUI.API.Core
{
    public sealed class SLUIText : SLUIMaskableGraphic
    {
        public string text
        {
            get => Original.text;
            set => Original.text = value;
        }

        public int fontSize
        {
            get => Original.fontSize;
            set => Original.fontSize = value;
        }

        public string fontStyle => Original.fontStyle.ToString();
        public string alignment => Original.alignment.ToString();

        public bool resizeTextForBestFit
        {
            get => Original.resizeTextForBestFit;
            set => Original.resizeTextForBestFit = value;
        }

        public int resizeTextMinSize
        {
            get => Original.resizeTextMinSize;
            set => Original.resizeTextMinSize = value;
        }

        public int resizeTextMaxSize
        {
            get => Original.resizeTextMaxSize;
            set => Original.resizeTextMaxSize = value;
        }

        public SLUIColor color
        {
            get => Original.color.ToSLUIColor();
            set => Original.color = value.ToRealColor();
        }

        internal new Text Original { get; private set; }

        /// <inheritdoc />
        internal override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<Text>();
        }

        /// <inheritdoc />
        internal override void OnComponentLoaded()
        {
            // invoke base method
            base.OnComponentLoaded();

            // default
            Original.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        }

        public void SetFontStyle(string style)
        {
            if (Enum.TryParse<FontStyle>(style, true, out var t))
            {
                SetFontStyle(t);
            }
            else Debug.LogError($"Failed to parse '{style}' in to {typeof(FontStyle)}");
        }

        public void SetFontStyle(FontStyle s)
        {
            Original.fontStyle = s;
        }


        public void SetAlignment(string style)
        {
            if (Enum.TryParse<TextAnchor>(style, true, out var t))
            {
                SetAlignment(t);
            }
            else Debug.LogError($"Failed to parse '{style}' in to {typeof(TextAnchor)}");
        }

        public void SetAlignment(TextAnchor a)
        {
            Original.alignment = a;
        }
    }
}
