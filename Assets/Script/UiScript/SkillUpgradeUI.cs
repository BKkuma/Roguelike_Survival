using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeUI : MonoBehaviour
{
    public GameObject panel;
    public Text aoeText, singleText, dpsText;
    public Button aoeButton, singleButton, dpsButton;

    public int aoeLevel = 0;
    public int singleLevel = 0;
    public int dpsLevel = 0;

    public int aoeBaseDamage = 10;
    public int singleBaseDamage = 20;
    public int dpsBaseDamage = 5;

    public int coinCostPerUpgrade = 10;

    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        panel.SetActive(false);

        aoeButton.onClick.AddListener(() =>
        {
            if (!player.aoeUnlocked)
                UpgradeAOE();
            else
                UpgradeSkill(ref aoeLevel);
        });

        singleButton.onClick.AddListener(() =>
        {
            if (!player.singleUnlocked)
                UpgradeSingle();
            else
                UpgradeSkill(ref singleLevel);
        });

        dpsButton.onClick.AddListener(() =>
        {
            if (!player.dpsUnlocked)
                UpgradeDPS();
            else
                UpgradeSkill(ref dpsLevel);
        });

        UpdateUI();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool isOpen = !panel.activeSelf;
            panel.SetActive(isOpen);
            Time.timeScale = isOpen ? 0f : 1f;
            UpdateUI();
        }
    }

    void UpgradeSkill(ref int level)
    {
        if (player.coins >= coinCostPerUpgrade)
        {
            player.coins -= coinCostPerUpgrade;
            level++;
            player.SendMessage("UpdateCoinUI");
            player.SendMessage("UpdateCoinDatabase");
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        aoeText.text = player.aoeUnlocked
            ? $"AOE Skill: Damage {GetAOEDamage()}"
            : "AOE Skill: Locked";
        aoeButton.GetComponentInChildren<Text>().text = player.aoeUnlocked
            ? $"Upgrade ({coinCostPerUpgrade} Coins)"
            : "Unlock (5 Coins)";

        singleText.text = player.singleUnlocked
            ? $"Single Skill: Damage {GetSingleDamage()}"
            : "Single Skill: Locked";
        singleButton.GetComponentInChildren<Text>().text = player.singleUnlocked
            ? $"Upgrade ({coinCostPerUpgrade} Coins)"
            : "Unlock (8 Coins)";

        dpsText.text = player.dpsUnlocked
            ? $"DPS Aura: Damage {GetDPSDamage()}"
            : "DPS Aura: Locked";
        dpsButton.GetComponentInChildren<Text>().text = player.dpsUnlocked
            ? $"Upgrade ({coinCostPerUpgrade} Coins)"
            : "Unlock (10 Coins)";
    }


    public int GetAOEDamage() => aoeBaseDamage + aoeLevel * 10;
    public int GetSingleDamage() => singleBaseDamage + singleLevel * 10;
    public int GetDPSDamage() => dpsBaseDamage + dpsLevel * 10;

    void UpgradeAOE()
    {
        if (!player.aoeUnlocked && player.coins >= 5)
        {
            player.coins -= 5;
            player.aoeUnlocked = true;
            Debug.Log("AOE Skill unlocked!");
            player.SendMessage("UpdateCoinUI");
            player.SendMessage("UpdateCoinDatabase");
            UpdateUI();
        }
    }

    void UpgradeDPS()
    {
        if (!player.dpsUnlocked && player.coins >= 10)
        {
            player.coins -= 10;
            player.dpsUnlocked = true;
            player.SendMessage("UpdateCoinUI");
            player.SendMessage("UpdateCoinDatabase");

            player.SendMessage("ResetDPSTimer");

            UpdateUI();
        }
    }

    void UpgradeSingle()
    {
        if (!player.singleUnlocked && player.coins >= 8)
        {
            player.coins -= 8;
            player.singleUnlocked = true;
            player.SendMessage("UpdateCoinUI");
            player.SendMessage("UpdateCoinDatabase");

            player.SendMessage("ResetSingleTimer");

            UpdateUI();
        }
    }

}
