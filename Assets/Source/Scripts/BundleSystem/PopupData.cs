using UnityEngine;

[CreateAssetMenu(fileName = "PopupData", menuName = "Create new popup data", order = 51)]
public class PopupData : ScriptableObject
{
    [field: SerializeField] public Sprite Background { get; private set; }
    [field: SerializeField] public Sprite Logo { get; private set; }
    [field: SerializeField] public Sprite Button { get; private set; }
}
