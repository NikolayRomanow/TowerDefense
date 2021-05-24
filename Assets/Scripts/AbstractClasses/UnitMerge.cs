using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbstractClasses
{
    public class UnitMerge : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                
                if (touch.phase == TouchPhase.Began)
                {
                    RaycastHit2D hit = Physics2D.Raycast(_camera.ScreenToWorldPoint
                        (touch.position), Vector3.forward * 10);
                    
                    if (!hit.collider)
                        return;

                    if (hit.collider.TryGetComponent(out Unit unit))
                    {
                        if (unit.isEnemy)
                            return;
                        
                        if (unit.touchedByFinger)
                        {
                            unit.touchedByFinger = false;
                            if (!unit.canFly)
                                unit.swfClip.sequence = "walk";
                            else
                                unit.swfClip.sequence = "fly";
                        }
                        else
                        {
                            unit.touchedByFinger = true;
                            unit.swfClip.sequence = "idle";
                        }
                    }
                }
            }
        }
    }
}
