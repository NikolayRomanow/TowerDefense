using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AbstractClasses
{

    public enum TypeUpgrade
    {
        MeleeAttack,
        RangeAttack,
        BuildSpeed
    }
    
    public class Righthand : MonoBehaviour
    {
        [SerializeField] private Button meleeAttackButton, rangeAttackButton, buildSpeedButton;
        [SerializeField] private TypeUpgrade typeUpgrade;
        [SerializeField] private Image meleeIcon, rangeIcon, buildIcon;

        private Image currentIcon;
        private ButtonsManager _buttonsManager;

        public float effect;

        private void Awake()
        {
            _buttonsManager = FindObjectOfType<ButtonsManager>();
        }

        private void Start()
        {
            SetUpgrade(1);
        }

        public void SetUpgrade(int index)
        {
            switch (index)
            {
                case 1:
                    var playerUnits1 =
                        UnitsManager.AllUnits.FindAll(unit => !unit.isEnemy);
                    
                    foreach (var unit in playerUnits1)
                    {
                        unit.attackBonus = false;
                    }

                    buildIcon.fillAmount = 1f;
                    rangeIcon.fillAmount = 0f;
                    meleeIcon.fillAmount = 0f;

                    currentIcon = buildIcon;
                    
                    buildIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
                    rangeIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    meleeIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    
                    UnitsManager.WarriorUnitSpawner.timeIsUpgradble = 0.1f;
                    break;
                
                case 2:
                    var playerUnits2 =
                        UnitsManager.AllUnits.FindAll(unit => !unit.isEnemy && unit.unitType == UnitType.Melee);
                    
                    var playerUnitsOther =
                        UnitsManager.AllUnits.FindAll(unit => !unit.isEnemy && unit.unitType == UnitType.Range);
                    
                    foreach (var unit in playerUnits2)
                    {
                        unit.attackBonus = true;
                    }

                    foreach (var unit in playerUnitsOther)
                    {
                        unit.attackBonus = false;
                    }
                    
                    UnitsManager.WarriorUnitSpawner.timeIsUpgradble = 0f;
                    
                    buildIcon.fillAmount = 0f;
                    rangeIcon.fillAmount = 0f;
                    meleeIcon.fillAmount = 1f;

                    currentIcon = meleeIcon;
                    
                    buildIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    rangeIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    meleeIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
                    
                    break;
                
                case 3:
                    var playerUnits3 =
                        UnitsManager.AllUnits.FindAll(unit => !unit.isEnemy && unit.unitType == UnitType.Range);
                    
                    var playerUnitsOther2 =
                        UnitsManager.AllUnits.FindAll(unit => !unit.isEnemy && unit.unitType == UnitType.Melee);
                    
                    foreach (var unit in playerUnits3)
                    {
                        unit.attackBonus = false;
                    }

                    foreach (var unit in playerUnitsOther2)
                    {
                        unit.attackBonus = true;
                    }
                    
                    UnitsManager.WarriorUnitSpawner.timeIsUpgradble = 0f;
                    
                    buildIcon.fillAmount = 0f;
                    rangeIcon.fillAmount = 1f;
                    meleeIcon.fillAmount = 0f;

                    currentIcon = rangeIcon;
                    
                    buildIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    rangeIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
                    meleeIcon.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    
                    break;
            }
            
            _buttonsManager.ClearFrames();
            currentIcon.transform.parent.parent.GetComponent<Image>().color = Color.red;
        }
    }
}
