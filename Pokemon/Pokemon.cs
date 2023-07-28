using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon : MonoBehaviour
{
    public PokemonBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Pokemon(PokemonBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = maxHp;
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
            if (Moves.Count >= 4)
            {
                break;
            }
        }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }
    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5; }
    }
    public int maxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10; }
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;
        float type1 = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1);
        float type2 = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);
        float type = type1 * type2;
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };
        
        float modifiers = Random.Range(0.85f, 1f) * type * critical;

        // Usando el assembler
        Functions functions = new Functions();
        Interpreter interpreter = new Interpreter();
        functions.interpreter = interpreter;
        
        float a = (functions.CalculateAttackValue(attacker.Level))/250f;
        float d = a * move.Base.Power * ((float) attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        Debug.Log(a);
        HP -= damage;
        functions.Dispose();
        interpreter.Dispose();
        if (HP <= 0)
        { 
            HP = 0; 
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        return Moves[Random.Range(0, Moves.Count)];
    }
    
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

