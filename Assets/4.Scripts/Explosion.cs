﻿using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	//宣告:爆炸範圍、力量、傷害值、結束時間
	public float explosionRadius = 5.0f;
	public float explosionPower = 10.0f;
	public float explosionDamage = 100.0f;
	public float explosionTimeout = 2.0f;

	//功能:初始化
	//對爆炸範圍內目標(ApplyDamage)發送傷害值，並對鋼鐵發送力量
	//刪除自己
	// Use this for initialization
	IEnumerator Start () {
	
		var explosionPosition = transform.position;
		
		// Apply damage to close by objects first
		Collider[] colliders  = Physics.OverlapSphere (explosionPosition, explosionRadius);

		foreach (var hit in colliders) {
			// Calculate distance from the explosion position to the closest point on the collider
			var closestPoint = hit.ClosestPointOnBounds(explosionPosition);
			var distance = Vector3.Distance(closestPoint, explosionPosition);
			
			// The hit points we apply fall decrease with distance from the explosion point
			var hitPoints = 1.0f - Mathf.Clamp01(distance / explosionRadius);
			hitPoints *= explosionDamage;
			
			// Tell the rigidbody or any other script attached to the hit object how much damage is to be applied!
			hit.SendMessageUpwards("ApplyDamage", hitPoints, SendMessageOptions.DontRequireReceiver);
		}
		
		// Apply explosion forces to all rigidbodies
		// This needs to be in two steps for ragdolls to work correctly.
		// (Enemies are first turned into ragdolls with ApplyDamage then we apply forces to all the spawned body parts)
		colliders = Physics.OverlapSphere (explosionPosition, explosionRadius);

		foreach (var hit in colliders) {
			if (hit.GetComponent<Rigidbody>())
				hit.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPosition, explosionRadius, 3.0f);
		}
		
		// stop emitting particles
		if (GetComponent<ParticleEmitter>()) {
			GetComponent<ParticleEmitter>().emit = true;

			yield return new WaitForSeconds(0.5f);

			GetComponent<ParticleEmitter>().emit = false;
		}
		
		// destroy the explosion after a while
		Destroy (gameObject, explosionTimeout);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
