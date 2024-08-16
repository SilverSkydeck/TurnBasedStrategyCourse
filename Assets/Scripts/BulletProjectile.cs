using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer bulletTrailRenderer;
    [SerializeField] private Transform bulletHitParticleEffectPrefab;

    private Vector3 targetPosition;

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        float projectileSpeed = 35f;
        //float stopDistance = .1f;

        float currentDistance = Vector3.Distance(targetPosition, transform.position);
        transform.position += moveDirection * projectileSpeed * Time.deltaTime;
        float currentDistancePlusDT = Vector3.Distance(targetPosition, transform.position);

        if (currentDistance < currentDistancePlusDT) // projectile start to move away from the target
        {
            transform.position = targetPosition;
            bulletTrailRenderer.transform.parent = null;
            Destroy(gameObject);

            Instantiate(bulletHitParticleEffectPrefab, targetPosition, Quaternion.identity);
        }

    }

}
