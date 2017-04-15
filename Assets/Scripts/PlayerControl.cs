using UnityEngine;

public class PlayerControl : MonoBehaviour
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

	    Debug.Log("Current Health: " + Health + "\nCurrent Mana: " + Mana);

	}
	
	// Update is called once per frame
    private void Update ()
    {
		
	}

    private void FixedUpdate()
    {

    }

    public void PlayerDamage(int damage)
    {
        Health -= damage;
    }

}
