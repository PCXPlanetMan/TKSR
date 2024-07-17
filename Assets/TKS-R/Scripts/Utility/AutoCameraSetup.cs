using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace TKSR
{
    public class AutoCameraSetup : MonoBehaviour
    {
        public bool autoSetupCameraFollow = true;
        public string cameraFollowGameObjectName = "CameraFollowTarget";

        private readonly float DefaultCameraOrthHeight = 4f;
        private readonly float BattleCameraOrthHeight = 5f;

        void Awake ()
        {
            if(!autoSetupCameraFollow)
                return;

            CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera> ();
            cam.m_Lens.OrthographicSize = DefaultCameraOrthHeight;
        
            if(cam == null)
                throw new UnityException("Virtual Camera was not found, default follow cannot be assigned.");

            //we manually do a "find", because otherwise, GameObject.Find seem to return object from a "preview scene" breaking the camera as the object is not the right one
            var rootObj = gameObject.scene.GetRootGameObjects();
            GameObject cameraFollowGameObject = null;
            foreach (var go in rootObj)
            {
                if (go.name == cameraFollowGameObjectName)
                    cameraFollowGameObject = go;
                else
                {
                    var t = go.transform.Find(cameraFollowGameObjectName);
                    if (t != null)
                        cameraFollowGameObject = t.gameObject;
                }

                if (cameraFollowGameObject != null) break;
            }
        
            if(cameraFollowGameObject == null)
                throw new UnityException("GameObject called " + cameraFollowGameObjectName + " was not found, default follow cannot be assigned.");

            if (cam.Follow == null)
            {
                cam.Follow = cameraFollowGameObject.transform;
            }


            MakeCameraOrthLimit(cam);
        }

        /// <summary>
        /// 强制相机尺寸保证视野正确
        /// </summary>
        /// <param name="cam"></param>
        private void MakeCameraOrthLimit(CinemachineVirtualCamera cam)
        {
            GameObject goMap = GameObject.FindWithTag("Map");
            if (goMap != null)
            {
                var sr = goMap.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    var bounds = sr.sprite.bounds;
                    var preferredCamOrth = bounds.extents.x * Screen.height / Screen.width;
                    if (preferredCamOrth < 4)
                    {
                        Debug.Log($"PreferredCamOrth = {preferredCamOrth}" );
                        cam.m_Lens.OrthographicSize = preferredCamOrth;
                    }
                }
            }
        }

        /// <summary>
        /// 强制设置相机焦点
        /// </summary>
        /// <param name="follower"></param>
        public void SetCameraFollower(GameObject follower)
        {
            autoSetupCameraFollow = false;
            
            CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera> ();
            cam.m_Lens.OrthographicSize = DefaultCameraOrthHeight;
            
            cam.Follow = follower.transform;

            MakeCameraOrthLimit(cam);
        }

        /// <summary>
        /// 为战场提供视角追踪
        /// </summary>
        /// <param name="targetFollower"></param>
        public void UpdateBattleCameraFollower(Transform targetFollower)
        {
            if (autoSetupCameraFollow)
            {
                return;
            }
            
            CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera> ();
            cam.m_Lens.OrthographicSize = BattleCameraOrthHeight;
            cam.Follow = targetFollower;
        }
    }
}