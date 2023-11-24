using UnityEngine;

public class SpawnMushroom : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private GameObject particleFX;
    [SerializeField] private float yOffset = 2.2f;
    [SerializeField] private GameObject enemy;
    private bool isTriggered;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        isTriggered = false;
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if(!isTriggered)
        {
            if(col.gameObject.tag == Neuromancer.Unit.HERO_TAG || Neuromancer.Unit.IsAlly(col.gameObject.transform))
            {
                isTriggered = true;
                transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
                Instantiate(particleFX, transform.position, Quaternion.identity);
                anim.SetTrigger("SpawnMushroom");
            }
        }

    }

    // Animation Event
    public void SpawnEnemy()
    {
        
        Instantiate(enemy, transform.position, this.transform.rotation);
        Destroy(this.gameObject);
    }
}
