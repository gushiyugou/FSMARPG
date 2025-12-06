using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new AttckAction", menuName = "Character/Attack/NormalAttackAction")]
public class NormalAttackAction_SO : ScriptableObject
{
    [Header("技能配置信息")]
    [SerializeField]private string animationName;
    [SerializeField]private string attackAnimationEndName;
    [SerializeField]private string[] hitAnimationName;
    [SerializeField]private AudioClip hitSound;
    [SerializeField]private AudioClip attackSound;
    [SerializeField]private float damaged;
    [SerializeField]private float coolTime;
    [SerializeField]private int actionID;
    [Header("技能特效")]
    [SerializeField] private AttackEffect effectPrefab;

    public string AnimationName => animationName;
    public string AttackAnimationEndName => attackAnimationEndName;
    public string[] HitAnimationName => hitAnimationName;
    public AudioClip HitSound => hitSound;
    public AudioClip AttackSound => attackSound;
    public float Damaged => damaged;
    public float CoolTime => coolTime;
    public int ActionID => actionID;
    public AttackEffect EffectPrefab => effectPrefab;
}

[Serializable]
public class AttackEffect
{
    public string name;
    public GameObject attackEffect;
    public float spawnerTime;
    public Vector3 position;
    public float rotationAngle;
    public float forwardOffset;
    public bool isSonCharacter;

}