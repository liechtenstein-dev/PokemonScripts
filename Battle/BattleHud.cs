using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BattleHud : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private HpBar hpBar;

    private Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.maxHp);
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.maxHp);
    }    
}
