using UnityEngine;

namespace Agava.Wink
{
    [CreateAssetMenu(fileName = "CarouselItemAsset", menuName = "Create new CarouselItemAsset", order = 51)]
    public class CarouselItemAsset : ScriptableObject
    {
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField, TextArea] public string DeafaultLabel { get; private set; }
        //[field: SerializeField, LeanTranslationName] public string Description { get; private set; }
        [field: SerializeField] public AppAuthenticator AppAuthenticator { get; private set; }
    }
}
