using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using EmeraldAI.Example;

public class AllyInteractionTests : InputTestFixture
{
    [UnityTest]
    public IEnumerator AllyBluntBreak()
    {
        LogAssert.ignoreFailingMessages = true;

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
        

        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Shell (Enemy)");
        Object testObjectPrefab = Resources.Load("Breakables/Rock");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
        GameObject testObject = GameObject.Instantiate((GameObject)testObjectPrefab, PlayerController.player.transform.position + new Vector3(10f, 0f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Move(mouse.position, new Vector2(960f, 650f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < 8; i++)
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
        Assert.IsTrue(null == testObject);
    }

    [UnityTest]
    public IEnumerator AllyBluntNotBreak()
    {
        LogAssert.ignoreFailingMessages = true;

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


        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Shell (Enemy)");
        Object testObjectPrefab = Resources.Load("Breakables/Thorn Vines_01");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
        GameObject testObject = GameObject.Instantiate((GameObject)testObjectPrefab, PlayerController.player.transform.position + new Vector3(12f, 0f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Move(mouse.position, new Vector2(960f, 650f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < 8; i++)
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
        Assert.IsTrue(null != testObject);
    }

    [UnityTest]
    public IEnumerator AllySharpBreak()
    {
        LogAssert.ignoreFailingMessages = true;

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


        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Spider (Enemy)");
        Object testObjectPrefab = Resources.Load("Breakables/Thorn Vines_01");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
        GameObject testObject = GameObject.Instantiate((GameObject)testObjectPrefab, PlayerController.player.transform.position + new Vector3(12f, 0f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Move(mouse.position, new Vector2(960f, 650f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < 8; i++)
        {
            Click(mouse.rightButton);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        PressAndRelease(keyboard.digit1Key);
        yield return new WaitForSeconds(0.05f);
        Move(mouse.position, new Vector2(960f, 800f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        Click(mouse.leftButton);


        yield return new WaitForSeconds(4f);
        Assert.IsTrue(null == testObject);
    }

    [UnityTest]
    public IEnumerator AllySharpNotBreak()
    {
        LogAssert.ignoreFailingMessages = true;

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


        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Spider (Enemy)");
        Object testObjectPrefab = Resources.Load("Breakables/Rock");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
        GameObject testObject = GameObject.Instantiate((GameObject)testObjectPrefab, PlayerController.player.transform.position + new Vector3(10f, 0f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Move(mouse.position, new Vector2(960f, 650f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < 8; i++)
        {
            Click(mouse.rightButton);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        PressAndRelease(keyboard.digit1Key);
        yield return new WaitForSeconds(0.05f);
        Move(mouse.position, new Vector2(960f, 800f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        Click(mouse.leftButton);


        yield return new WaitForSeconds(4f);
        Assert.IsTrue(null != testObject);
    }

    [UnityTest]
    public IEnumerator AllyInteractKey()
    {
        LogAssert.ignoreFailingMessages = true;

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


        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Spider (Enemy)");
        Object testObjectPrefab = Resources.Load("Interactables/Key");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
        GameObject testObject = GameObject.Instantiate((GameObject)testObjectPrefab, PlayerController.player.transform.position + new Vector3(12f, 0f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Move(mouse.position, new Vector2(960f, 650f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < 8; i++)
        {
            Click(mouse.rightButton);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        PressAndRelease(keyboard.digit1Key);
        yield return new WaitForSeconds(0.05f);
        Move(mouse.position, new Vector2(960f, 800f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        Click(mouse.leftButton);


        yield return new WaitForSeconds(4f);
        Assert.IsTrue(null == testObject);
    }

    [UnityTest]
    public IEnumerator AllyInteractLever()
    {
        LogAssert.ignoreFailingMessages = true;

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
        Object testObjectPrefab = Resources.Load("Interactables/Lever2");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab, PlayerController.player.transform.position + new Vector3(6f, 0f, 0f), Quaternion.identity);
        GameObject testObject = GameObject.Instantiate((GameObject)testObjectPrefab, PlayerController.player.transform.position + new Vector3(12f, 0f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        Mouse mouse = InputSystem.AddDevice<Mouse>();
        Move(mouse.position, new Vector2(960f, 650f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < 8; i++)
        {
            Click(mouse.rightButton);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        PressAndRelease(keyboard.digit1Key);
        yield return new WaitForSeconds(0.05f);
        Move(mouse.position, new Vector2(960f, 800f), queueEventOnly: true);
        yield return new WaitForSeconds(0.05f);
        Click(mouse.leftButton);

        Quaternion originalRotation = testObject.transform.GetChild(0).transform.rotation;
        yield return new WaitForSeconds(4f);
        Quaternion afterRotation = testObject.transform.GetChild(0).transform.rotation;
        Assert.IsTrue(originalRotation != afterRotation);
    }
}
