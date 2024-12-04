using System;
using System.Collections.Generic;
using Seed.DecalProjector;
using UnityEngine;

public class DecalIntroEffectMono : MonoBehaviour {
    private DecalIntroEffect decalIntroEffect;
    
    [SerializeField]
    private Transform[] seedlingTransforms;
    
    private void Start() {
        decalIntroEffect = new DecalIntroEffect(SDFCanvasController.Instance.Canvas);
        decalIntroEffect.Play(seedlingTransforms, 5f);
    }

    private void Update() {
        decalIntroEffect.Update();
    }

    private void OnDrawGizmos() {
        decalIntroEffect?.DrawGizmos();
    }
}