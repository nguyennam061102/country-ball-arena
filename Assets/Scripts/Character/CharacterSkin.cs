using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Skin", menuName = "PlayerSkin")]
public class CharacterSkin : ScriptableObject
{
    public int skinId;
    public Sprite skinSprite;
    public bool defaultSkin, useColor;
    public Color skinColor = new Color(255, 255, 255, 255);
    public SkeletonDataAsset faceAnim;
    public string skinName;
    public float health, speed;
}
