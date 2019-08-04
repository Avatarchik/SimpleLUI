//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API.Core;
using SimpleLUI.Editor.Util;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleLUI.Editor
{
    internal class SLUILuaBuilder
    {
        public StringBuilder String { get; }

        internal SLUILuaBuilder()
        {
            String = new StringBuilder();
        }

        public void Space(string label = null)
        {
            String.AppendLine();
            if (!string.IsNullOrEmpty(label))
                String.AppendLine($"-- {label}");
        }

        public void Import(string lib, string nameSpace)
        {
            String.AppendLine($"import ('{lib}', '{nameSpace}')");
        }

        public string GameObjectFromRect(RectTransform t)
        {
            var name = CollectVar(t);
            String.AppendLine($"local {name} = core:Create('{t.name}')");
            if (!t.gameObject.activeSelf)
                String.AppendLine($"{name}:SetActive(false)");
            return name;
        }

        public void Parent(RectTransform child, RectTransform parent)
        {
            var childName = CollectVar(child);
            var parentName = CollectVar(parent);

            String.AppendLine($"{childName}.rectTransform:SetParent({parentName}.rectTransform)");
        }

        public void Anchor(RectTransform t)
        {
            var name = CollectVar(t);
            var anchor = SLUIRectTransform.GetAnchor(t);
            if (anchor != SLUIRectAnchorName.Unknown)
            {
                String.AppendLine($"{name}.rectTransform:SetAnchor('{SLUIRectTransform.GetAnchor(t)}')");
            }
            else
            {
                String.AppendLine($"{name}.rectTransform.anchorMin = {CollectVector2(t.anchorMin)}");
                String.AppendLine($"{name}.rectTransform.anchorMax = {CollectVector2(t.anchorMax)}");
            }
        }

        public void Transform(RectTransform t)
        {
            var name = CollectVar(t);

            if (t.pivot.x != 0.5f || t.pivot.y != 0.5f)
                String.AppendLine($"{name}.rectTransform.pivot = {CollectVector2(t.pivot)}");
            String.AppendLine($"{name}.rectTransform.anchoredPosition = {CollectVector2(t.anchoredPosition)}");
            String.AppendLine($"{name}.rectTransform.sizeDelta = {CollectVector2(t.sizeDelta)}");
            if (t.localRotation != Quaternion.identity)
                String.AppendLine($"{name}.rectTransform.localRotation = {CollectQuaternion(t.localRotation)}");
            if (t.localScale != Vector3.one)
                String.AppendLine($"{name}.rectTransform.localScale = {CollectVector2(t.localScale)}");
        }

        public string Image(Image i, string spriteName)
        {
            var parentName = CollectVar(i.rectTransform);
            var name = CollectVar(i);

            String.AppendLine($"local {name} = {parentName}:AddComponent('Image')");
            String.AppendLine($"{name}:SetType('{i.type.ToString()}')");
            if (i.color != Color.white)
                String.AppendLine($"{name}.color = {CollectColor(i.color)}");
            if (!i.raycastTarget)
                String.AppendLine($"{name}.raycastTarget = false");
            if (i.preserveAspect)
                String.AppendLine($"{name}.preserveAspect = true");
            if(!i.fillCenter)
                String.AppendLine($"{name}.fillCenter = false");
            if (File.Exists(spriteName))
            {
                String.AppendLine($"{name}.sprite = '{spriteName}'");
            }

            return name;
        }

        public string Button(Button b)
        {
            var parentName = CollectVar(b.GetComponent<RectTransform>());
            var name = CollectVar(b);

            String.AppendLine($"local {name} = {parentName}:AddComponent('Button')");
            if (!b.interactable)
                String.AppendLine($"{name}.interactable = false");
            String.AppendLine($"{name}.normalColor = {CollectColor(b.colors.normalColor)}");
            String.AppendLine($"{name}.highlightedColor = {CollectColor(b.colors.highlightedColor)}");
            String.AppendLine($"{name}.pressedColor = {CollectColor(b.colors.pressedColor)}");
            String.AppendLine($"{name}.selectedColor = {CollectColor(b.colors.selectedColor)}");
            String.AppendLine($"{name}.disabledColor = {CollectColor(b.colors.disabledColor)}");
            return name;
        }

        public string Text(Text t)
        {
            var parentName = CollectVar(t.GetComponent<RectTransform>());
            var name = CollectVar(t);

            String.AppendLine($"local {name} = {parentName}:AddComponent('Text')");
            String.AppendLine($"{name}.text = {'"'}{t.text}{'"'}");
            if (t.fontSize != 14)
                String.AppendLine($"{name}.fontSize = {t.fontSize}");
            if (t.fontStyle != FontStyle.Normal)
                String.AppendLine($"{name}:SetFontStyle('{t.fontStyle.ToString()}')");
            if (t.alignment != TextAnchor.UpperLeft)
                String.AppendLine($"{name}:SetAlignment('{t.alignment.ToString()}')");
            if (t.resizeTextForBestFit)
                String.AppendLine($"{name}.resizeTextForBestFit = true");
            if (t.resizeTextMinSize != 10)
                String.AppendLine($"{name}.resizeTextMinSize = {t.resizeTextMinSize}");
            if (t.resizeTextMaxSize != 40)
                String.AppendLine($"{name}.resizeTextMaxSize = {t.resizeTextMaxSize}");
            String.AppendLine($"{name}.color = {CollectColor(t.color)}");
            if (!t.raycastTarget)
                String.AppendLine($"{name}.raycastTarget = false");

            return name;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.ToString();
        }

        private static string CollectVar(RectTransform r)
        {
            return SLUILuaBuilderSyntax.FixVarName($"obj{r.gameObject.GetInstanceID()}");
            //SLUILuaBuilderSyntax.FixVarName(r.name.ToLower());
        }

        private static string CollectVar(Component c)
        {
            return SLUILuaBuilderSyntax.FixVarName($"obj{c.gameObject.GetInstanceID()}_{c.GetType().Name.ToLower()}"); 
            //SLUILuaBuilderSyntax.FixVarName(c.name.ToLower() + "_" + c.GetType().Name.ToLower());
        }

        private static string CollectQuaternion(Quaternion q, bool simple = false)
        {
            var str = $"{q.x.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{q.y.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{q.z.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{q.w.ToString(CultureInfo.InvariantCulture)}";
            if (!simple)
            {
                str = $"SLUIQuaternion({str})";
            }
            return str;
        }
        private static string CollectVector2(Vector2 v, bool simple = false)
        {
            var str = $"{v.x.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{v.y.ToString(CultureInfo.InvariantCulture)}";
            if (!simple)
            {
                str = $"SLUIVector2({str})";
            }
            return str;
        }
        private static string CollectColor(Color c, bool simple = false)
        {
            var str = $"{c.r.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{c.g.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{c.b.ToString(CultureInfo.InvariantCulture)}, " +
                      $"{c.a.ToString(CultureInfo.InvariantCulture)}";
            if (!simple)
            {
                str = $"SLUIColor({str})";
            }
            return str;
        }
    }
}
