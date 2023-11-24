using UnityEngine;

[RequireComponent(typeof(ObjectPermanent))]
public class Breakable : MonoBehaviour
{
    public GameObject residue;
    protected ObjectPermanent permanent;

    private void Awake() {
        permanent = GetComponent<ObjectPermanent>();
    }

    public void Break(bool forGood = true) {
        Instantiate(residue, transform.position+Vector3.up*0.5f, Quaternion.identity);
        if ((permanent != null) && forGood)
            permanent.SetDestroy();
        Destroy(this.gameObject);
    }

}