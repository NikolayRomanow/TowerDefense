using System.Collections;
using System.Collections.Generic;
using AbstractClasses;
using UnityEngine;

namespace Spells
{
    public class PunishSpell : MonoBehaviour
    {
        public void Punish()
        {
            var playersBase = UnitsManager.AllUnits.Find(item => item.isBuilding && !item.isEnemy);

            var enemies = UnitsManager.AllUnits.FindAll(unit => unit.isEnemy && !unit.isBuilding);

            if (enemies.Count <= 0)
                return;

            var shortlyUnit = enemies[0];

            foreach (var unit in enemies)
            {
                if (Vector3.Distance(shortlyUnit.transform.position, playersBase.transform.position) > 
                    Vector3.Distance(unit.transform.position, playersBase.transform.position))
                    shortlyUnit = unit;
            }
            
            shortlyUnit.AttackThisUnit(100, null);
            shortlyUnit.transform.position = new Vector3(shortlyUnit.transform.position.x + 5f,
                shortlyUnit.transform.position.y, shortlyUnit.transform.position.z);

            ShellsList.AllShells.Remove(this.gameObject);
            Destroy(gameObject);
        }
    }
}
