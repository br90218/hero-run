using UnityEngine;

public class PCPlayerControl : MonoBehaviour
{
    public GameObject DeathEff, d;
    public float duration, delay, force = 10, squeeze = 1, bar = 1;
    public bool isDead = false, isDelaying = false, foo = false, foofoo = false;
	[SerializeField] private RespawnController _respawnController;
	// Use this for initialization
	void Start ()
	{
        force = 10;
	}
	
	// Update is called once per frame
	void Update ()
	{
        
        if (isDead == true)
        {
            
            transform.position -= new Vector3(0, 1, 0) * Time.deltaTime * squeeze;
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                squeeze = .5f;
                isDead = false;
                _respawnController.Respawn();
            }
        }
        if (isDelaying == true)
        {
            squeeze += Time.deltaTime;
            
            delay -= Time.deltaTime;
            transform.position -= new Vector3(0, 1, 0) * Time.deltaTime * squeeze * squeeze;
            if (delay <=0)
            {
                isDelaying = false;
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                foo = true;
                bar = .1f;
            }
        }
        if (foo == true)
        {
            gameObject.GetComponent<Rigidbody>().velocity = (new Vector3(0, -1, 0) * force);
            foofoo = true;
                foo = false;

        }
    }

	public void Death ()
	{
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        duration = 1.0f;
        isDelaying = false;
        delay = 1.0f;
        isDead = true;
        bar = 1.0f;
        foo = false;
        
        d = Instantiate(DeathEff, transform.position, Quaternion.Euler(-180, 0, 0));
		//play animations here
		
	}

	public void Respawn ()
	{
        isDelaying = true;
        d.transform.position = transform.position;
	}
}
