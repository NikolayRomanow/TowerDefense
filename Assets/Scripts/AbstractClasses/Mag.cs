using System;
using System.Collections;
using System.Collections.Generic;
using Spells;
using UnityEngine;
using UnityEngine.UI;

namespace AbstractClasses
{
    public class Mag : MonoBehaviour
    {
        [SerializeField] private Image fillPunish, fillShield, fillFrost;
        [SerializeField] private float timeCreatePunish, timeCreateShield, timeCreateFrost;
        [SerializeField] private float timeCreate;
        [SerializeField] private GameObject spellPunish, spellShield, spellFrost, spellHolyShield;
        [SerializeField] private GameObject currentSpell;
        [SerializeField] private Transform spellsParent;

        public float effect = 1f;
        
        private Image currentIcon;
        private ButtonsManager _buttonsManager;
        
        private Coroutine _spawnCoroutine;
        
        private void SpawnSpell(GameObject _spell)
        {
            var spell = Instantiate(_spell, spellsParent);

            if (ShellsList.AllShells.Count <= 0)
            {
                ShellsList.AllShells.Add(spell);
            }

            else
            {
                if (ShellsList.AllShells[ShellsList.AllShells.Count - 1].TryGetComponent(out PunishSpell punishSpell) &&
                    spell.TryGetComponent(out ShieldSpell shieldSpell))
                {
                    shieldSpell.isHoly = true;
                    ShellsList.AllShells.Remove(punishSpell.gameObject);
                    Destroy(punishSpell.gameObject);
                    ShellsList.AllShells.Add(spell);
                }
                
                else if (ShellsList.AllShells[ShellsList.AllShells.Count - 1].TryGetComponent(out ShieldSpell shieldSpell2) &&
                    spell.TryGetComponent(out PunishSpell punishSpell2))
                {
                    shieldSpell2.isHoly = true;
                    ShellsList.AllShells.Remove(punishSpell2.gameObject);
                    Destroy(punishSpell2.gameObject);
                }
                
                else
                {
                    ShellsList.AllShells.Add(spell);    
                }
            }
        }

        private void Awake()
        {
            _buttonsManager = FindObjectOfType<ButtonsManager>();
        }

        private void Start()
        {
            currentSpell = spellPunish;
            timeCreate = timeCreatePunish;
            
            StartSpawn(fillPunish);
            
            SetCurrentSpell(1);
        }

        private void StartSpawn(Image image)
        {
            _spawnCoroutine = StartCoroutine(SpellSpawner(image));
        }
        
        public void SetCurrentSpell(int index)
        {
            Image _image = null;
            
            switch (index)
            {
                case 1:
                    currentSpell = spellPunish;
                    timeCreate = timeCreatePunish;
                    _image = fillPunish;
                    var enemies1 = UnitsManager.AllUnits.FindAll(x => x.isEnemy && !x.isBuilding);
                    foreach (var enemy in enemies1)
                    {
                        enemy.badMove = false;
                    }
                    fillFrost.fillAmount = 0f;
                    fillFrost.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    fillPunish.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
                    fillShield.transform.parent.transform.parent.GetComponent<Image>().enabled = false;

                    currentIcon = fillPunish;
                    break;
                case 2:
                    currentSpell = spellShield;
                    timeCreate = timeCreateShield;
                    _image = fillShield;
                    var enemies2 = UnitsManager.AllUnits.FindAll(x => x.isEnemy && !x.isBuilding);
                    foreach (var enemy in enemies2)
                    {
                        enemy.badMove = false;
                    }
                    fillFrost.fillAmount = 0f;
                    fillFrost.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    fillPunish.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    fillShield.transform.parent.transform.parent.GetComponent<Image>().enabled = true;

                    currentIcon = fillShield;
                    break;
                case 3:
                    var enemies = UnitsManager.AllUnits.FindAll(x => x.isEnemy && !x.isBuilding);
                    foreach (var enemy in enemies)
                    {
                        enemy.badMove = true;
                    }
                    StopCoroutine(_spawnCoroutine);
                    _spawnCoroutine = null;
                    fillFrost.fillAmount = 1f;
                    fillFrost.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
                    fillPunish.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    fillShield.transform.parent.transform.parent.GetComponent<Image>().enabled = false;

                    _buttonsManager.ClearFrames();
                    fillFrost.transform.parent.parent.GetComponent<Image>().color = Color.red;

                    currentIcon = fillFrost;
                    return;
            }
            
            _buttonsManager.ClearFrames();
            currentIcon.transform.parent.parent.GetComponent<Image>().color = Color.red;

            effect = 2f;
            
            if (_spawnCoroutine!=null)
            {
                StopCoroutine(_spawnCoroutine);

                _spawnCoroutine = null;
            }
            _spawnCoroutine = StartCoroutine(SpellSpawner(_image));
        }
        
        IEnumerator SpellSpawner(Image image)
        {
            while (true)
            {
                float stepSpawn = timeCreate;

                float _time = image.fillAmount * stepSpawn;
                
                while (_time < stepSpawn && image.fillAmount < 1)
                {
                    _time += Time.deltaTime * effect;
                    image.fillAmount = _time / stepSpawn;
                    yield return null;
                }
                
                //yield return new WaitForSeconds(stepSpawn);
                SpawnSpell(currentSpell);

                image.fillAmount = 0f;
            }
        }
    }

    public static class ShellsList
    {
        public static List<GameObject> AllShells = new List<GameObject>();
    }
}
