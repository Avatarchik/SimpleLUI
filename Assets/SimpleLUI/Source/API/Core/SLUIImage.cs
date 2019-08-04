//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using SimpleLUI.API.Core.Math;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SimpleLUI.API.Core
{
    [Serializable]
    public class SLUIImageStyle
    {
        public SLUIVector2 Pivot = new SLUIVector2(0.5f, 0.5f);
        public float PixelPerUnit = 100f;
        public uint Extrude = 1;
        public SpriteMeshType MeshType = SpriteMeshType.Tight;
        public SLUIQuaternion Border = new SLUIQuaternion(0f, 0f, 0f, 0f);
    }

    public sealed class SLUIImage : SLUIMaskableGraphic
    {
        private string _sprite;
        private Coroutine _spriteWorker;
        public string sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                if (!sprite.Contains(":"))
                {
                    sprite = $"{Environment.CurrentDirectory}//{sprite}";
                }

                if (!Original.isActiveAndEnabled)
                {
                    // TODO
                    return;
                }

                if (_spriteWorker != null)
                {
                    Original.StopCoroutine(_spriteWorker);
                    _spriteWorker = null;
                }

                _spriteWorker = Original.StartCoroutine(LoadFile());
            }
        }

        public bool preserveAspect
        {
            get => Original.preserveAspect;
            set => Original.preserveAspect = value;
        }

        public SLUIColor color
        {
            get => Original.color.ToSLUIColor();
            set => Original.color = value.ToRealColor();
        }

        public string imageType => Original.type.ToString();

        internal new Image Original { get; private set; }

        public SLUIImage() { }
        ~SLUIImage()
        {
            if (_spriteWorker != null && Original != null)
                Original.StopCoroutine(_spriteWorker);
        }

        /// <inheritdoc />
        internal override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<Image>();
        }

        private IEnumerator LoadFile()
        {
            var url = $"file:///{sprite}";
            using (var r = UnityWebRequestTexture.GetTexture(url))
            {
                yield return r.SendWebRequest();
                
                if (r.isNetworkError || r.isHttpError)
                    Debug.LogError($"Failed to load {url}. {r.error}");
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(r);
                    var styleFile = sprite + ".style";
                    var style = new SLUIImageStyle();
                    if (File.Exists(styleFile))
                    {
                        style = JsonUtility.FromJson<SLUIImageStyle>(File.ReadAllText(styleFile));
                    }
        
                    Original.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                        style.Pivot.ToRealVector(), style.PixelPerUnit, style.Extrude, style.MeshType,
                        new Vector4(style.Border.x, style.Border.y, style.Border.z, style.Border.w));
                    Original.sprite.name = Path.GetFileNameWithoutExtension(sprite) ?? throw new InvalidOperationException();

                    yield return Original.sprite;
                }
            }

            _spriteWorker = null;
        }

        public void SetType([NotNull] string type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (Enum.TryParse<Image.Type>(type, true, out var t))
            {
                SetType(t);
            }
            else Debug.LogError($"Failed to parse '{type}' in to {typeof(Image.Type)}");
        }

        public void SetType(Image.Type type)
        {
            Original.type = type;
        }
    }
}
