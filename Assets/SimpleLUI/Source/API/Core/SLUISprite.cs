//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SimpleLUI.API.Core
{
    public class SLUISprite
    {
        public string File { get; }
        public Sprite Original { get; private set; }

        public SLUISprite() { }
        public SLUISprite(string file)
        {
            if (!file.Contains(":"))
            {
                file = $"{Environment.CurrentDirectory}\\{file}";
            }

            File = file;
            LoadFile();
        }

        private void LoadFile()
        {
            var url = $"file://{File}";
            using (UnityWebRequest r = UnityWebRequestTexture.GetTexture(url))
            {
                var req = r.SendWebRequest();
                while (!req.isDone)
                {
                    // wait
                }

                if (r.isNetworkError || r.isHttpError)
                    Debug.Log($"Failed to load {File}. {r.error}");
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(r);
                    Original = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                }
            }
        }
    }
}
