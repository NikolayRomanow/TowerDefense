using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbstractClasses
{
    public class ShieldSpell : MonoBehaviour
    {
        public bool isHoly;
        
        public virtual void Shield()
        {
            var playersBase = UnitsManager.AllUnits.Find(item => item.isBuilding && !item.isEnemy);

            var myUnits = UnitsManager.AllUnits.FindAll(unit => !unit.isEnemy && !unit.isBuilding);

            if (myUnits.Count <= 0)
                return;

            var rangerUnit = myUnits[0];

            foreach (var unit in myUnits)
            {
                if (Vector3.Distance(rangerUnit.transform.position, playersBase.transform.position) < 
                    Vector3.Distance(unit.transform.position, playersBase.transform.position))
                    rangerUnit = unit;
            }

            rangerUnit.currentArmored = 100f;
            rangerUnit.armoredSlider.maxValue = 100f;
            rangerUnit.armoredSlider.value = 100f;
            rangerUnit.armoredSlider.gameObject.SetActive(true);

            if (isHoly)
                rangerUnit.InShield = true;
            
            ShellsList.AllShells.Remove(this.gameObject);
            Destroy(gameObject);
        }
    }
}
