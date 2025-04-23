using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeUI : MonoBehaviour
{
    public PlayerController player;

    [Header("UI References")]
    public GameObject skillPanel;

    [Header("AOE Skill")]
    public Button aoeUpgradeButton;
    public Text aoeText;
    public Text aoeButtonText;
    public int aoeLevel = 0;
    public int aoeMaxLevel = 20;
    public int aoeUpgradeCost = 10;
    public int baseAOEDamage = 20;

    [Header("Single Target Skill")]
    public Button singleUpgradeButton;
    public Text singleText;
    public Text singleButtonText;
    public int singleLevel = 0;
    public int singleMaxLevel = 20;
    public int singleUpgradeCost = 15;
    public int baseSingleDamage = 30;

    [Header("DPS Aura Skill")]
    public Button dpsUpgradeButton;
    public Text dpsText;
    public Text dpsButtonText;
    public int dpsLevel = 0;
    public int dpsMaxLevel = 20;
    public int dpsUpgradeCost = 20;
    public int baseDPSDamage = 10;

    void Start()
    {
        UpdateUI();

        aoeUpgradeButton.onClick.AddListener(UpgradeAOE);
        singleUpgradeButton.onClick.AddListener(UpgradeSingle);
        dpsUpgradeButton.onClick.AddListener(UpgradeDPS);

        skillPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleSkillPanel();
        }
    }

    public void ToggleSkillPanel()
    {
        bool isActive = !skillPanel.activeSelf;
        skillPanel.SetActive(isActive);

        // หยุดหรือเล่นเกมตามการเปิด/ปิดของ Panel
        Time.timeScale = isActive ? 0f : 1f; // เมื่อเปิด Panel หยุดเวลา (Time scale = 0) เมื่อปิด Panel กลับมาเล่น (Time scale = 1)

        UpdateUI();
    }

    void UpgradeAOE()
    {
        if (aoeLevel < aoeMaxLevel && player.coins >= aoeUpgradeCost)
        {
            aoeLevel++;
            player.coins -= aoeUpgradeCost;
            player.UpdateCoinUI(); // เรียก UpdateCoinUI() เพื่ออัปเดต UI ของจำนวน coin
            player.aoeUnlocked = true;
            UpdateUI();
        }
    }

    void UpgradeSingle()
    {
        if (singleLevel < singleMaxLevel && player.coins >= singleUpgradeCost)
        {
            singleLevel++;
            player.coins -= singleUpgradeCost;
            player.UpdateCoinUI(); // เรียก UpdateCoinUI() เพื่ออัปเดต UI ของจำนวน coin
            player.singleUnlocked = true;
            UpdateUI();
        }
    }

    void UpgradeDPS()
    {
        if (dpsLevel < dpsMaxLevel && player.coins >= dpsUpgradeCost)
        {
            dpsLevel++;
            player.coins -= dpsUpgradeCost;
            player.UpdateCoinUI(); // เรียก UpdateCoinUI() เพื่ออัปเดต UI ของจำนวน coin
            player.dpsUnlocked = true;
            UpdateUI();
        }
    }


    void UpdateUI()
    {
        // AOE
        if (aoeLevel == 0)
        {
            aoeText.text = "Unlock";
            aoeButtonText.text = $"Unlock ({aoeUpgradeCost}💰)";
        }
        else if (aoeLevel >= aoeMaxLevel)
        {
            aoeText.text = $"AOE Lv. MAX - Dmg: {GetAOEDamage()}";
            aoeButtonText.text = "MAX";
        }
        else
        {
            aoeText.text = $"AOE Lv. {aoeLevel}/{aoeMaxLevel} - Dmg: {GetAOEDamage()}";
            aoeButtonText.text = $"Upgrade ({aoeUpgradeCost}💰)";
        }
        aoeUpgradeButton.interactable = aoeLevel < aoeMaxLevel;

        // Single Target
        if (singleLevel == 0)
        {
            singleText.text = "Unlock";
            singleButtonText.text = $"Unlock ({singleUpgradeCost}💰)";
        }
        else if (singleLevel >= singleMaxLevel)
        {
            singleText.text = $"Single Lv. MAX - Dmg: {GetSingleDamage()}";
            singleButtonText.text = "MAX";
        }
        else
        {
            singleText.text = $"Single Lv. {singleLevel}/{singleMaxLevel} - Dmg: {GetSingleDamage()}";
            singleButtonText.text = $"Upgrade ({singleUpgradeCost}💰)";
        }
        singleUpgradeButton.interactable = singleLevel < singleMaxLevel;

        // DPS Aura
        if (dpsLevel == 0)
        {
            dpsText.text = "Unlock";
            dpsButtonText.text = $"Unlock ({dpsUpgradeCost}💰)";
        }
        else if (dpsLevel >= dpsMaxLevel)
        {
            dpsText.text = $"DPS Lv. MAX - Dmg: {GetDPSDamage()}";
            dpsButtonText.text = "MAX";
        }
        else
        {
            dpsText.text = $"DPS Lv. {dpsLevel}/{dpsMaxLevel} - Dmg: {GetDPSDamage()}";
            dpsButtonText.text = $"Upgrade ({dpsUpgradeCost}💰)";
        }
        dpsUpgradeButton.interactable = dpsLevel < dpsMaxLevel;
    }


    public int GetAOEDamage()
    {
        return baseAOEDamage + (aoeLevel * 10);
    }

    public int GetSingleDamage()
    {
        return baseSingleDamage + (singleLevel * 10);
    }

    public int GetDPSDamage()
    {
        return baseDPSDamage + (dpsLevel * 5);
    }
}
