using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class UnitStatusTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void UnitStatusTestsSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator UnitStatusTestsWithEnumeratorPasses()
    {
        // first goto Initialization to setup
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("Initialization");
        while (!loadingScene.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        // // then goto Catacomb_Floor3 (or another scene for testing) to do more setup
        // loadingScene = SceneManager.LoadSceneAsync("Catacomb_Floor3");
        // while (!loadingScene.isDone)
        // {
        //     yield return null;
        // }
        // yield return new WaitForSeconds(0.1f);

        // now we can create a unit and test it.
        Object testUnitPrefab = Resources.Load("EmeraldAIUnits/Bud (Enemy)");
        GameObject testUnit = GameObject.Instantiate((GameObject)testUnitPrefab);
        UnitStatus unitStatus = testUnit.GetComponent<UnitStatus>();

        // this is one way to access a private field from another class.
        FieldInfo isStunnedField = typeof(UnitStatus).GetField("isStunned", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsFalse((bool)isStunnedField.GetValue(unitStatus));
        float stunDuration = 1f;
        unitStatus.Stun(stunDuration);
        yield return new WaitForSeconds(stunDuration/2f);
        Assert.IsTrue((bool)isStunnedField.GetValue(unitStatus));
        yield return new WaitForSeconds(stunDuration/2f+0.1f);
        Assert.IsFalse((bool)isStunnedField.GetValue(unitStatus));

    }
}
