using UnityEngine;
using UnityEditor;
using System.Collections;

public class ParticleComponent : ExploderComponent {
	public GameObject explosionEffectsContainer;
	public float scale = 1;
	public float playbackSpeed = 1;
	public override void onExplosionStarted(Exploder exploder)
	{
		GameObject container = (GameObject)GameObject.Instantiate(explosionEffectsContainer, transform.position, Quaternion.identity);
		ParticleSystem[] systems = container.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem system in systems)
        {
            var main = system.main; // main モジュールを取得
            main.startSpeedMultiplier *= scale; // 修正
            main.startSizeMultiplier *= scale; // 修正
            system.transform.localScale *= scale;
            main.simulationSpeed = playbackSpeed; // 修正
        }

    }
}
