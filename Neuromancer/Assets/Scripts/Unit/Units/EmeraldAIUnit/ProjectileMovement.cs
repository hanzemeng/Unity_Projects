using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{

    private float projectileLifespan;
    private float projectileSpeed;
    
    private float spawnTime;
    
    private bool fieldsSet = false;
    
    private void Start()
    {
    
        spawnTime = Time.time;
    
    }
    
    private void Update()
    {
    
        if (!fieldsSet) return;

        transform.position = transform.position + transform.forward * projectileSpeed * Time.deltaTime;

        if (Time.time > spawnTime + projectileLifespan)
        {
            
            Destroy(gameObject);
            
        }

    }

    public void SetFields(float lifespan, float speed)
    {

        projectileSpeed = speed;
        projectileLifespan = lifespan;
        fieldsSet = true;

    }

}
