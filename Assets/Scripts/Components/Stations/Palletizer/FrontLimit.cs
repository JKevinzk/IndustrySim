using System;
using UnityEngine;
using static Components.Stations.Palletizer.PalletizerName;

namespace Components.Stations.Palletizer
{
    public class FrontLimit : ElevatorLimit, IPalletizerPart
    {
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (state)
                Debug.Log("Front检测器探测到物体");
        }

        public void SetBit(Palletizer palletizer)
        {
            bit = palletizer.GetSensorBit(SElevatorFrontLimit);
        }
    }
}