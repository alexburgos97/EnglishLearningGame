using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.Samples.EmbeddedPackages
{
    public class BoomBox : MonoBehaviour
    {
        private void Awake()
        {
            EmbeddedPackagesController.instance.AddObject(gameObject);
        }
    }
}
