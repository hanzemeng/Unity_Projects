using System.Collections.Generic;
using UnityEngine;


public class MakeTransparent : MonoBehaviour
{
    public Transform player;
    public Vector3 offest;
    [SerializeField]
    private List<Transform> ObjectToHide = new List<Transform>();
    private List<Transform> ObjectToShow = new List<Transform>();
    private Dictionary<Transform, Material> originalMaterials = new Dictionary<Transform, Material>();
    [SerializeField] private float ObstructionFadingSpeed;
    [SerializeField] private float transparencyThreshold;

    private void LateUpdate()
    {
        if (player != null)
        {
            ManageBlockingView();

            foreach (var obstruction in ObjectToHide)
            {
                if (obstruction != null)
                    HideObstruction(obstruction);
            }

            foreach (var obstruction in ObjectToShow)
            {
                if (obstruction != null)
                    ShowObstruction(obstruction);
            }
        }

    }


    private void ManageBlockingView()
    {
        Vector3 playerPosition = player.transform.position + offest;
        float characterDistance = Vector3.Distance(transform.position, playerPosition);
        int layerNumber = LayerMask.NameToLayer("Wall");
        int layerMask = 1 << layerNumber;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, (playerPosition - transform.position).normalized, characterDistance, layerMask);
        if (hits.Length > 0)
        {
            // Repaint all the previous obstructions. Because some of the stuff might be not blocking anymore
            foreach (var obstruction in ObjectToHide)
            {
                if (obstruction != null)
                    ObjectToShow.Add(obstruction);
            }

            ObjectToHide.Clear();

            // Hide the current obstructions
            foreach (var hit in hits)
            {
                Transform obstruction = hit.transform;
                ObjectToHide.Add(obstruction);
                ObjectToShow.Remove(obstruction);
                SetModeTransparent(obstruction);
            }
        }
        else
        {
            // Mean that no more stuff is blocking the view and sometimes all the stuff is not blocking as the same time

            foreach (var obstruction in ObjectToHide)
            {
                if (obstruction != null)
                    ObjectToShow.Add(obstruction);
            }

            ObjectToHide.Clear();

        }
    }

    private void HideObstruction(Transform obj)
    {
        //obj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        var color = obj.GetComponent<Renderer>().material.color;
        color.a = Mathf.Max(transparencyThreshold, color.a - ObstructionFadingSpeed * Time.deltaTime);
        obj.GetComponent<Renderer>().material.color = color;

    }

    private void SetModeTransparent(Transform tr)
    {
        MeshRenderer renderer = tr.GetComponent<MeshRenderer>();
        Material originalMat = renderer.sharedMaterial;
        if (!originalMaterials.ContainsKey(tr))
        {
            originalMaterials.Add(tr, originalMat);
        }
        else
        {
            return;
        }
        Material materialTrans = new Material(originalMat);
        materialTrans.CopyPropertiesFromMaterial(originalMat);
        materialTrans.SetInt("_SrcBlend",(int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        materialTrans.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        materialTrans.SetInt("_ZWrite", 0);
        materialTrans.SetInt("_Surface", 1);

        materialTrans.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        materialTrans.SetShaderPassEnabled("DepthOnly", false);
        materialTrans.SetShaderPassEnabled("SHADOWCASTER", true);

        materialTrans.SetOverrideTag("RenderType", "Transparent");
        materialTrans.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        materialTrans.EnableKeyword("_ALPHAPREMULTIPLY_ON");    

        renderer.material = materialTrans;
        renderer.material.mainTexture = originalMat.mainTexture;
    }

    private void SetModeOpaque(Transform tr)
    {
        if (originalMaterials.ContainsKey(tr))
        {
            tr.GetComponent<MeshRenderer>().material = originalMaterials[tr];
            originalMaterials.Remove(tr);
        }

    }

    private void ShowObstruction(Transform obj)
    {
        var color = obj.GetComponent<Renderer>().material.color;
        color.a = Mathf.Min(1, color.a + ObstructionFadingSpeed * Time.deltaTime);
        obj.GetComponent<Renderer>().material.color = color;
        if (Mathf.Approximately(color.a, 1f))
        {
            SetModeOpaque(obj);
        }
    }
}

