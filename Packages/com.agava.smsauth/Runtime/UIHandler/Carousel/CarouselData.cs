using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Agava.Wink
{
    [Serializable, Preserve]
    public class CarouselData
    {
        [field: SerializeField] public string FieldLabel { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField, TextArea] public string AppName { get; private set; }
        [field: SerializeField] public AppAuthenticator AppAuthenticator { get; private set; }
        [field: SerializeField] public AppMonetizationType AppMonetizationType { get; private set; }

        public void SetAppName(string remoteAppName) => AppName = remoteAppName;
    }
}
