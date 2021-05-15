using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicByte
{
    [ImageEffectAllowedInSceneView, ExecuteInEditMode, RequireComponent(typeof(Camera))]
    public class PostProcessingLayer : MonoBehaviour
    {
        public bool Tonemapping;
        [SerializeField]
        Tonemapping tonemapping = new Tonemapping();

        List<Effect> effects = new List<Effect>();
        public void OnRenderCamera()
        {
            clearList();
            if (Tonemapping)
            {
                effects.Add(tonemapping);
                tonemapping.preProcessing();
            }
        }
        public void clearList()
        {
            effects = new List<Effect>();
        }
        public List<Effect> getEffects() { return effects; }
    }
}