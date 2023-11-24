using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using EmeraldAI.Example;

public class EnemyAttackTests : InputTestFixture
{
    [UnityTest]
    public IEnumerator EnemySpawnInAttackRange()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
			Object.Destroy(o);
		}
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("Initialization");
        while (!loadingScene.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Bud (Enemy)");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab);

        yield return new WaitForSeconds(4f);
        float playerStartingHealth = PlayerController.player.transform.GetComponent<EmeraldAIPlayerHealth>().StartingHealth;
        float playerCurrentHealth = PlayerController.player.transform.GetComponent<EmeraldAIPlayerHealth>().CurrentHealth;
        Assert.IsTrue(playerCurrentHealth < playerStartingHealth);
    }

    [UnityTest]
    public IEnumerator PlayerWalksIntoEnemy()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
			Object.Destroy(o);
		}
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("Initialization");
        while (!loadingScene.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Bud (Enemy)");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(20f, 0f, 0f), Quaternion.identity);

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        Press(keyboard.wKey);
        yield return new WaitForSeconds(1f);
        Release(keyboard.wKey);

        yield return new WaitForSeconds(4f);
        float playerStartingHealth = PlayerController.player.transform.GetComponent<EmeraldAIPlayerHealth>().StartingHealth;
        float playerCurrentHealth = PlayerController.player.transform.GetComponent<EmeraldAIPlayerHealth>().CurrentHealth;
        Assert.IsTrue(playerCurrentHealth < playerStartingHealth);
    }

    [UnityTest]
    public IEnumerator EnemyWalksIntoPlayer()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
			Object.Destroy(o);
		}
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("Initialization");
        while (!loadingScene.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Slime (Enemy)");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);

        yield return new WaitForSeconds(4f);
        float playerStartingHealth = PlayerController.player.transform.GetComponent<EmeraldAIPlayerHealth>().StartingHealth;
        float playerCurrentHealth = PlayerController.player.transform.GetComponent<EmeraldAIPlayerHealth>().CurrentHealth;
        Assert.IsTrue(playerCurrentHealth < playerStartingHealth);
    }

    [UnityTest]
    public IEnumerator AllyWalksIntoEnemy()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
			Object.Destroy(o);
		}
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("Initialization");
        while (!loadingScene.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        LogAssert.ignoreFailingMessages = true;

        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Slime (Enemy)");
        GameObject testUnitEnemy = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(20f, 0f, 0f), Quaternion.identity);
        GameObject testUnitAlly = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Move(mouse.position, new Vector2(960f, 650f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        for(int i=0; i<5; i++)
        {
            Click(mouse.rightButton);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.05f);
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        PressAndRelease(keyboard.digit1Key);
        yield return new WaitForSeconds(0.05f);
        Move(mouse.position, new Vector2(960f, 800f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        Click(mouse.leftButton);

        yield return new WaitForSeconds(4f);
        if(null == testUnitAlly)
        {
            yield break;
        }
        float allyStartingHealth = testUnitAlly.transform.GetComponent<EmeraldAIPlayerHealth>().StartingHealth;
        float allyCurrentHealth = testUnitAlly.transform.GetComponent<EmeraldAIPlayerHealth>().CurrentHealth;
        Assert.IsTrue(allyCurrentHealth < allyStartingHealth);
    }

    [UnityTest]
    public IEnumerator EnemyWalksIntoAlly()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
			Object.Destroy(o);
		}
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("Initialization");
        while (!loadingScene.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        LogAssert.ignoreFailingMessages = true;

        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Slime (Enemy)");
        GameObject testUnitEnemy = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(20f, 0f, 0f), Quaternion.identity);
        GameObject testUnitAlly = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(10f, 0f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Move(mouse.position, new Vector2(960f, 750f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        for(int i=0; i<5; i++)
        {
            Click(mouse.rightButton);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(4f);
        if(null == testUnitAlly)
        {
            yield break;
        }
        float allyStartingHealth = testUnitAlly.transform.GetComponent<EmeraldAIPlayerHealth>().StartingHealth;
        float allyCurrentHealth = testUnitAlly.transform.GetComponent<EmeraldAIPlayerHealth>().CurrentHealth;
        Assert.IsTrue(allyCurrentHealth < allyStartingHealth);
    }
}
