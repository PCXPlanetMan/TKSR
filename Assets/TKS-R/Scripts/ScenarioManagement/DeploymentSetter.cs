using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class DeploymentSetter : MonoBehaviour
    {
        public GameObject sameTimeDeployment;

        public void DoDeployAtTheSame()
        {
            if (sameTimeDeployment != null)
            {
                sameTimeDeployment.gameObject.SetActive(true);
            }
        }
    }
}