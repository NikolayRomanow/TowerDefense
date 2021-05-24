using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FTRuntime;
using Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AbstractClasses
{
    public enum UnitType
    {
        Melee,
        Range
    }
    
    [RequireComponent(typeof(BoxCollider2D))]
    public class Unit : MonoBehaviour
    {
        public string unitName;

        [Header("Characteristics")] 
        public UnitType unitType;
        public bool canFly;
        public bool isEnemy;
        public bool isBuilding;
        public bool attackBonus;
        public float minAttackPower;
        public float maxAttackPower;
        public float currentArmored;
        [SerializeField] private float attackRecharge;
        [SerializeField] private float maxHealth;
        [SerializeField] private float armor;
        public float movementSpeed;
        [SerializeField] private float distanceAttack;

        [Header("Data")]
        public bool touchedByFinger;
        public bool InShield;
        public bool badMove;
        public float currentHealth;
        public Transform targetCastle;
        [SerializeField] private Unit enemyUnit;
        [SerializeField] private bool onBattle;
        [SerializeField] private Slider slider;
        public Slider armoredSlider;

        public SwfClip swfClip;
        public SwfClipController swfClipController;

        public Animator animatorDamage;
        private Coroutine _attackCoroutine;
        private BoxCollider2D _boxCollider2D;

        public TextMeshProUGUI damageText;
        
        IEnumerator AttackEnemyUnit(Unit enemy)
        {
            while (enemy != null)
            {
                if (enemy.currentHealth <= 0)
                    break;
                
                float damage = Random.Range(minAttackPower, maxAttackPower);

                if (attackBonus)
                    damage *= 1.1f;
                    
                enemy.AttackThisUnit(damage, this);

                yield return new WaitForSeconds(attackRecharge);
            }

            if (!canFly)
                swfClip.sequence = "walk";
            else
                swfClip.sequence = "fly";
            
            enemyUnit = null;
            
            onBattle = false;
            
            _attackCoroutine = null;
        }

        public void AttackThisUnit(float damage, Unit unit)
        {
            if (InShield && unit != null)
            {
                unit.AttackThisUnit(100, this);
                unit.transform.position = new Vector3(unit.transform.position.x + 5f,
                    unit.transform.position.y, unit.transform.position.z);

                InShield = false;
            }
             
            if (!isBuilding)
            {
                if (currentArmored > 0)
                {
                    armoredSlider.gameObject.SetActive(true);
                    damageText.color = Color.blue;
                }
                else
                {
                    damageText.color = Color.red;
                    armoredSlider.gameObject.SetActive(false);
                }

                damageText.text = $"-{(int)damage}";
                animatorDamage.Play("Damager");
            }

            damage -= armor;

            if (damage < 0)
                damage = 0;

            if (currentArmored > 0)
            {
                if (damage > currentArmored)
                {
                    damage -= currentArmored;
                    currentArmored = 0;
                    armoredSlider.value = currentArmored;
                }

                else
                {
                    currentArmored -= damage;
                    armoredSlider.value = currentArmored;
                    return;
                }
            }
            
            currentHealth -= damage;
            
            if (!isBuilding)
                slider.value = currentHealth;
            
            if (currentHealth <= 0)
            {
                onBattle = true;
                
                UnitsManager.AllUnits.Remove(this);
                
                if (_attackCoroutine != null)
                    StopCoroutine(_attackCoroutine);

                if (isBuilding)
                {
                    UnitsManager.AllUnits = new List<Unit>();
                    ShellsList.AllShells = new List<GameObject>();
                    
                    Destroy(gameObject);
                    SceneManager.LoadScene(0);
                }

                else
                {
                    
                    swfClipController.loopMode = SwfClipController.LoopModes.Once;
                    swfClip.sequence = "dead";
  
                    Destroy(gameObject, 1f);
                }
            }
        }
        
        private void Movement()
        {
            var currentPosition = transform.position;
            var targetPosition = targetCastle.position;

            targetPosition = new Vector3(targetPosition.x, currentPosition.y, currentPosition.z);

            var move = movementSpeed;

            if (badMove)
                move *= 0.3f;
            
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, move * 5f);
        }

        private void StartAttack(Unit enemy)
        {
            swfClip.sequence = "attack";
            enemyUnit = enemy;
            onBattle = true;
            _attackCoroutine = StartCoroutine(AttackEnemyUnit(enemy));
        }
        
        private void Awake()
        {
            if (!isBuilding)
            {
                damageText = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();

                armoredSlider.gameObject.SetActive(false);

                animatorDamage = GetComponent<Animator>();
                
                currentHealth = maxHealth;
                movementSpeed /= 100f;

                if (armoredSlider)
                {
                    armoredSlider.value = 0f;
                    armoredSlider.direction = Slider.Direction.RightToLeft;
                }

                if (slider)
                {
                    slider.direction = Slider.Direction.RightToLeft;
                }
                
                slider.maxValue = maxHealth;
                slider.value = currentHealth;

                _boxCollider2D = GetComponent<BoxCollider2D>();
                _boxCollider2D.isTrigger = true;

                swfClip = GetComponent<SwfClip>();
                
                if (!canFly)
                    swfClip.sequence = "walk";
                else
                    swfClip.sequence = "fly";
                
                swfClipController = GetComponent<SwfClipController>();
            }
                
            UnitsManager.AllUnits.Add(this);
        }

        private void Update()
        {
            if (isBuilding)
                return;
            
            slider.value = currentHealth;
            
            if (touchedByFinger)
                return;
                
            var myMergeUnit = UnitsManager.AllUnits.Find(unit =>
                unit.touchedByFinger && unit.transform.position == this.transform.position && unit != this);
            
            if (myMergeUnit)
                UnitsManager.WarriorUnitSpawner.MergeUnits(this, myMergeUnit);
            
            if (enemyUnit)
                return;

            var otherUnits = UnitsManager.AllUnits;

            if (isEnemy)
                otherUnits = otherUnits.FindAll(unit => !unit.isEnemy);
            else
                otherUnits = otherUnits.FindAll(unit => unit.isEnemy);
            
                

            if (!canFly)
            {
                otherUnits = otherUnits.FindAll(unit => !unit.canFly);
                otherUnits = otherUnits.FindAll(unit =>
                    Vector3.Distance(this.transform.position, unit.transform.position) <= distanceAttack * 2f);
            }
            else
                otherUnits = otherUnits.FindAll(unit =>
                    Vector3.Distance(this.transform.position, unit.transform.position) <= distanceAttack * 6f);

            Unit shortlyUnit = null;
            
            if (otherUnits.Count <= 0)
                return;

            shortlyUnit = otherUnits[0];
            
            foreach (var unit in otherUnits)
            {
                if (Vector3.Distance(shortlyUnit.transform.position, this.transform.position) >
                    Vector3.Distance(unit.transform.position, this.transform.position))
                    shortlyUnit = unit;

                if (unit.isBuilding)
                {
                    shortlyUnit = unit;
                    break;
                }
            }
            
            StartAttack(shortlyUnit);
        }

        private void FixedUpdate()
        {
            if (isBuilding)
                return;
            
            if (!onBattle && !touchedByFinger)
                Movement();
        }
    }
    
    public static class UnitsManager
    {
        public static List<Unit> AllUnits = new List<Unit>();

        public static WarriorUnitSpawner WarriorUnitSpawner;
    }
}
