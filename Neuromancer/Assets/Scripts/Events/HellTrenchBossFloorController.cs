using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using EmeraldAI;
using EmeraldAI.Utility;

public class HellTrenchBossFloorController : MonoBehaviour
{
    public GameObject boss;
    public List<Transform> bossEffects;

    public void OnTriggerEnter(Collider collider)
    {
        if("Hero" != collider.tag)
        {
            return;
        }

        PlayerController.player.enabled = false;
        PlayerController.player.transform.GetComponent<Animator>().SetBool("isMoving", false);
        PlayerController.player.transform.GetComponent<Animator>().SetBool("isFalling", false);
        PlayerController.player.transform.GetComponent<Animator>().SetBool("isHoldingSpell", false);
        PlayerController.player.transform.GetComponent<Animator>().SetBool("isDashing", false);
        PlayerController.player.transform.GetComponent<Animator>().SetFloat("motionX", 0f);
        PlayerController.player.transform.GetComponent<Animator>().SetFloat("motionY", 0f);
        PlayerController.player.transform.GetComponent<PlayerAnimation>().enabled = false;
        CameraController.current.enabled = false;

        EmeraldAISystem[] units = FindObjectsOfType<EmeraldAISystem>();
        foreach(EmeraldAISystem unit in units)
        {
            unit.enabled = false;
            unit.transform.GetComponent<EmeraldAIBehaviors>().enabled = false;
            unit.transform.GetComponent<EmeraldAIDetection>().enabled = false;
            unit.transform.GetComponent<EmeraldAILookAtController>().enabled = false;
            unit.transform.GetComponent<NavMeshAgent>().enabled = false;
        }
        StartCoroutine(EndGameCoroutine());

    }

    private IEnumerator EndGameCoroutine()
    {
        Vector3 origin = CameraController.current.virtualCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset;
        Vector3 destination = new Vector3(0f, 15f,-30f);
        float originValue = CameraController.current.virtualCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value;
        float lerpAmount = 0f;
        while(lerpAmount < 1f)
        {
            lerpAmount += 2f*Time.deltaTime;
            CameraController.current.virtualCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset = Vector3.Lerp(origin, destination, lerpAmount);
            CameraController.current.virtualCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = Mathf.Lerp(originValue, 0f, lerpAmount);
            yield return null;
        }
        CameraController.current.virtualCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset = destination;
        CameraController.current.virtualCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = 0f;

        PlayerController.player.transform.GetComponent<Animator>().Play("Idle01");
        yield return new WaitForSeconds(0.5f);
        PlayerController.player.transform.GetComponent<Animator>().Play("Attack01");
        yield return new WaitForSeconds(1f);
        for(int i=0; i<bossEffects.Count; i++)
        {
            // bossEffects[i].GetComponent<ParticleSystem>()?.Play(); does not work for some reason
            ParticleSystem p = bossEffects[i].GetComponent<ParticleSystem>();
            if(null != p)
            {
                p.Play();
            }
            for(int j=0; j<bossEffects[i].childCount; j++)
            {
                bossEffects.Add(bossEffects[i].GetChild(j));
            }

        }
        yield return new WaitForSeconds(0.1f);
        boss.SetActive(false);

        yield return new WaitForSeconds(2f);
        LevelManager.levelManager.LoadLevel(LevelName.CUTSCENE_CREDITS, 0);
    }
}
