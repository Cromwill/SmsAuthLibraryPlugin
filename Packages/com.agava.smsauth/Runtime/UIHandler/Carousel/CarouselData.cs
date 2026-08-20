using System;
using UnityEngine;

namespace Agava.Wink
{
    [Serializable]
    public class CarouselData
    {
        [field: SerializeField] public string FieldLabel { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField, TextArea] public string AppName { get; private set; }
        [field: SerializeField] public AppAuthenticator AppAuthenticator { get; private set; }

        public void SetAppName(string remoteAppName) => AppName = remoteAppName;
    }
}
