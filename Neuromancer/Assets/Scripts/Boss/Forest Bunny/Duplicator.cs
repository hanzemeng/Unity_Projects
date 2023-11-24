using UnityEngine;

public class Duplicator : MonoBehaviour
{
    public GameObject duplicationUnitPrefab;
    public int maxDuplication;
    private int currentDuplicationCount;

    private void Start()
    {
        currentDuplicationCount = 0;
    }

    public void DuplicateAt(Vector3 newUnitPosition)
    {
        if(currentDuplicationCount >= maxDuplication)
        {
            return;
        }

        GameObject newUnit = Instantiate(duplicationUnitPrefab, newUnitPosition, Quaternion.identity);
        newUnit.GetComponent<Duplicate>().duplicator = this;
        currentDuplicationCount++;
    }
}
