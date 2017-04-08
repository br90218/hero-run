using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConrol : MonoBehaviour
{

    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private int _maxMana = 10;

    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public int MaxMana
    {
        get { return _maxMana; }
        set { _maxMana = value; }
    }


    public int Health { get; private set; }
    public int Mana { get; private set; }



    // Use this for initialization
    private void Start ()
	{
	    Health = _maxHealth;
	    Mana = _maxMana;

	}
	
	// Update is called once per frame
    public void Update () {
		
	}

    public void PlayerDamage(int damage)
    {
        Health -= damage;
    }
    
}
