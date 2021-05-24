using System;
using System.Collections;
using System.Collections.Generic;
using Heroes;
using UnityEngine;
using UnityEngine.UI;

namespace AbstractClasses
{
    public class ButtonsManager : MonoBehaviour
    {
        public List<Image> frames;

        public WarriorUnitSpawner WarriorUnitSpawner;
        public Mag Mag;
        public Righthand Righthand;

        private void Awake()
        {
            WarriorUnitSpawner = FindObjectOfType<WarriorUnitSpawner>();
            Mag = FindObjectOfType<Mag>();
            Righthand = FindObjectOfType<Righthand>();
        }

        public void ClearFrames()
        {
            foreach (var element in frames)
            {
                element.color = Color.white;
            }

            Mag.effect = 1;
            WarriorUnitSpawner.effect = 1;
            Righthand.effect = 1;
        }
    }
}
