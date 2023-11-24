using UnityEngine;
using EmeraldAI.Example;
using UnityEngine.Events;
public class PlayerProgression : MonoBehaviour
{
    public static PlayerProgression playerProgression;

    public int skillPoint;

    public int playerMaxHealthLevel;
    public int[] playerMaxHealthCost = {2,2,5,5,10,20};
    public int[] playerMaxHealthEffect = {20,20,30,30,50,200};
    public int playerInitialMaxHealth;

    public int playerHealthRegenerationLevel;
    public int[] playerHealthRegenerationCost = {5,10,20,30,40};
    public float[] playerHealthRegenerationEffect = {0.1f, 0.1f, 0.15f, 0.15f, 0.5f}; // subtract healthRegenFrequency
    public float playerInitialHealthRegeneration;
    
    public int playerMaxManaLevel;
    public int[] playerMaxManaCost = {2,2,5,5,10,20};
    public float[] playerMaxManaEffect = {10f,10f,15f,15f,20f,50f};
    public float playerInitialMaxMana;

    public int playerManaRegenerationLevel;
    public int[] playerManaRegenerationCost = {5,5,10,10,20,40};
    public float[] playerManaRegenerationEffect = {0.5f,0.5f,1f,1f,2f,10f};
    public float playerInitialManaRegeneration;

    public int playerMaxAllyCountLevel;
    public int[] playerMaxAllyCountCost = {10,20,50,100};
    public int[] playerMaxAllyCountEffect = {1,1,1,1};
    public int playerInitialMaxAllyCount;

    [HideInInspector] public UnityEvent onChangeEvent = new UnityEvent();

    private void Awake()
    {
        if(null != playerProgression)
        {
            Destroy(gameObject);
            return;
        }
        playerProgression = this;
    }

    public void ResetPlayerProgression()
    {
        playerMaxHealthLevel = 0;
        playerProgression.skillPoint = 0;
        playerHealthRegenerationLevel = 0;
        playerMaxManaLevel = 0;
        playerManaRegenerationLevel = 0;
        playerMaxAllyCountLevel = 0;

        PlayerController.player.gameObject.GetComponent<EmeraldAIPlayerHealth>().StartingHealth = playerInitialMaxHealth;
        PlayerController.player.gameObject.GetComponent<EmeraldAIPlayerHealth>().healthRegenFrequency = playerInitialHealthRegeneration;
        PlayerController.player.gameObject.GetComponent<UnitMagic>().maxMagic = playerInitialMaxMana;
        PlayerController.player.gameObject.GetComponent<UnitMagic>().magicRegen = playerInitialManaRegeneration;
        UnitGroupManager.current.maxAllies = playerInitialMaxAllyCount;
    }

    public void UpdatePlayerProgression()
    {
        EmeraldAIPlayerHealth emeraldAIPlayerHealth = PlayerController.player.gameObject.GetComponent<EmeraldAIPlayerHealth>();
        for(int i=0; i<playerMaxHealthLevel; i++)
        {
            emeraldAIPlayerHealth.StartingHealth += playerMaxHealthEffect[i];
        }
        for(int i=0; i<playerHealthRegenerationLevel; i++)
        {
            emeraldAIPlayerHealth.healthRegenFrequency -= playerHealthRegenerationEffect[i];
        }

        UnitMagic unitMagic =  PlayerController.player.gameObject.GetComponent<UnitMagic>();
        for(int i=0; i<playerMaxManaLevel; i++)
        {
            unitMagic.maxMagic += playerMaxManaEffect[i];
        }
        for(int i=0; i<playerManaRegenerationLevel; i++)
        {
            unitMagic.magicRegen += playerManaRegenerationEffect[i];
        }

        for(int i=0; i<playerMaxAllyCountLevel; i++)
        {
           UnitGroupManager.current.maxAllies += playerMaxAllyCountEffect[i];
        }
        onChangeEvent.Invoke();
    }

    public void UpgradeMaxHealth()
    {
        if(playerMaxHealthLevel > 5 || skillPoint < playerMaxHealthCost[playerMaxHealthLevel])
        {
            return;
        }

        skillPoint -= playerMaxHealthCost[playerMaxHealthLevel];
        EmeraldAIPlayerHealth emeraldAIPlayerHealth = PlayerController.player.gameObject.GetComponent<EmeraldAIPlayerHealth>();
        emeraldAIPlayerHealth.StartingHealth += playerMaxHealthEffect[playerMaxHealthLevel];
        playerMaxHealthLevel++;
        onChangeEvent.Invoke();
    }
    public void UpgradeHealthRegeneration()
    {
        if(playerHealthRegenerationLevel > 4 || skillPoint < playerHealthRegenerationCost[playerHealthRegenerationLevel])
        {
            return;
        }

        skillPoint -= playerHealthRegenerationCost[playerHealthRegenerationLevel];
        EmeraldAIPlayerHealth emeraldAIPlayerHealth = PlayerController.player.gameObject.GetComponent<EmeraldAIPlayerHealth>();
        emeraldAIPlayerHealth.healthRegenFrequency -= playerHealthRegenerationEffect[playerHealthRegenerationLevel];
        playerHealthRegenerationLevel++;
        onChangeEvent.Invoke();
    }
    public void UpgradeMaxMana()
    {
        if(playerMaxManaLevel > 5 || skillPoint < playerMaxManaCost[playerMaxManaLevel])
        {
            return;
        }

        skillPoint -= playerMaxManaCost[playerMaxManaLevel];
        UnitMagic unitMagic =  PlayerController.player.gameObject.GetComponent<UnitMagic>();
        unitMagic.maxMagic += playerMaxManaEffect[playerMaxManaLevel];
        playerMaxManaLevel++;
        onChangeEvent.Invoke();
    }
    public void UpgradeManaRegeneration()
    {
        if(playerManaRegenerationLevel > 5 || skillPoint < playerManaRegenerationCost[playerManaRegenerationLevel])
        {
            return;
        }

        skillPoint -= playerManaRegenerationCost[playerManaRegenerationLevel];
        UnitMagic unitMagic =  PlayerController.player.gameObject.GetComponent<UnitMagic>();
        unitMagic.magicRegen += playerManaRegenerationEffect[playerManaRegenerationLevel];
        playerManaRegenerationLevel++;
        onChangeEvent.Invoke();
    }
    public void UpgradeMaxAllyCount()
    {
        if(playerMaxAllyCountLevel > 3 || skillPoint < playerMaxAllyCountCost[playerMaxAllyCountLevel])
        {
            return;
        }

        skillPoint -= playerMaxAllyCountCost[playerMaxAllyCountLevel];
        UnitGroupManager.current.maxAllies += playerMaxAllyCountEffect[playerMaxAllyCountLevel];
        playerMaxAllyCountLevel++;
        onChangeEvent.Invoke();
    }
}
