using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialSys.Samples.EmbeddedPackages
{    
    public class EmbeddedPackagesController : MonoBehaviour
    {
        public static EmbeddedPackagesController instance;
        [SerializeField] MeshCollider boomboxBoundary;
        private List<GameObject> registeredObjects = new List<GameObject>();

        readonly string avatarID = "1";
        readonly string animationID = "2";
        readonly string attachmentID = "3";
        readonly string prefabObjectID = "4";

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }

        public void EquipAvatar()
        {
            SpatialBridge.actorService.localActor.avatar.SetAvatarBody(AssetType.EmbeddedAsset, avatarID);
        }

        public void PlayAnimation()
        {
            SpatialBridge.actorService.localActor.avatar.PlayEmote(AssetType.EmbeddedAsset, animationID);
        }

        public void EquipAttachment()
        {
            SpatialBridge.actorService.localActor.avatar.EquipAttachment(AssetType.EmbeddedAsset, attachmentID);
        }

        public void SpawnBoomBox()
        {
            SpatialBridge.spaceContentService.SpawnPrefabObject(AssetType.EmbeddedAsset, prefabObjectID, GetRandomPosition(), Quaternion.identity);
        }

        public void Reset()
        {
            SpatialBridge.actorService.localActor.avatar.ResetAvatarBody();
            SpatialBridge.actorService.localActor.avatar.ClearAttachments();
            ClearObjects();
        }

        public void AddObject(GameObject obj)
        {
            registeredObjects.Add(obj);
        }

        private void ClearObjects()
        {
            foreach (var obj in registeredObjects) { Destroy(obj); }
            registeredObjects.Clear();
        }

        private Vector3 GetRandomPosition()
        {
            return new Vector3(
                UnityEngine.Random.Range(boomboxBoundary.bounds.min.x, boomboxBoundary.bounds.max.x),
                UnityEngine.Random.Range(boomboxBoundary.bounds.min.y, boomboxBoundary.bounds.max.y),
                UnityEngine.Random.Range(boomboxBoundary.bounds.min.z, boomboxBoundary.bounds.max.z)
            );
        }
    }
}

