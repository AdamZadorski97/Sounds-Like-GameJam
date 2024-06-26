using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckSpell : MonoBehaviour
{
    public string spellName;
    public UnityEvent OnUseSpell;
    public void SendToPlayer()
    {
        PlayerController.Instance.checkSpell = this;
    }
    public void RemoveFromPlayer()
    {
        PlayerController.Instance.checkSpell = null;
    }

    public void UseSpell(string _spellName)
    {
        if(spellName == _spellName)
        {
            OnUseSpell.Invoke();
        }
       
    }
}
