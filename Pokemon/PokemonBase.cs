using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;
    // Base stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] private List<LearnableMove> learnableMoves;
    
    public string Name { get => name; }
    public string Description { get => description;}
    public int MaxHp { get => maxHp;}
    public int Attack { get => attack;}
    public int Defense { get => defense;}
    public int SpAttack { get => spAttack;}
    public int SpDefense { get => spDefense;}
    public Sprite FrontSprite { get => frontSprite;}
    public Sprite BackSprite { get => backSprite;}
    public PokemonType Type1 { get => type1;}
    public PokemonType Type2 { get => type2; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; }
    
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase moveBase;
    [SerializeField] private int level;

    public MoveBase MoveBase
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
}

public static class TypeChart
{
    static float[][] chart =
    {
        //                 NOR FIR WAT ELE GRAS ICE FIG POI GRO FLY PSY BUG ROC GHO DRA
        /*NOR*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*FIR*/ new float[] { 1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*WAT*/ new float[] { 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*ELE*/ new float[] { 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*GRAS*/ new float[] { 1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*ICE*/ new float[] { 1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*FIG*/ new float[] { 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*POI*/ new float[] { 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*GRO*/ new float[] { 1f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*FLY*/ new float[] { 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*PSY*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*BUG*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f },
        /*ROC*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 1f },
        /*GHO*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f },
        /*DRA*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f }
    };

    static public float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
    
}