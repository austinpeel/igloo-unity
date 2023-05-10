using UnityEngine;

[CreateAssetMenu(fileName = "New Numbers Sprites Axis", menuName = "Numbers Sprites Axis", order = 52)]
public class NumbersSpritesAxis : ScriptableObject
{
    public NumberSprite[] numberSprites;
}

[System.Serializable]
public struct NumberSprite
{
    public int number;
    public Sprite sprite;
}
