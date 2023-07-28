using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private Text dialogText;
    [SerializeField] private int lettersPerSecond;
    
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject moveSelector;
    [SerializeField] private GameObject moveDetails;
    
    [SerializeField] private List<Text> actionText;
    [SerializeField] private List<Text> moveText;
    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;
    
    
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray()){
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }


    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionText.Count; i++)
        {
            if (i == selectedAction)
            {
                actionText[i].color = Color.blue;
            }
            else
            {
                actionText[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveText.Count; i++)
        {
            if (i == selectedMove)
            {
                moveText[i].color = Color.yellow;
            }
            else
            {
                moveText[i].color = Color.black;
            }
        }
        
        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();
    }
    
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveText.Count; i++)
        {
            if (i < moves.Count)
            {
                moveText[i].text = moves[i].Base.Name;
            }
            else
            {
                moveText[i].text = "-";
            }
        }
    }

}
