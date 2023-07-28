using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;
    [SerializeField] private bool isPlayerUnit;
    
    public Pokemon Pokemon { get; set; }

    private Vector3 originalPosition;
    private Image Image;

    private void Awake()
    {
        Image = GetComponent<Image>();
        originalPosition = Image.transform.localPosition;
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            Image.transform.localPosition = new Vector3(-500f, originalPosition.y);
        else
            Image.transform.localPosition = new Vector3(500f, originalPosition.y);
        Image.transform.DOLocalMoveX(originalPosition.x, 1f);
    }
    
    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(Image.transform.DOLocalMoveX(originalPosition.x + 50f, 0.25f));
        else
            sequence.Append(Image.transform.DOLocalMoveX(originalPosition.x - 50f, 0.25f));
        sequence.Append(Image.transform.DOLocalMoveX(originalPosition.x, 0.25f));
    }
    
    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(Image.DOColor(Color.gray, 0.1f));
        sequence.Append(Image.DOColor(Color.white, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(Image.transform.DOLocalMoveY(originalPosition.y - 150f, 0.5f));
        sequence.Join(Image.DOFade(0f, 0.5f));
    }
    
    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        if(isPlayerUnit)
            Image.sprite = Pokemon.Base.BackSprite;
        else
            Image.sprite = Pokemon.Base.FrontSprite;
        Image.color = Color.white;
        PlayEnterAnimation();
    }
}
