using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using AbstractClasses;
using UnityEngine;
using UnityEngine.UI;


namespace Heroes
{
    public class WarriorUnitSpawner : MonoBehaviour
    {
        [SerializeField] private Button swordsManButton, archerButton, griffonButton;
        [SerializeField] private GameObject swordsMan, archer, griffon, knight, sniper, crossbow;
        [SerializeField] private GameObject currentChar;
        [SerializeField] private Transform spawnPointEarth, spawnPointFly;
        [SerializeField] private Transform enemyCastle;
        [SerializeField] private bool spawnFlyChar;
        [SerializeField] private float timeToSpawnSwordsMan, timeToSpawnArcher, timeToSpawnGriffon;
        [SerializeField] private float timeToSpawn;
        [SerializeField] private Image fillSwords, fillArcher, fillGriff;

        private ButtonsManager _buttonsManager;

        public float effect; 
        
        public float timeIsUpgradble;

        private Coroutine _spawnCoroutine;

        public void MergeUnits(Unit unit1, Unit unit2)
        {
            GameObject mergeUnit = null;
            
            switch (unit1.unitName)
            {
                case "SwordsMan":
                    switch (unit2.unitName)
                    {
                        case "SwordsMan":
                            mergeUnit = knight;
                            break;
                        case "Archer":
                            mergeUnit = crossbow;
                            break;
                    }
                    break;
                
                case "Archer":
                    switch (unit2.unitName)
                    {
                        case "SwordsMan":
                            mergeUnit = crossbow;
                            break;
                        case "Archer":
                            mergeUnit = sniper;
                            break;
                    }
                    break;
            }
            
            SpawnChar(mergeUnit, unit1.transform);
            DestroyUnit(unit1);
            DestroyUnit(unit2);
        }

        public void DestroyUnit(Unit unit)
        {
            UnitsManager.AllUnits.Remove(unit);
            Destroy(unit.gameObject);
        }
        
        public void SpawnChar(GameObject character, Transform spawnPos)
        {
            var pers = Instantiate(character);

            var unit = pers.GetComponent<Unit>();
            
            unit.targetCastle = enemyCastle;
            unit.transform.position = spawnPos.position;
        }
        
        public void SpawnChar(GameObject character)
        {
            Transform spawnPoint = null;
            if (spawnFlyChar)
                spawnPoint = spawnPointFly;
            else
                spawnPoint = spawnPointEarth;

            var pers = Instantiate(character);

            var unit = pers.GetComponent<Unit>();
            
            unit.targetCastle = enemyCastle;
            if (unit.canFly)
                unit.transform.position = spawnPointFly.position;
            else
                unit.transform.position = spawnPointEarth.position;
        }

        private void Awake()
        {
            UnitsManager.WarriorUnitSpawner = this;

            _buttonsManager = FindObjectOfType<ButtonsManager>();
        }

        private void Start()
        {
            currentChar = swordsMan;
            timeToSpawn = timeToSpawnSwordsMan;
            
            StartSpawn(fillSwords);
            
            SetCurrentUnit(1);
        }

        private void StartSpawn(Image image)
        {
            _spawnCoroutine = StartCoroutine(UnitSpawner(image));
        }

        public void SetCurrentUnit(int index)
        {
            Image _image = null;
            
            switch (index)
            {
                case 1:
                    currentChar = swordsMan;
                    timeToSpawn = timeToSpawnSwordsMan;
                    _image = fillSwords;
                    fillArcher.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    fillGriff.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    fillSwords.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
                    break;
                case 2:
                    currentChar = archer;
                    timeToSpawn = timeToSpawnArcher;
                    _image = fillArcher;
                    fillArcher.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
                    fillGriff.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    fillSwords.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    break;
                case 3:
                    currentChar = griffon;
                    timeToSpawn = timeToSpawnGriffon;
                    _image = fillGriff;
                    fillArcher.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    fillGriff.transform.parent.transform.parent.GetComponent<Image>().enabled = true;
                    fillSwords.transform.parent.transform.parent.GetComponent<Image>().enabled = false;
                    break;
            }
            
            _buttonsManager.ClearFrames();
            _image.transform.parent.parent.GetComponent<Image>().color = Color.red;
            effect = 2f;
            
            StopCoroutine(_spawnCoroutine);

            _spawnCoroutine = null;
            _spawnCoroutine = StartCoroutine(UnitSpawner(_image));
        }
        
        IEnumerator UnitSpawner(Image image)
        {
            while (true)
            {
                float stepSpawn = timeToSpawn;

                if (timeIsUpgradble > 0)
                    stepSpawn = stepSpawn - (stepSpawn * timeIsUpgradble);

                float _time = image.fillAmount * stepSpawn;
                
                while (_time < stepSpawn && image.fillAmount < 1)
                {
                    _time += Time.deltaTime * effect;
                    image.fillAmount = _time / stepSpawn;
                    yield return null;
                }
                
                //yield return new WaitForSeconds(stepSpawn);
                SpawnChar(currentChar);

                image.fillAmount = 0f;
            }
        }
    }
}
