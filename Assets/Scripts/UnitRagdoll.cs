using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;
    [SerializeField] private Transform characterRagdoll;

    private float ragdollExplosionForce = 200f;
    private float ragdollExplosionRadius = 10f;

    private float timeBeforeBlink = 3f;
    private float blinkTimeInterval = 1f;
    private float disappearTimer = 7f;

    

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);

        ApplyExplosionToRagdoll(ragdollRootBone, ragdollExplosionForce, transform.position, ragdollExplosionRadius);

    }

    private void Update()
    {
        timeBeforeBlink -= Time.deltaTime;
        if (timeBeforeBlink > 0) return;
        else
        {
            blinkTimeInterval -= Time.deltaTime;
            disappearTimer -= Time.deltaTime;

            if (blinkTimeInterval <= 0)
            {

                blinkTimeInterval = Mathf.Max(0.1f, 0.14f * disappearTimer);
                //SkinnedMeshRenderer characterSkinnedMeshRenderer = characterRagdoll.GetComponent<SkinnedMeshRenderer>();
                //characterSkinnedMeshRenderer.enabled=!characterSkinnedMeshRenderer.isVisible;
                characterRagdoll.gameObject.SetActive(!characterRagdoll.gameObject.activeSelf);
            }

            if (disappearTimer < 0) Destroy(gameObject);
        }
    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }


            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRadius);
        }
    }

}
